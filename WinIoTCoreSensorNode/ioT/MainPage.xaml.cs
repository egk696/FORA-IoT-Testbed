using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GrovePi;
using GrovePi.Sensors;
using System.Threading.Tasks;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;
using System.Threading;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ioT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Windows.UI.Xaml.DispatcherTimer _timer_1;
        Windows.UI.Xaml.DispatcherTimer _timer_2;
        Windows.UI.Xaml.DispatcherTimer _timer_3;

        VisualEffect vis;
        GUIDATAHANDLER handler;
        ILightSensor lightsensor_mod = DeviceFactory.Build.LightSensor(Pin.AnalogPin0);
        ISoundSensor soundsensor_mod = DeviceFactory.Build.SoundSensor(Pin.AnalogPin1);
        IDHTTemperatureAndHumiditySensor THsensor_mod = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin3, DHTModel.Dht11);
        

        volatile int max_sensor_val = 0;
        volatile int min_sensor_val = 0;

        //Sample Entries
        public List<string> lightrate_sample = new List<string>();
        public List<string> microphonerate_sample = new List<string>();
        public List<string> thrate_sample = new List<string>();

        public MainPage()
        {



            this.InitializeComponent();
            vis = new VisualEffect(forwardBut, backwardBut, page0,lightsensrate,microsensrate,THsensrate, fortostage2, fortostage3, backtostage1, backtostage2, conimage, usernamebox, passbox, senbox, calmes, pre, forw, logbox,page1,radChart);
            handler = new GUIDATAHANDLER(vis);

            //Sample Entry
            lightrate_sample.Add("0.5 sec");
            lightrate_sample.Add("1 sec");
            lightrate_sample.Add("1.5 sec");
            lightrate_sample.Add("2 sec");

            microphonerate_sample.Add("33 Hz");
            microphonerate_sample.Add("50 Hz");
            microphonerate_sample.Add("100 Hz");
            microphonerate_sample.Add("200 Hz");

            thrate_sample.Add("1 sec");
            thrate_sample.Add("2 sec");
            thrate_sample.Add("3 sec");



            //use this code
            handler.setLightSensRateValues(lightrate_sample);
            handler.setMicrophoneSensRateValues(microphonerate_sample);
            handler.setTHSensRateValues(thrate_sample);
            
            _timer_1 = new Windows.UI.Xaml.DispatcherTimer();
            _timer_1.Tick += LightSensorRead;
            _timer_2 = new Windows.UI.Xaml.DispatcherTimer();
            _timer_2.Tick += SoundSensorRead;
            _timer_3 = new Windows.UI.Xaml.DispatcherTimer();
            _timer_3.Tick += THSensorRead;

        }

        private void backwardBut_Click(object sender, RoutedEventArgs e)
        {
            int res = vis.BKpressed();
            if( res == 0)
            {
                CloseApp();
            }
            else
            {
                vis.setHome();
                _timer_1.Stop();
                _timer_2.Stop();
                _timer_3.Stop();
            }
        }

        private void forwardBut_Click(object sender, RoutedEventArgs e)
        {
            vis.setView();
            setLightRate();
            setSoundRate();
            setTHRate();
            _timer_1.Start();
            _timer_2.Start();
            _timer_3.Start();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            vis.setHome();
            
        }
        public void CloseApp()
        {
            Application.Current.Exit();
        }

        private void fortostage2_Click(object sender, RoutedEventArgs e)
        {
            handler.setConnection(usernamebox.Text, passbox.Password, senbox.Text);
        }

        private void backtostage1_Click(object sender, RoutedEventArgs e)
        {
            vis.st2tost1();
        }

        private void fortostage3_Click(object sender, RoutedEventArgs e)
        {
            vis.st2tost3();
        }

        private void backtostage2_Click(object sender, RoutedEventArgs e)
        {
            vis.st3tost2();
        }

        private void forw_Click(object sender, RoutedEventArgs e)
        {
            if(vis.getCalStage() == 0)
            {
                min_sensor_val = lightsensor_mod.SensorValue();
                vis.calst1tost2();
            }
            else if(vis.getCalStage() == 1)
            {
                max_sensor_val = lightsensor_mod.SensorValue();
                vis.calst2tost3();

            }            
        }

        private void pre_Click(object sender, RoutedEventArgs e)
        {
            if (vis.getCalStage() == 2)
            {
                vis.calst3tost2();
            }
            else if (vis.getCalStage() == 1)
            {
                vis.calst2tost1();
            }
        }

        private void lightsen_Click(object sender, RoutedEventArgs e)
        {
            vis.setVisualTo(0);
        }

        private void microsen_Click(object sender, RoutedEventArgs e)
        {
            vis.setVisualTo(1);
        }

        private void thermalsen_Click(object sender, RoutedEventArgs e)
        {
            vis.setVisualTo(2);
        }

        private void humidsen_Click(object sender, RoutedEventArgs e)
        {
            vis.setVisualTo(3);
        }
        
        private void LightSensorRead(object sender, object e)
        {
            int sensorvalue;
            lock (lightsensor_mod)
            {
                sensorvalue = lightsensor_mod.SensorValue();
            }
            sensorvalue = Convert.ToInt32(((sensorvalue - min_sensor_val) / ((max_sensor_val - min_sensor_val) * 1.0)) * 100);
            handler.addDATASETtoLightSensor(DateTime.Now, sensorvalue);
        }

        private void SoundSensorRead(object sender, object e)
        {
            int sensorvalue;
            lock (lightsensor_mod)
            {
                sensorvalue = soundsensor_mod.SensorValue();
            }
            handler.addDATASETtomicrophoneSensor(DateTime.Now, sensorvalue);
        }

        private void THSensorRead(object sender, object e)
        {
            float sensortemp;
            // Same for Humidity.  
            float sensorhum;
            lock (THsensor_mod)
            {
                THsensor_mod.Measure();
                sensortemp = Convert.ToSingle(THsensor_mod.TemperatureInCelsius);
                // Same for Humidity.  
                sensorhum = Convert.ToSingle(THsensor_mod.Humidity);
            }
            
            var time = DateTime.Now;
            handler.addDATASETtoThermalSensor(time, sensortemp);
            handler.addDATASETtoHumidSensor(time, sensorhum);
        }

        private void setLightRate()
        {
            int res = handler.getlightsamplingrate();
            switch (res)
            {
                case 0:
                    _timer_1.Interval = new TimeSpan(0, 0, 0, 0, 500);
                    break;
                case 1:
                    _timer_1.Interval = new TimeSpan(0, 0, 0, 1, 0);
                    break;
                case 2:
                    _timer_1.Interval = new TimeSpan(0, 0, 0, 1, 500);
                    break;
                case 3:
                    _timer_1.Interval = new TimeSpan(0, 0, 0, 2, 0);
                    break;
                default:
                    _timer_1.Interval = new TimeSpan(0, 0, 0, 1, 0);
                    break;
            }
        }
        private void setSoundRate()
        {
            int res = handler.getmicrosamplingrate();
            switch (res)
            {
                case 0:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 30);
                    break;
                case 1:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 20);
                    break;
                case 2:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    break;
                case 3:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 5);
                    break;
                default:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    break;
            }
        }
        
        private void setTHRate()
        {
            int res = handler.getthsamplingrate();
            switch (res)
            {
                case 0:
                    _timer_3.Interval = new TimeSpan(0, 0, 0, 1, 0);
                    break;
                case 1:
                    _timer_3.Interval = new TimeSpan(0, 0, 0, 2, 0);
                    break;
                case 2:
                    _timer_3.Interval = new TimeSpan(0, 0, 0, 3, 0);
                    break;
                default:
                    _timer_3.Interval = new TimeSpan(0, 0, 0, 2, 0);
                    break;
            }
        }

        private void stopshow_Click(object sender, RoutedEventArgs e)
        {
            vis.setVisualTo(5);
        }
    }
}

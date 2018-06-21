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

        DateTime t_from_start;

        VisualEffect vis;
        GUIDATAHANDLER handler;
        ILightSensor lightsensor_mod = DeviceFactory.Build.LightSensor(Pin.AnalogPin0);
        ISoundSensor soundsensor_mod = DeviceFactory.Build.SoundSensor(Pin.AnalogPin1);
        IDHTTemperatureAndHumiditySensor THsensor_mod = DeviceFactory.Build.DHTTemperatureAndHumiditySensor(Pin.DigitalPin3, DHTModel.Dht11);
        

        int max_sensor_val = 0;
        int min_sensor_val = 0;

        //Sample Entries
        public List<string> lightrate_sample = new List<string>();
        public List<string> microphonerate_sample = new List<string>();
        public List<string> thrate_sample = new List<string>();

        public MainPage()
        {



            this.InitializeComponent();
            vis = new VisualEffect(forwardBut, backwardBut, page0,lightsensrate,microsensrate,THsensrate, fortostage2, fortostage3, backtostage1, backtostage2, conimage, usernamebox, passbox, senbox, calmes, pre, forw, loger,page1, chart);
            handler = new GUIDATAHANDLER(vis);

            //Sample Entry
            lightrate_sample.Add("0.5 sec");
            lightrate_sample.Add("1 sec");
            lightrate_sample.Add("1.5 sec");
            lightrate_sample.Add("2 sec");

            microphonerate_sample.Add("200 Hz");
            microphonerate_sample.Add("300 Hz");
            microphonerate_sample.Add("500 Hz");
            microphonerate_sample.Add("1000 Hz");

            thrate_sample.Add("1 sec");
            thrate_sample.Add("2 sec");
            thrate_sample.Add("3 sec");



            //use this code
            handler.setLightSensRateValues(lightrate_sample);
            handler.setMicrophoneSensRateValues(microphonerate_sample);
            handler.setTHSensRateValues(thrate_sample);

            t_from_start = DateTime.Now;
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
            handler.getSamplingRate(lightsensrate.SelectedIndex, microsensrate.SelectedIndex, THsensrate.SelectedIndex);
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
                handler.getDimLightCal();
                min_sensor_val = lightsensor_mod.SensorValue();
                vis.calst1tost2();
            }
            else if(vis.getCalStage() == 1)
            {
                handler.getLightLightCal();
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

            (chart.Series[0] as LineSeries).ItemsSource = vis.lightsen;
            vis.setVisualTo(0);
        }

        private void microsen_Click(object sender, RoutedEventArgs e)
        {
            (chart.Series[0] as LineSeries).ItemsSource = vis.microsen;
            vis.setVisualTo(1);
        }

        private void thermalsen_Click(object sender, RoutedEventArgs e)
        {
            (chart.Series[0] as LineSeries).ItemsSource = vis.thermalsen;
            vis.setVisualTo(2);
        }

        private void humidsen_Click(object sender, RoutedEventArgs e)
        {
            (chart.Series[0] as LineSeries).ItemsSource = vis.humidsen;
            vis.setVisualTo(3);
        }
        private void LightSensorRead(object sender, object e)
        {
            _timer_1.Stop();
            int sensorvalue = lightsensor_mod.SensorValue();
            sensorvalue = Convert.ToInt32(((sensorvalue-min_sensor_val)/((max_sensor_val-min_sensor_val)*1.0)) * 100);
            double time = DateTime.Now.Subtract(t_from_start).TotalMilliseconds/1000;
            handler.addDATASETtoLightSensor(time, sensorvalue);
            _timer_1.Start();
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
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 40);
                    break;
                case 1:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 30);
                    break;
                case 2:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 20);
                    break;
                case 3:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 0, 10);
                    break;
                default:
                    _timer_2.Interval = new TimeSpan(0, 0, 0, 1, 0);
                    break;
            }

        }
        private void SoundSensorRead(object sender, object e)
        {
            _timer_2.Stop();
            int sensorvalue = soundsensor_mod.SensorValue();
            double time = DateTime.Now.Subtract(t_from_start).TotalMilliseconds / 1000;
            handler.addDATASETtomicrophoneSensor(time, sensorvalue);
            _timer_2.Start();
        }
        private void THSensorRead(object sender, object e)
        {
            _timer_3.Stop();
            THsensor_mod.Measure();
            float sensortemp = Convert.ToSingle( THsensor_mod.TemperatureInCelsius);
            // Same for Humidity.  
            float sensorhum = Convert.ToSingle( THsensor_mod.Humidity);

            double time = DateTime.Now.Subtract(t_from_start).TotalMilliseconds / 1000;
            handler.addDATASETtoThermalSensor(time, sensortemp);
            handler.addDATASETtoHumidSensor(time, sensorhum);
            _timer_3.Start();
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
            (chart.Series[0] as LineSeries).ItemsSource = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace ioT
{
    public class DataSet
    {
        public int Times { get; set; }
        public float Amount { get; set; }
    }
    class VisualEffect
    {
        Windows.UI.Xaml.DispatcherTimer _timer;
        Button forwardBut;
        Button backwardBut;
        Grid Page0;
        ComboBox lightsensrate;
        ComboBox microsensrate;
        ComboBox thsensrate;
        Button ft2;
        Button ft3;
        Button bt1;
        Button bt2;
        TextBox user;
        PasswordBox pass;
        TextBox senname;
        Image conimage;
        TextBlock calmes;
        Button pre;
        Button forw;
        RichTextBlock loger;
        Grid page1;
        Chart chart;
        int calstage = 0;
        int crrsenstodisp = 0;
        private int curr_page = 0;
        public List<DataSet> lightsen = new List<DataSet>();
        public List<DataSet> microsen = new List<DataSet>();
        public List<DataSet> thermalsen = new List<DataSet>();
        public List<DataSet> humidsen = new List<DataSet>();
        public VisualEffect(Button _forwardBut, Button _backwardBut, Grid _page0, ComboBox _lightsensrate, ComboBox _microsenrate, ComboBox _thsensrate, Button _ft2, Button _ft3, Button _bt1, Button _bt2, Image _conimage,TextBox _user, PasswordBox _pass, TextBox _sen, TextBlock _calmes, Button _pre, Button _forw, RichTextBlock _loger,Grid _page1,Chart _chart)
        {
            _timer = new Windows.UI.Xaml.DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            forwardBut = _forwardBut;
            backwardBut = _backwardBut;
            Page0 = _page0;
            lightsensrate = _lightsensrate;
            microsensrate = _microsenrate;
            thsensrate = _thsensrate;
            ft2 = _ft2;
            ft3 = _ft3;
            bt1 = _bt1;
            bt2 = _bt2;
            conimage = _conimage;
            user = _user;
            pass = _pass;
            senname = _sen;
            calmes = _calmes;
            pre = _pre;
            forw = _forw;
            loger = _loger;
            page1 = _page1;
            chart = _chart;
        }
        public void TimerStarter()
        {
            _timer.Start();
        }
        public void TimerStopper()
        {
            _timer.Stop();
        }
        private void _timer_Tick(object sender, object e)
        {
            if(crrsenstodisp == 0)
            {
                (chart.Series[0] as LineSeries).ItemsSource = lightsen;
            }
            else if(crrsenstodisp == 1)
            {
                (chart.Series[0] as LineSeries).ItemsSource = microsen;
            }
            else if(crrsenstodisp == 2)
            {
                (chart.Series[0] as LineSeries).ItemsSource = thermalsen;
            }
            else if(crrsenstodisp == 3)
            {
                (chart.Series[0] as LineSeries).ItemsSource = humidsen;
            }

            
            (chart.Series[0] as LineSeries).Refresh();

        }

        public void setHome()
        {
            TimerStopper();
            curr_page = 0;
            page1.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            backwardBut.Content = "Exit";
            forwardBut.Content = "Start";
            ft2.IsEnabled = true;
            ft3.IsEnabled = false;
            bt1.IsEnabled = false;
            bt2.IsEnabled = false;
            conimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/disconnected.png"));
            Page0.Visibility = Windows.UI.Xaml.Visibility.Visible;
            forwardBut.IsEnabled = false;
            lightsensrate.IsEnabled = false;
            microsensrate.IsEnabled = false;
            thsensrate.IsEnabled = false;
            user.Text = "iot.fora.ts1";
            pass.Password = "";
            senname.Text = "Sensor1";
            user.IsEnabled = true;
            pass.IsEnabled = true;
            senname.IsEnabled = true;
            calmes.Text = "";
            pre.IsEnabled = false;
            forw.IsEnabled = false;
            calstage = 0;

        }
        public int BKpressed()
        {
            if(curr_page == 0)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public void setCombolightsensrateVal(List<string> val)
        {
            lightsensrate.ItemsSource = val;
        }
        public void setCombomicrophonesensrateVal(List<string> val)
        {
            microsensrate.ItemsSource = val;
        }
        public void setComboTHsensrateVal(List<string> val)
        {
            thsensrate.ItemsSource = val;
        }

        public void st1tost2()
        {
            conimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/connected.png"));
            ft2.IsEnabled = false;
            ft3.IsEnabled = true;
            bt1.IsEnabled = true;
            bt2.IsEnabled = false;
            lightsensrate.IsEnabled = true;
            microsensrate.IsEnabled = true;
            thsensrate.IsEnabled = true;
            lightsensrate.SelectedIndex = 0;
            microsensrate.SelectedIndex = 0;
            thsensrate.SelectedIndex = 0;
            user.IsEnabled = false;
            pass.IsEnabled = false;
            senname.IsEnabled = false;
        }
        public void st2tost3()
        {
            ft2.IsEnabled = false;
            ft3.IsEnabled = false;
            bt1.IsEnabled = false;
            bt2.IsEnabled = true;
            lightsensrate.IsEnabled = false;
            microsensrate.IsEnabled = false;
            thsensrate.IsEnabled = false;

            calmes.Text = "Dim the Light sensor and Press next";
            forw.IsEnabled = true;
            calstage = 0;


        }
        public void st2tost1()
        {
            conimage.Source = new BitmapImage(new Uri("ms-appx:///Assets/disconnected.png"));
            ft2.IsEnabled = true;
            ft3.IsEnabled = false;
            bt1.IsEnabled = false;
            bt2.IsEnabled = false;
            lightsensrate.IsEnabled = false;
            microsensrate.IsEnabled = false;
            thsensrate.IsEnabled = false;
            user.IsEnabled = true;
            pass.IsEnabled = true;
            senname.IsEnabled = true;


        }
        public void st3tost2()
        {
            ft2.IsEnabled = false;
            ft3.IsEnabled = true;
            bt1.IsEnabled = true;
            bt2.IsEnabled = false;
            lightsensrate.IsEnabled = true;
            microsensrate.IsEnabled = true;
            thsensrate.IsEnabled = true;
            calmes.Text = "";
            pre.IsEnabled = false;
            forw.IsEnabled = false;
            forwardBut.IsEnabled = false;

        }
        public void calst1tost2()
        {
            
            pre.IsEnabled = true;
            calstage = 1;
            calmes.Text = "Light the Light sensor and Press next";
        }
        public void calst2tost3()
        {
            calstage = 2;
            forw.IsEnabled = false;
            calmes.Text = "";
            forwardBut.IsEnabled = true;
        }
        public void calst3tost2()
        {
            calstage = 1;
            forw.IsEnabled = true;
            calmes.Text = "Light the Light sensor and Press next";
            forwardBut.IsEnabled = false;
        }
        public void calst2tost1()
        {
            calstage = 0;
            pre.IsEnabled = false;
            calmes.Text = "Dim the Light sensor and Press next";

        }
        public int getCalStage()
        {
            return calstage;
        }
        public void setView()
        {
            curr_page = 1;
            Page0.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            forwardBut.IsEnabled = false;
            backwardBut.Content = "Back";
            page1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            TimerStarter();
            crrsenstodisp = 0;
        }
        public void setVisualTo(int a)
        {
            crrsenstodisp = a;
        }
    }
}

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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ioT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        VisualEffect vis;
        GUIDATAHANDLER handler;


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
            lightrate_sample.Add("1 sec");
            lightrate_sample.Add("2 sec");
            lightrate_sample.Add("3 sec");
            lightrate_sample.Add("4 sec");
            lightrate_sample.Add("5 sec");

            microphonerate_sample.Add("100 Hz");
            microphonerate_sample.Add("200 Hz");
            microphonerate_sample.Add("300 Hz");
            microphonerate_sample.Add("400 Hz");
            microphonerate_sample.Add("500 Hz");

            thrate_sample.Add("1 sec");
            thrate_sample.Add("2 sec");
            thrate_sample.Add("3 sec");



            //use this code
            handler.setLightSensRateValues(lightrate_sample);
            handler.setMicrophoneSensRateValues(microphonerate_sample);
            handler.setTHSensRateValues(thrate_sample);




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
            }
        }

        private void forwardBut_Click(object sender, RoutedEventArgs e)
        {
            vis.setView();
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
                vis.calst1tost2();
            }
            else if(vis.getCalStage() == 1)
            {
                handler.getLightLightCal();
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
    }
}

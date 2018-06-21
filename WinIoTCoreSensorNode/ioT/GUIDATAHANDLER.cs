using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioT
{
    class GUIDATAHANDLER
    {
        public List<string> lightrate_local;
        public List<string> microphonerate_local;
        public List<string> thrate_local;
        public VisualEffect vis;
        public GUIDATAHANDLER(VisualEffect _vis)
        {
            vis = _vis;
        }
        public void setLightSensRateValues(List<string> shownvalues)
        {
            lightrate_local = shownvalues;
            vis.setCombolightsensrateVal(lightrate_local);
        }
        public void setMicrophoneSensRateValues(List<string> shownvalues)
        {
            microphonerate_local = shownvalues;
            vis.setCombomicrophonesensrateVal(microphonerate_local);
        }
        public void setTHSensRateValues(List<string> shownvalues)
        {
            thrate_local = shownvalues;
            vis.setComboTHsensrateVal(thrate_local);
        }
        public void setConnection(string user, string pass, string sensorname)
        {
            //Your Functuion Goes Here
            Connected();
        }
        public void NowConnecting()
        {
            //Call this When you are Connecting
        }
        public void Connected()
        {
            //Call This when you are Connected
            vis.st1tost2();
        }
        public void getSamplingRate(int lightindex, int microindex, int thindex)
        {
            //Your code Goes Here
        }
        public void getDimLightCal()
        {
            //Your Code Goes Here
        }
        public void getLightLightCal()
        {
            //Your Code Goes Here
        }
        public void writetologger(string s)
        {
           
        }
        public string getlightsamplingrate()
        {
            return "100";
        }
        public string getmicrosamplingrate()
        {
            return "100";
        }
        public string getthsamplingrate()
        {
            return "100";
        }
        public void addDATASETtoLightSensor(int time, float value)
        {
            if(vis.lightsen.Count > 50)
            {
                vis.lightsen.RemoveAt(0);
            }
            vis.lightsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtomicrophoneSensor(int time, float value)
        {
            if (vis.microsen.Count > 50)
            {
                vis.microsen.RemoveAt(0);
            }
            vis.microsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtoThermalSensor(int time, float value)
        {
            if (vis.thermalsen.Count > 50)
            {
                vis.thermalsen.RemoveAt(0);
            }
            vis.thermalsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtoHumidSensor(int time, float value)
        {
            if (vis.humidsen.Count > 50)
            {
                vis.humidsen.RemoveAt(0);
            }
            vis.humidsen.Add(new DataSet() { Times = time, Amount = value });

        }
    }
}

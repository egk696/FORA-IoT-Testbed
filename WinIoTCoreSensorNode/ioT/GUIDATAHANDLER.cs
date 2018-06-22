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
        public void Connected()
        {
            //Call This when you are Connected
            vis.st1tost2();
        }
        public void writetologger(string s)
        {
            vis.setLogger(s);
        }
        public int getlightsamplingrate()
        {
            return vis.getLightsensSelectedRate();
        }
        public int getmicrosamplingrate()
        {
            return vis.getMicrosensSelectedRate();
        }
        public int getthsamplingrate()
        {
            return vis.getTHsensSelectedRate();
        }
        public void addDATASETtoLightSensor(double time, float value)
        {
            if(vis.lightsen.Count > 100)
            {
                vis.lightsen.RemoveAt(0);
            }
            vis.lightsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtomicrophoneSensor(double time, float value)
        {
            if (vis.microsen.Count > 100)
            {
                vis.microsen.RemoveAt(0);
            }
            vis.microsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtoThermalSensor(double time, float value)
        {
            if (vis.thermalsen.Count > 100)
            {
                vis.thermalsen.RemoveAt(0);
            }
            vis.thermalsen.Add(new DataSet() { Times = time, Amount = value });

        }
        public void addDATASETtoHumidSensor(double time, float value)
        {
            if (vis.humidsen.Count >100)
            {
                vis.humidsen.RemoveAt(0);
            }
            vis.humidsen.Add(new DataSet() { Times = time, Amount = value });

        }
    }
}

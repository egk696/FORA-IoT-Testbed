using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ioT
{
    class GUIDATAHANDLER
    {
        const int max_vals = 100;
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
        public void addDATASETtoLightSensor(DateTime time, float value)
        {
            vis.lightsen._Enqueue(value, max_vals);
        }
        public void addDATASETtomicrophoneSensor(DateTime time, float value)
        {
            vis.microsen._Enqueue(value, max_vals);
        }
        public void addDATASETtoThermalSensor(DateTime time, float value)
        {
            vis.thermalsen._Enqueue(value, max_vals);
        }
        public void addDATASETtoHumidSensor(DateTime time, float value)
        {
            vis.humidsen._Enqueue(value, max_vals);
        }
    }

    public static class Extensions
    {
        public static void _Enqueue<T>(this ObservableCollection<T> queue, T val, int max = int.MaxValue)
        {
            lock(queue)
            {
                if (queue.Count >= max)
                    queue.RemoveAt(0);
                queue.Add(val);
            }
        }
    }
}

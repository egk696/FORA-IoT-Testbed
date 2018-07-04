#!/usr/bin/python
# -*- coding: utf-8 -*-
import sys
import time
import math
import threading
import grovepi
import json
import requests
import datetime
from guizero import App, Text, TextBox, PushButton, Slider, Picture

# Grove specs
dht_sensor_port = 4  # port D2
pot_sensor_port = 0  # port A0
light_sensor_port = 2 # port A2
sound_sensor_port = 1 # port A1
dht_blue = 0
dht_white = 1
adc_ref = 5
grove_vcc = 5

# Settings
nodeName = "Node2"
logFileName = "sensor.log"
cloudServerURL = "10.16.168.49:82"

# Sharing
pDHT = None
pLight = None
pSound = None
stop = 0
mutex = threading.Lock()

# Sends data to cloud server
def sendToCloud(funcName,time,payload):
  resp = requests.post("http://"+cloudServerURL, data=json.dumps(payload))
  printLog("@"+time+":"+funcName+"POST-status:"+str(resp.status_code))

# Gathers sensor value for temperature and humidity
def processDHT(period):
  global mutex
  [t,h] = -1,-1
  with mutex:
    printLog("processDHT started with period %f sec" %(period))
  while stop==0:
    startT = datetime.datetime.now()
    try:
        mutex.acquire()
        # Get value from temperature sensor
        [t,h] = grovepi.dht(dht_sensor_port, dht_blue)
    except Exception as e:
        printLog("processDHT[error]: %s\r\n" % (str(e)))
    finally:
        mutex.release()
    # Send
    t = threading.Thread(target = sendToCloud, args = ("processDHT", startT.isoformat(), {'nodeid' : nodeName,'tempvals' : [{ 'timestamp' : startT.isoformat(), 'val' : t }],'humidvals' : [{ 'timestamp' : startT.isoformat(), 'val' : h }]}))
    t.start()
    time.sleep(max(period - (datetime.datetime.now()-startT).total_seconds(), 0))

# Gathers sensor values for light
def processLight(period):
  global mutex
  lightResistance = -1
  with mutex:
    printLog("processLight started with period %f sec" %(period))
  while stop==0:
    startT = datetime.datetime.now()
    try:
      mutex.acquire()
      # Get value from light sensor
      rawValue = max(1, grovepi.analogRead(light_sensor_port))
      # Calculate resistance
      lightResistance = round((float)(1023-rawValue) * 10 / rawValue,2)
    except Exception as e:
      printLog("processLight[error] %s\r\n" % (str(e)))
    finally:
      mutex.release()
    # Send
    t = threading.Thread(target = sendToCloud, args = ("processLight", startT.isoformat(), {'nodeid' : nodeName,'lightvals' : [{ 'timestamp' : startT.isoformat(), 'val' : lightResistance }]}))
    t.start()
    time.sleep(max(period - (datetime.datetime.now()-startT).total_seconds(), 0))

# Gathers sensor values for sound
def processSound(period):
  global mutex
  sound = -1
  with mutex:
    printLog("processSound started with period %f sec" %(period))
  while stop==0:
    startT = datetime.datetime.now()
    try:
      mutex.acquire()
      # Get value from sound sensor
      sound = grovepi.analogRead(sound_sensor_port)
    except Exception as e:
      printLog("processSound[error] %s\r\n" % (str(e)))
    finally:
      mutex.release()
    # Send
    t = threading.Thread(target = sendToCloud, args = ("processSound", startT.isoformat(), {'nodeid' : nodeName,'soundvals' : [{ 'timestamp' : startT.isoformat(), 'val' : sound }]}))
    t.start()
    time.sleep(max(period - (datetime.datetime.now()-startT).total_seconds(), 0))

# Function that actually starts the threads
def startSensorNode():
    global cloudServerURL, nodeName, stop, pDHT, pLight, pSound
    try:
        nodeName = nodeTxtBox.value
        cloudServerURL = serverTxtBox.value
    finally:
        # Initialize
        printLog("SensorNode '" + nodeName + "' started... pushing to server @"+cloudServerURL)
        try:
            printLog("Init GrovePi")
            # Init Pins
            grovepi.pinMode(dht_sensor_port, "INPUT")
            grovepi.pinMode(pot_sensor_port, "INPUT")
            grovepi.pinMode(light_sensor_port, "INPUT")
            grovepi.pinMode(sound_sensor_port, "INPUT")
            # Schedule Threads
            printLog("Scheduling Threads")
            pDHT = threading.Thread(target = processDHT, args=[2])
            pLight = threading.Thread(target = processLight, args=[1])
            pSound = threading.Thread(target = processSound, args=[0.001])
            pDHT.start()
            pLight.start()
            pSound.start()
        except Exception as e:
            printLog("startSensorNode[error]: " + str(e))
            exit(-1)

# Function that stops the threads
def stopSensorNode():
    global cloudServerURL, stop, pDHT, pLight, pSound
    try:
        # Check for exit
        stop = 1
        # Wait for threads to Join
        printLog("Stopping...")
    except Exception as e:
        printLog("stopSensorNode[error]:" + str(e))
        exit(-1)
  
# Logs
def printLog(txt):
    try:
        print(txt)
##        logTxtBox.value += "\n" + txt
    except NameError:
        exit(-1)

def main():
    try:
        startSensorNode()
        while stop!=0:
            pass
    except KeyboardInterrupt:
        stopSensorNode()
    pDHT.join()
    pLight.join()
    pSound.join()
    printLog("SensorNode exiting")
    exit(0)
    
  
if __name__ == "__main__":
    # execute only if run as a script
    main()
    
# Main App
##app = App(title = "SensorNode")
##nodeLabel = Text(app, align="left", text="Enter node name")
##nodeTxtBox = TextBox(app, align="right", width="20", text=nodeName)
##serverLabel = Text(app, align="left", text="Enter server ip:port")
##serverTxtBox = TextBox(app, align="right", width="20", text=cloudServerURL)
##startBtn = PushButton(app, command=startSensorNode, text="Start SensorNode")
##stopBtn = PushButton(app, command=stopSensorNode, text="Stop SensorNode")
##logLabel = Text(app, text="Log")
##logTxtBox = TextBox(app, width=100, height=20, enabled=True, multiline=True, scrollbar=True)
##app.display()
  


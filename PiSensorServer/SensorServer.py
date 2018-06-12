import time
import sched
import math
import threading
import multiprocessing
import grovepi

# Grove specs
adc_ref = 5
grove_vcc = 5

# Connections
dht_sensor_port = 4  # port D2
pot_sensor_port = 0  # port A0
light_sensor_port = 2 # port A2
sound_sensor_port = 1 # port A1
dht_blue = 0
dht_white = 1
dhtFileName = "sensor.log"

# Sensor Values
[t,h] = [0,0]
angle = 0.0
lightResistance = 0.0
sound = 0.0

# Sharing
mutex = multiprocessing.Lock()

# Server
sensorValuesMsg = "no sensor values available"

from http.server import BaseHTTPRequestHandler, HTTPServer
 
# HTTPRequestHandler class
class sensorHTTPServer_RequestHandler(BaseHTTPRequestHandler):
  # GET
  def do_GET(self):
        # Send response status code
        self.send_response(200)
 
        # Send headers
        self.send_header('Content-type','text/html')
        self.end_headers()

        # Send message back to client
        # Write content as utf-8 data
        self.wfile.write(bytes(sensorValuesMsg, "utf8"))
        return

def runSensorServer():
  # Server settings
  # Choose port 8080, for port 80, which is normally used for a http server, you need root access
  server_address = ('0.0.0.0', 8080)
  httpd = HTTPServer(server_address, sensorHTTPServer_RequestHandler)
  print('Running SensorServer on port:8080...')
  httpd.serve_forever()
  
def processDHT(period):
  global t, h, mutex
  with mutex:
    print("processDHT started with period %f sec" %(period))
  while True:
    try:
      # Get value from temperature sensor
      mutex.acquire()
      [t,h] = grovepi.dht(dht_sensor_port, dht_blue)
    except IOError as e:
      print("processDHT:IOError: %s\r\n" % (str(e)))
    except Exception as e:
      print("processDHT:Error: %s\r\n" % (str(e)))
    finally:
      mutex.release()
    time.sleep(period)
      
def processPOT(period):
  global angle, mutex
  with mutex:
    print("processPOT started with period %f sec" %(period))
  while True:
    try:
      # Get value from angle sensor
      mutex.acquire()
      rawValue = grovepi.analogRead(pot_sensor_port)
      # Calculate voltage
      voltage = round((float)(rawValue) * adc_ref / 1023, 2)
      # Calculate degrees
      angle = round(voltage*300/grove_vcc, 2)
    except IOError as e:
      print("processPOT:IOError: %s\r\n" % (str(e)))
    except Exception as e:
      print("processPOT:Error: %s\r\n" % (str(e)))
    finally:
      mutex.release()
    time.sleep(period)
        
def processLight(period):
  global lightResistance, mutex
  with mutex:
    print("processLight started with period %f sec" %(period))
  while True:
    try:
      # Get value from light sensor
      mutex.acquire()
      rawValue = max(1, grovepi.analogRead(light_sensor_port))
      # Calculate resistance
      lightResistance = round((float)(1023-rawValue) * 10 / rawValue,2)
    except IOError as e:
      print("processLight:IOError: %s\r\n" % (str(e)))
    except Exception as e:
      print("processLight:Error: %s\r\n" % (str(e)))
    finally:
      mutex.release()
    time.sleep(period)

def processSound(period):
  global sound, mutex
  with mutex:
    print("processSound started with period %f sec" %(period))
  while True:
    try:
      # Get value from sound sensor
      mutex.acquire()
      sound = grovepi.analogRead(sound_sensor_port)
      # Convert to dB
    except IOError as e:
      print("processSound:IOError: %s\r\n" % (str(e)))
    except Exception as e:
      print("processSound:Error: %s\r\n" % (str(e)))
    finally:
      mutex.release()
    time.sleep(period)

def processSensorData(period):
  global mutex, sensorValuesMsg
  while True:
    sensorValuesMsg = "%10.2f\t%2.0f\t%2.0f\t%3.1f\t%3.1f\t%3.1f" % (time.time(), t, h, angle, lightResistance, sound)
    with mutex:
      print(sensorValuesMsg)
    time.sleep(period)
    
# Main function
def main():
    try:
      print("Init GrovePi")
      # Init Pins
      grovepi.pinMode(dht_sensor_port, "INPUT")
      grovepi.pinMode(pot_sensor_port, "INPUT")
      grovepi.pinMode(light_sensor_port, "INPUT")
      grovepi.pinMode(sound_sensor_port, "INPUT")
      # Schedule Threads
      print("Schedule Threads")
      pDHT = threading.Thread(target = processDHT, args=[2])
      pLight = threading.Thread(target = processLight, args=[1])
      pPOT = threading.Thread(target = processPOT, args=[0.2])
      pSound = threading.Thread(target = processSound, args=[0.005])
      pServer = threading.Thread(target = processSensorData, args=[0.5])
      pDHT.start()
      pLight.start()
      pPOT.start()
      pSound.start()
      pServer.start()
      # Server
      print('Starting SensorServer...')
      runSensorServer()
    except Exception as e:
        print("MainError: " + str(e))
        exit(-1)
    except KeyboardInterrupt:
        print("DHTLogger closed by keyboard")
        exit(1)

    # Wait for threads to Join
    pDHT.join()
    pLight.join()
    pPOT.join()
    pSound.join()
    pServer.join()
    
    print("SensorServer exiting")
    exit(0)

if __name__ == '__main__':
    main()

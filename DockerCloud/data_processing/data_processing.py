#!/usr/bin/python
# -*- coding: utf-8 -*-

#  ---------- Data Integration Service ----------
# import BaseHTTPServer
from http.server import BaseHTTPRequestHandler, HTTPServer
# import httplib
import http.client
import time
import datetime
import json
import numpy
import dateutil.parser

HOSTNAME='0.0.0.0'
PORT_NUMBER = 80
SOUND_THRESHOLD = 500
COEFF_SOUND = 2
COEFF_HUMID = 1.7
COEFF_TEMP = 1
MAX_SOUNDF = 100
MIN_SOUNDF = 0
MAX_HUMID = 100
MIN_HUMID = 0
MAX_TEMP = 100
MIN_TEMP = 0.1
SHIFT_TEMP = 50
MAX_PROBAB = COEFF_SOUND*MAX_SOUNDF + COEFF_HUMID*MAX_HUMID + COEFF_TEMP*(1/MAX_TEMP)
MIN_PROBAB = COEFF_SOUND*MIN_SOUNDF + COEFF_HUMID*MIN_HUMID + COEFF_TEMP*(1/MIN_TEMP)

json_buffer = ''

def setBuffer(value):
    global json_buffer
    json_buffer = value

def getBuffer():
    return json_buffer

def datetimeToSeconds(timestamp_string):
#    datetime_val = datetime.datetime.strptime(timestamp_string, "%Y-%m-%dT%H:%M:%S.%fZ")
    datetime_val = dateutil.parser.parse(timestamp_string)
    return datetime_val.timestamp()/1000

def newData():
    return_string = ''

    # Retrieve json string from data_integration module
    remote_hostname = 'data-integration'
    remote_portnumber = 80
    connection = http.client.HTTPConnection(remote_hostname, remote_portnumber)
    connection.request('GET', '/')
    response = connection.getresponse()

    # if it is not possible to get data return an empty string
    if response.status != 200:
        return return_string

    json_string = response.read()

    # Extract values from the json string
    # rcv_vals = json.loads(test_string) # for testing
    rcv_vals = json.loads(json_string)
    rainpercs = numpy.zeros(len(rcv_vals))

    # print('Received values are listed below.')
    i = 0;
    for node in rcv_vals:
        # print(node['nodeid']+':')
        try:
            tempvals = []
            for tempval in node['tempvals']:
                if tempval['val'] != 'NaN':
                    tempvals.append(tempval['val'])
                # print(tempval['timestamp'])
                # print(tempval['val'])
                pass
        except KeyError:
            print('no temp vals')
        else:
            tempvals = numpy.asarray(tempvals)
            # Shift the temperatures value in the proper range
            tempvals = [x+SHIFT_TEMP for x in tempvals]
            # print(tempvals)
            # Compute avarege temperature
            temp_avg = numpy.mean(tempvals)
            # print(temp_avg)
        try:
            humidvals = []
            for humidval in node ['humidvals']:
                if humidval['val'] != 'NaN':
                    humidvals.append(humidval['val'])
                # print(humidval['timestamp'])
                # print(humidval['val'])
        except KeyError:
            print('no humid vals')
        else:
            humidvals = numpy.asarray(humidvals)
            # print(humidvals)
            # Compute avarege humidity
            humid_avg = numpy.mean(humidvals)
            # print(humid_avg)

        # No need to process light values
        # try:
            # for lightval in node ['lightvals']:
                # print(lightval['timestamp'])
                # print(lightval['val'])
        # except KeyError:
            # print('no light vals')
        try:
            soundintensities = []
            soundtimes = [] # array timestamps converted in microseconds
            for soundsample in node ['soundvals']:
                # print(soundsample['timestamp'])
                # print(soundsample['val'])
                if soundsample['val'] != 'NaN':
                    soundintensities.append(soundsample['val'])
                time_usecs = datetimeToSeconds(soundsample['timestamp'])
                soundtimes.append(time_usecs)
        except KeyError:
            rainperc = -1
            print('no sound vals')
        else:
            # Process sound values
            soundintensities = numpy.asarray(soundintensities) # float64 array
            soundtimes = numpy.asarray(soundtimes) # float64 array

            # Compute the number of values higher than the threshold
            number_up_val = 0
            sound_freq = 0
            for x in soundintensities:
                if x>SOUND_THRESHOLD:
                    number_up_val=number_up_val+1
            # Compute time interval
            time_interval = max(soundtimes) - min(soundtimes)
            # Compute frequency of sound values above threshold
            sound_freq = number_up_val/time_interval
            # print(str(number_up_val)+'/'+str(time_interval)+'='+str(frequency))

        # Compute probabilities of rain
        prob_rain = COEFF_SOUND*sound_freq + COEFF_HUMID*humid_avg + COEFF_TEMP*(1/temp_avg)
        # Store the percentage of rain for the current node
        rainpercs[i] = prob_rain * 100 / MAX_PROBAB
        i = i+1

    # Create the new object with the probabilities of rain
    for node, rainperc in zip(rcv_vals, rainpercs):
        node['rainprob'] = rainperc
    # Generate the new string in the buffer
    return_string = json.dumps(rcv_vals)

    return return_string;

class myRequestHandler(BaseHTTPRequestHandler):
    # process GET request
    def do_GET(self):
        try:
	    
            try:
                data = newData()
                if data != '':
                    setBuffer(data)
            except Exception as e:
                print(str(e))

            data = getBuffer()
           
            if data == '':
                raise ValueError('No data available')

            # Send response status code
            self.send_response(200)

            # Send headers
            # self.send_header('Content-type', 'text/html')
            self.send_header('Content-type', 'application/json')
            self._end_headers()

            # Send json string contained in the buffer
            self.wfile.write(bytes(data,'utf-8'))

        except Exception as e:
            print(str(e))
            self.send_response(204)
            self._end_headers()
            self.wfile.write(bytes('Exception: "' + str(e) + '"','utf-8'))

    # process HEAD request
    def do_HEAD(self):
        self.send_response(200)
        self.send_header("Content-type", "text/html")
        self._end_headers()
    
    # with access-control-allow-origin (see https://stackoverflow.com/questions/21956683/enable-access-control-on-simple-http-server)    
    def _end_headers (self):
        self.send_header('Access-Control-Allow-Origin', '*')
        self.end_headers()

if __name__== '__main__':
    httpserver_class = HTTPServer
    myServer = httpserver_class((HOSTNAME,PORT_NUMBER), myRequestHandler)

    try:
        # Initialize the buffer
        setBuffer(newData())
    except:
        pass

    try:
        myServer.serve_forever()
    except KeyboardInterrupt:
        print("Stopping...")
    except Exception as e:
        print('[Data_Processing - HTTP Server] Error: \"'+ str(e) + '\"')

    myServer.server_close()
    print(time.asctime() + ' [Data_Processing - HTTP Server] Server Stopped - ' + str(HOSTNAME) + ':' + str(PORT_NUMBER))

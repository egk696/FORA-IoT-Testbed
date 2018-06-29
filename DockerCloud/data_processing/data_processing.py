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
json_buffer = ''
test_string ='[{"nodeid":"Node1","tempvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}],"humidvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}]},{"nodeid":"Node2","tempvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}],"humidvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}],"soundvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156}, {"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}],"lightvals":[{"timestamp":"2018-06-28T10:44:00.000000Z","val":156.156}, {"timestamp":"2018-06-28T10:44:01.000000Z","val":157.157}]}]'

def setBuffer(value):
    global json_buffer
    json_buffer = value

def getBuffer():
    return json_buffer

def datetimeToMicroseconds(timestamp_string):
#    datetime_val = datetime.datetime.strptime(timestamp_string, "%Y-%m-%dT%H:%M:%S.%fZ")
    datetime_val = dateutil.parser.parse(timestamp_string)
    return datetime_val.timestamp()

def newData():
    return_string = ''
    rainprobs = numpy.zeros(2)
    # rainprobs = [0.0, 0.0]

    # Retrieve json string from data_integration module
    remote_hostname = 'data-integration'
    remote_portnumber = 80
    connection = http.client.HTTPConnection(remote_hostname, remote_portnumber)
    connection.request('GET', '/')
    response = connection.getresponse()

    # if it is not possible to get data return an empty string
    print(response.status)
    if response.status != 200:
        return return_string
    
    json_string = response.read()

    # Extract values from the json string
    # rcv_vals = json.loads(test_string)
    rcv_vals = json.loads(json_string)
    
    print('Received values are listed below.')
    for node in rcv_vals:
        print(node['nodeid']+':')
        try:
            for tempval in node['tempvals']:
                print(tempval['timestamp'])
                print(tempval['val'])
        except KeyError:
            print('no temp vals')
        try:
            for humidval in node ['humidvals']:
                print(humidval['timestamp'])
                print(humidval['val'])
        except KeyError:
            print('no humid vals')
        try:
            for lightval in node ['lightvals']:
                print(lightval['timestamp'])
                print(lightval['val'])
        except KeyError:
            print('no light vals')
        try:
            soundintensities = []
            soundtimes = [] # array timestamps converted in microseconds
            for soundsample in node ['soundvals']:
                print(soundsample['timestamp'])
                print(soundsample['val'])
                soundintensities.append(soundsample['val'])
                time_usecs = datetimeToMicroseconds(soundsample['timestamp'])
                soundtimes.append(time_usecs)
        except KeyError:
            print('no sound vals')
        else:
            # Process sound values
            soundintensities = numpy.asarray(soundintensities) # float64 array
            soundtimes = numpy.asarray(soundtimes) # float64 array

            # print(soundintensities)
            # print(soundintensities.dtype)
            # print(soundtimes)
            # print(soundtimes.dtype)
            # frequencies = numpy.fft.fft2(array_soundvals)
            # frequencies = numpy.absolute(frequencies)
            # print(frequencies)


    # Compare values with the thresholds
    pass
    # Compute probabilities of rain
    pass
    # Create the new object with the probabilities of rain
    for node, rainprob in zip(rcv_vals, rainprobs):
        node['rainprob'] = rainprob
    # Generate the new string in the buffer
    return_string = json.dumps(rcv_vals)

    return return_string;

class myRequestHandler(BaseHTTPRequestHandler):
    # process GET request
    def do_GET(self):
        try:
            if getBuffer() == '':
                raise ValueError('No data available')
            # Send response status code
            self.send_response(200)

            # Send headers
            # self.send_header('Content-type', 'text/html')
            self.send_header('Content-type', 'application/json')
            self.end_headers()

            # # If someone went to "http://localip/foo/bar/",
            # # then self.path equals "/foo/bar/".
            # self.wfile.write('<br><p>You accessed path: '+ self.path+ '</p>')
            # self.wfile.write('<br></body></html>')

            # Send json string contained in the buffer
            self.wfile.write(bytes(getBuffer(),'utf-8'))

            # Empty the json_buffer string
            setBuffer('')

            # Refill the buffer
            setBuffer(newData())
        except Exception as e:
            self.send_response(204)
            self.end_headers()
            self.wfile.write(bytes('Exception: "' + str(e) + '"','utf-8'))

    # process HEAD request
    def do_HEAD(self):
        self.send_response(200)
        self.send_header("Content-type", "text/html")
        self.end_headers()

if __name__== '__main__':
    httpserver_class = HTTPServer
    myServer = httpserver_class((HOSTNAME,PORT_NUMBER), myRequestHandler)

    # Initialize the buffer
    setBuffer(newData())

    try:
        myServer.serve_forever()
    except KeyboardInterrupt:
        print("Stopping...")
    except Exception as e:
        print('[Data_Processing - HTTP Server] Error: \"'+ str(e) + '\"')

    myServer.server_close()
    print(time.asctime() + ' [Data_Processing - HTTP Server] Server Stopped - ' + str(HOSTNAME) + ':' + str(PORT_NUMBER))

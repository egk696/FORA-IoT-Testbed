#!/usr/bin/python
# -*- coding: utf-8 -*-

#  ---------- Data Integration Service ----------
import BaseHTTPServer
import time
import json
import numpy

HOSTNAME='0.0.0.0'
PORT_NUMBER = 8081
json_buffer = ''
test_string ='[{"nodeid":"Node1","tempvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000","val":157.157}],"humidvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000","val":157.157}]},{"nodeid":"Node2","tempvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000","val":157.157}],"humidvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156},{"timestamp":"2018-06-28T10:44:01.000000","val":157.157}],"soundvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156}, {"timestamp":"2018-06-28T10:44:01.000000","val":157.157}],"lightvals":[{"timestamp":"2018-06-28T10:44:00.000000","val":156.156}, {"timestamp":"2018-06-28T10:44:01.000000","val":157.157}]}]'

def setBuffer(value):
    global json_buffer
    json_buffer = value

def getBuffer():
    return json_buffer

def newData():
    return_string = ''
    rainprobs = numpy.zeros(2)
    # Retrieve json string from data_integration module
    pass
    # Extract values from the json string
    rcv_vals = json.loads(test_string)

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
            for soundval in node ['soundvals']:
                print(soundval['timestamp'])
                print(soundval['val'])
        except KeyError:
            print('no sound vals')

    # Process sound values
    pass
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

class myRequestHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    # process GET request
    def do_GET(self):
        try:
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
            self.wfile.write(bytes(getBuffer()).encode('utf-8'))

            # Empty the json_buffer string
            setBuffer('')

            # Refill the buffer
            # setBuffer(newData())
        except Exception as e:
            self.send_response(204)
            self.end_headers()
            self.wfile.write('Exception: "' + str(e) + '"')

    def do_HEAD(self):
        self.send_response(200)
        self.send_header("Content-type", "text/html")
        self.end_headers()

if __name__== '__main__':
    httpserver_class = BaseHTTPServer.HTTPServer
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

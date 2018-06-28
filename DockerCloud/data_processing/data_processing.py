# Data Integration Service
import BaseHTTPServer
import time
import json

HOSTNAME='0.0.0.0'
PORT_NUMBER = 8081

class myRequestHandler(BaseHTTPServer.BaseHTTPRequestHandler):
    # process GET request
    def do_GET(self):
        try:
            self.send_response(200)
            self.send_header('Content-type', 'text/html')
            self.end_headers()
            self.wfile.write('<html><head><title>Title goes here.</title></head>')
            self.wfile.write('<body><p>This is a test.</p>')
            # If someone went to "http://localip/foo/bar/",
            # then self.path equals "/foo/bar/".
            self.wfile.write('<br><p>You accessed path: '+ self.path+ '</p>')
            self.wfile.write('<br></body></html>')
            #TODO return json string containing the probabilities of raining
            # self.send_header('Content-type', 'application/json')
            # myFunc()
        except Exception:
            self.send_response(204)
            self.end_headers()
    def do_HEAD(self):
        self.send_response(200)
        self.send_header("Content-type", "text/html")
        self.end_headers()

if __name__== '__main__':
    httpserver_class = BaseHTTPServer.HTTPServer
    myServer = httpserver_class((HOSTNAME,PORT_NUMBER), myRequestHandler)

    try:
        myServer.serve_forever()
    except KeyboardInterrupt:
        pass
    except Exception as e:
        print('[Data_Processing - HTTP Server] Error: \"'+ str(e) + '\"')

    myServer.server_close()
    print(time.asctime() + ' [Data_Processing - HTTP Server] Server Stops - ' + str(HOSTNAME) + ':' + str(PORT_NUMBER))

def myFunc():
    # Retrieve json string from data_integration module
    pass
    # Extract values from the json string
    pass
    # Process sound values
    pass
    # Compare values with the thresholds
    pass
    # Compute probability of rain
    pass
    return;

# # Import framework
# from flask import Flask
# from flask_restful import Resource, Api
#
# # Instantiate the app
# app = Flask(__name__)
# api = Api(app)
#
# class Product(Resource):
#     def get(self):
#         return {
#             'products': ['Bananas', 'Milk', 'Steak', 'Cereal']
#         }
#
# # Create routes
# api.add_resource(Product, '/')
#
# # Run the application
# if __name__ == '__main__':
#     app.run(host='0.0.0.0', port=80)

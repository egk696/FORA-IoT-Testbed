# Data Integration Service

from http.server import BaseHTTPRequestHandler, HTTPServer
import json
from collections import deque
import time

queue = deque('',1000)

class S(BaseHTTPRequestHandler):		

	def do_GET(self):
		try:
			val = queue.popleft()
			post_body = json.dumps(val)
			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(bytes(post_body, 'utf-8'))
		except IndexError:
			self.send_response(204)
			self.end_headers()

	def do_POST(self):
		content_len = int(self.headers['content-length'])
		post_body = self.rfile.read(content_len)
		val = json.loads(post_body)
		queue.append(val)
		self.send_response(200)
		self.end_headers()

	def do_HEAD(self):
		self.send_response(200)
		self.end_headers()
		
hostname = '0.0.0.0'
port = 80
myServer = HTTPServer((hostname, port), S)

try:
    myServer.serve_forever()
except KeyboardInterrupt:
    pass

myServer.server_close()
			

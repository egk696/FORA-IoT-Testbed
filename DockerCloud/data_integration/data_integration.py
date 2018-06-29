# Data Integration Service
from http.server import BaseHTTPRequestHandler, HTTPServer
import json
import threading
from collections import deque

class S(BaseHTTPRequestHandler):		

	lock = threading.Lock()
	sets = {}

	def do_GET(self):
		with S.lock:
			_sets = S.sets
			S.sets = {}
		
		if not _sets:
			self.send_response(204)
			self.end_headers()
		else:
			for set in _sets.values():
				for key, value in set.items():
					if type(value) is deque:
						set[key] = list(value)
			post_body = json.dumps(_sets)
			self.send_response(200)
			self.send_header('Content-type', 'application/json')
			self.end_headers()
			self.wfile.write(bytes(post_body, 'utf-8'))

	def do_POST(self):
		try:
			content_len = int(self.headers['content-length'])
			post_body = self.rfile.read(content_len)
			val = json.loads(post_body)
			nodeid = val['nodeid']
			
			with S.lock:
				try:
					set = S.sets[nodeid]
				except KeyError:
					set = {}
					S.sets[nodeid] = set
				
				for key, value in val.items():
					if type(value) is list:
						if key not in set:
							set[key] = deque('',1000)
						set[key].extend(value)
					else:
						set[key] = value
			
			self.send_response(200)
			self.end_headers()
		except:
			self.send_response(500)
			self.end_headers()
			raise

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
			

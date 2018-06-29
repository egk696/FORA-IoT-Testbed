import SimpleHTTPServer
import SocketServer

def run(port=80):
    Handler = SimpleHTTPServer.SimpleHTTPRequestHandler
    httpd = SocketServer.TCPServer(("0.0.0.0", port), Handler)
    print ("serving at port", port)
    httpd.serve_forever()

def get_bytes_from_file(filename):  
    return open(filename, "rb").read() 

if __name__ == "__main__":
    from sys import argv

    if len(argv) == 2:
        run(port=int(argv[1]))
    else:
        run()


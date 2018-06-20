# Multi-threaded Rasberry Pi Sensor Node

## Features
- Five periodic tasks with adjustable rates. Four sensor tasks and one post-processing task. 
- Tasks use mutex to share sensor access.
- HTTP server for remote sensing
- Uses GrovePi+ to read sensors

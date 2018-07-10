# Multi-threaded Rasberry Pi Sensor Node

## Features
- Three periodic sensor tasks with adjustable rates.
- Tasks use mutex to share sensor access.
- HTTP server POST to cloud server
- Uses GrovePi+ to read sensors

## Test and Run
1) Quickly run with the default values by executing:

        $ ./startup.sh

2) Start by executing directly the python script:

        $ python3 SensorNode.py <nodeName> <serverURL> <dhtPeriod> <lightPeriod> <soundPeriod>

    where:
    * nodeName: is a string for the node name
    * serverURL: is the cloud server URL destination for posting the collected values.
    * dthPeriod: is the period of the task reading the temperature/humidity sensor.
    * lightPeriod: is the period of the task reading the light sensor.
    * soundPeriod: is the period of the task reading the sound sensor.

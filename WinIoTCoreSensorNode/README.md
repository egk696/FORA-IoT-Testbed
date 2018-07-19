# Multi-threaded Rasberry Pi Sensor Node

## Features
- Three periodic sensor tasks with adjustable rates.
- Tasks use mutex to share sensor access.
- HTTP server POST to cloud server
- Uses GrovePi+ to read sensors

## Requirements
Raspberry Pi with [Windows IOT](https://docs.microsoft.com/en-us/windows/iot-core/tutorials/quickstarter/devicesetup).

GrovePi+ for the Raspberry with sound, humidity, temperature sensors.

[Visual Studio](https://visualstudio.microsoft.com/) (Tested using Community 2017)

## Running
1) Open the solution file in Visual Studio.

2) In the solution properties, under debug, specify the ip address of the Raspberry.

3) Run or Debug the project. This will download and compile any dependencies, as well as install the application on the Raspberry.

4) The node should start reading the sensors and send the values to the cloud at 192.168.0.100:82. If the cloud is at another location, the node can be setup by attaching a monitor, keyboard and mouse.
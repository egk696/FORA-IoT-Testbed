# FORA SC3 - Sensor Data Integration
This repository contains our FORA SC3 project. The project consists of a cloud part, located in [DockerCloud](DockerCloud) and a sensor node part, located in [RaspbianSensorNode](RaspbianSensorNode) and [WinIoTCoreSensorNode](WinIoTCoreSensorNode). 

The sensor nodes measure values using connected sensors and transmit these values to the cloud, where data from all nodes are collected, processed and visualized.

The two sensor node types behave similarly and merely showcase how the solution can work with both a Raspbian/python platform and a Windows IoT/.NET platform. Any combination of the two can be used in the setup.

Descriptions on how to setup the cloud and sensor nodes are located in the respective folders.

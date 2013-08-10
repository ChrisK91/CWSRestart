---
layout: manual
title: MITM configuration file
manual: mitm
nav: Configuration file
---
## Configuration file
The CubeWorldMITM.exe.config file contains the following settings:
- *MinLevel* the minimum level allowed to join the server
- *MaxLevel* the maximum level allowed to join the server
- *PlayerLimit* the maximum number of allowed players. If this number is reached, new connections will be dropped.
- *StartServer* indicates if CubeWorldMITM should start the server on its own. Starting the server through CubeWorldMITM will run it through the configurators. (*currently only available if you compile CubeWorldMITM yourself*)
- *ServerLocation* contains the location of the server. (*currently only available if you compile CubeWorldMITM yourself*)
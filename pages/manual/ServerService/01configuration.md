---
layout: manual
title: ServerService configuration file
manual: serverservice
nav: Configuration file
---
## Configuration file
The ServerService.dll.config file contains the following settings:

- *ServerProcessName* contains the process name of the server.
- *DoNotRedirectOutput* indicates, if the output of the server should be redirected through the program. If set to *false*, it will appear on the console.
- *SessionActiveDefault* indicates if CWSRestart is started on an active machine. Set it to *false* if you are starting the program on a locked PC/server.
- *BypassSendQuit* indicates if the program should try to stop the server peacfully. If this is set to *true* no key will be sent and the server will be terminated immediately.
- *CheckLoopback* defines if access should be checked from the loopback network.
- *CheckInternet* defines if access should be checked from the internet.
- *CheckLAN* defines if access should be checked from the local network.
- *IPService* contains the link to a service, that can be used to retreive the IP address.
- *Loopback* the loopback IP. Usually *127.0.0.1*
- *Internet* the global IP.
- *LAN* the local network IP.
- *Port* contains the port on wich access should be checked.
- *Timeout* is the time that is given the server to exit (in milliseconds).
- *ServerPath* contains the location of the server.
- *StatisticsInterval* is the intervall between to statistic updates (in milliseconds).
- *SaveStatisticsEvery* contains the number after wich statistics should be saved. Every *x* statistics updates, statistics are saved.
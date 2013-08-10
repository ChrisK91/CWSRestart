---
layout: manual
title: Changing the port of the default server
manual: mitm
nav: Changing the server.exe port
---
## Changing the server.exe port
To use the MITM server, you need to edit the server.exe to listen on port `12346`. You can do this by opening up the server.exe in the hexeditor of your choice, for instance in [HxD](http://mh-nexus.de/en/hxd/). Now you need to find the integer number `12345`. In hex, just look for `39 30 00 00`. Change this to the number `12345`, so you end up with `3A 30 00 00`.

To verify that the change was succesful, run `netstat -a -b -n -p TCP` when you have started your modified server. Now you can fire up the MITM server. The MITM server will accept traffic from port `12345`, so you dont need to edit your client.
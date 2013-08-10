---
layout: post
author: chris
title: The future of MITM
---
The MITM was pretty much a tool on its own until now. So let's discuss the future plans of the MITM server. The main goal was to identify CubeWorld player names. And currently I think it does that pretty well and pretty stable. As a byproduct of the name resolving, we also retreive the level so you can restrict server access to players within a certain level. I don't really go much further command wise. Theoretically the server can parse any package - we could even manipulate the players position, health,... - but I don't think that this functionality is needed by the MITM users.

I want to work on the basics first: launching the server and integrating it into CWSRestart and CWSWeb. Launching already works in the current development builds. You need to specify the server location and MITM will start the server. In addition, I also added configurators. Configurators are small plugins, that are called when the server has a certain MD5. They can then manipulate the server and return a path to the manipulated server to MITM.

Here is an example: CubeWorldMITM comes with an embedded configurator that changes the server port. If the MD5 of the server.exe matches that of the default server, the configurator will create a copy of the server.exe and patch that copy to another port. MITM will then continue with the patched server. In short: the next version will patch the port automatically :)

The next thing on my to do list is communication with CWSRestart and CWSWeb, so that you actually know which player you are kicking. I currently see two possibilities: add all the known players into a SQLite database and query that database from CWSRestart and CWSWeb or store them in a list in memory. At the moment I think that the SQLite database might be better. This way, we don't need to keep thousands of playernames in your RAM (not that they would use too many but I don't think thats practicably). The SQLite database would also elimiate the need for update messages, when a player is identified. CWSRestart and CWSWeb will query the database and get the playername (if the player is known).

I plan to push out the next version when player identification is in. I hope that this won't take too long. If everything goes as planned it should be ready the coming week.
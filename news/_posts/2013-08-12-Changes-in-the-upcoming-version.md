---
layout: post
author: chris
title: Changes in the upcoming version
---
If you keep an eye on my GitHub repo, you'll have noticed that CWSRestart and CWSWeb, that I just finished the basic player identification for CWSWeb and CWSRestart. This was the next big feature for the project, that I wanted to get out of the door. Now the MITM server is finally integrated into CWSRestart and CWSWeb.

When you have MITM running, you can chose to enable player identification. MITM will then register with CWSRestart and write all player names into a database that can be accessed by CWSRestart and CWSWeb. CWSRestart will display the names in the IP Filter dialog (which is resizable in the next version), so you know which player you are banning. CWSWeb will also display names next to the online list.

There are some other minor improvements as well. There has been some [restructuring with the settings](http://chrisk91.github.io/CWSRestart/news/2013/08/10/Cleaning-up-the-settings.html). Another new feature are userbars:

![Userbar]({{ site.baseurl }}img/manual/userbar.png)

Now you can show your server status everywhere. CWSRestart will also save your additional processes in the upcoming version. If you are hosting on multiple machines, you can configure CWSRestart with [preset](http://chrisk91.github.io/CWSRestart/news/2013/08/11/Presets.html).

So stay tuned for the release. I still have some testing to do, but everything should be ready tomorrow if I don't come across major bugs.

Here are the commit messages since the last version, in case you are even more interested:

    - Players can now be identified in CWSWeb
    - Modules can now notify CWSRestart, that they have are ready to identify players. Otherwise CWSRestart won't even use the database. This should improve performance when player identification is disabled
    - CWSRestart will try to get the current name (under which the player is online) first. If the player is not online, a list of known names will be displayed
    - Made IP Access dialog resizable
    - Basic player identification in CWSRestart
		Not pretty and still full of errors, but a start on #5
		Still needs some work, though...
    - Added userbars
		Userbars are created by CWSWeb. You can modify the base.png with a
		custom server banner.
    - restructuring, auto preset loading setting
    - Autosaving & restoring of additional processes
    - Added ability to send a preset from MITM to CWSRestart.
    - Added ability to load XML presets
    - Abstracted settings saving/loading into own class
    - Added ability to launch the server through the MITM server
    - Added configurators, that can automatically configure the server.exe
		-> Added configurator to patch the server port to 12346
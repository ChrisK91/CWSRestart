---
layout: post
author: chris
title: Changes in the upcoming version
---
Yesterday I uploaded an alpha built of the upcoming version 0.9.0. You can find it [over at GitHub](https://github.com/ChrisK91/CWSRestart/releases/tag/v0.9.0-alpha), in case you're interested.

The next version focuses more on the ascpects of the MITM server into the other tools. For instance, you'll be able to set a number of premium slots. Premium players can then login via CWSWeb and join your server, even if all regular slots are occupied.

You can also let MITM enforce your blacklist and whitelist. If you decide to do so, CWSRestart doesn't require administrative privileges anymore (although kicking still runs through CWSRestart currently).

Here is a more detailed changelog, if you are interested:

    - Performance improvements in the IP Filter list
    - Added ability to specify min and max HP
    - Code documentation and refractoring, to increase readbility
    - Rewrote the logging system for MITM
        You can specify the logging level in the config file. An additional log is saved to disc.
    - Added premium slots and authentication for premium players
    - The CWSWeb index page is now using the layout.cshtml
    - The statistics folder is now saved and restored when restarting CWSRestart.
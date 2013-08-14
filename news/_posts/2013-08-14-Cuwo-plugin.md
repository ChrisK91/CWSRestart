---
layout: post
author: chris
title: The cuwo proof of concept
---
After yesterdays post about custom servers, I continued to work on the plugin for the [cuwo](https://github.com/matpow2/cuwo) server. The server itself is written in Python, which makes it not so trivial to use my existing .net library.

I could write a implementation of CWSProtocol (which is used to speak with CWSRestart) in Python, but that would mean that I have to update more code if I make changes. And since I'm currently the only person commiting to the project, I would like to prevent that. Another goal I had was to prevent the need to compile cuwo yourself. Since the precompiled cuwo binaries come with <i>ctypes</i> which allows the import of unmanaged DLLs, I decided to create a library in C++/CLI. The downside of that is, that this plugin is Windows only (sorry about that :( ). Anyway, here is a small screenshot of the progress. The plugin itself seems stable, the memory usage with one player online is around 15MB.

If you want to check out or contribute to the project, head over to (https://github.com/ChrisK91/CuwoCWS)[https://github.com/ChrisK91/CuwoCWS]. And in case you are more of an image person, here is a screenshot of the current state:

![cuwo cwsrestart plugin]({{ site.baseurl }}img/news/cuwoplugin.png)
---
layout: post
author: chris
title: A look at custom servers
---
Most servers that I visit seem to run the server.exe that comes with CubeWorld. However, if you look around, more and more servers with custom software pop up. The first server that I came across is [Cuwo](https://github.com/matpow2/cuwo). But there are more. [Coob](https://github.com/amPerl/Coob/), [Glydar](https://github.com/Glydar/Glydar) and a [Ghrum implementation](https://github.com/Ghrum/CwVanilla).

CWSRestart should (at least in theory) work with every one of those servers, since it is a independant tool. It doesnt't modify the RAM or inject DLLs. CubeWorldMITM should work as well, since the protocol should be the same across every server. CWSWeb only communicates with CWSRestart, so it should run without problems. But what about statistics and player identification?

Most of these servers expose some sort of plugin architecture. So in theory, it should be possible to create CWSRestart plugins that enable communication between the custom server and CWSRestart. Writing a plugin for Coob shouldn't be an issue, since Coob is written in C# as well. Cuwo and Ghrum should both be able to load unmanaged DLLs. I would really like to reuse the code I've already written in C#, this way I only have to change stuff in one file. With [C++/CLI](http://en.wikipedia.org/wiki/C%2B%2B/CLI), I might be able to create a DLL that connects the existing code to the custom server. I already got a small proof of concept working, that connects Cuwo with CWSProtocol to CWSRestart. But my C++/CLI skills are pretty bad, and my Python skills almost non-existant.

![Cuwo CWSRestart connection]({{ site.baseurl }}img/news/cuwo_proof_of_concept.png)

I can't really promise anything because I'm not really familiar with Python-to-C++-to-C# code, but I'll look into adding support fort custom servers. And by the way: the current *proof of concept* doesn't even require you to compile Cuwo for yourself.

Also, I should note that I wan't to implement some additional CWSProtocol features next, that should increase the usability of custom servers and MITM. I want to introduce kicking and banning as an action, that custom servers can register for. They will then receive the blacklist/whitelist and enforce that list by themself. What does that mean for you? If you are running CubeWorldMITM, chances are that you don't need to run CWSRestart as administrator in one of the upcoming versions.
---
layout: post
author: chris
title: Premium slots
---
There is something I forgot to add into the recent [development update]({{ site.baseurl }}news/2013/08/19/Whats-next.html): I'm currently working on premium slots. Premium slots are a limited number of slots, that is reserved to a special set of players. These players will need to authenticate and can then join your server, even if the maximum number of players is reached.

Since CubeWorld doesn't have a global account authentication, everyone can rename his character. This prevents us from identifying users with their character names. As a workaround, premium users will need to authenticate in CWSWeb. Their IP is then entered into a special table, and MITM can check on new connections, if a player has premium access. In MITM, you'll need to specify a number of premium slots. These slots add up to the number of maximum users. If you want a premium only server, you can set the maximum number of players to 0. This way you can use the premium system as a whitelist replacement for users with dynamic IPs.

The system itself is currently in a very early stage and not functional. But if everything works well, it should be ready at the end of the week.

On a sidenote: I finished the logging overhaul in MITM. It can now log to your file system as well. I'll explain the new system in an upcoming post.
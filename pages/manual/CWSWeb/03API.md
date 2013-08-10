---
layout: manual
title: CWSWeb JSON API
manual: cwsweb
nav: JSON API
---
## API
If you want to use the data from CWSWeb on your webpage, you can use a small API:
- `/API/stats` will return all available info, such as runtime and if the server is running
- `/API/stats/serverrestarts` will return timestamps, indicating when the server has been restarted

The following urls are restricted to logged in users:

- `/API/stats/memoryusage` will return the memory usage of the server

You'll get the objects serialized as JSON. Timestamps are formated a bit odd. You can parse them to JavaScript dates with `new Date(parseInt(`*`timestamp`*`.substr(6)))`

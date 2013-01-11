AirPlayer
=========

## A Mono based library to access your AppleTV using the AirPlay protocol

### Introduction
This is a set of simple classes that implement the [AirPlay protocol](http://nto.github.com/AirPlay.html), 
allowing you to send pictures and movies to your AppleTV. It was inspired by the code that was published by Sebastian Pouliot 
on http://github.com/spouliot/airplay and described on [his blog](http://spouliot.wordpress.com/2012/12/10/airplay-vs-large-digital-frame). 

It relies heavily on the async keyword and therefore needs either MS.NET 4.5 or Mono 3.x (currently Mono JIT compiler version 3.0.2).
The code is completely Mono based and has no dependencies on iOS classes like NSNetService for dns-sd.
It should work on Windows and .NET but was so far only tested with MonoDevelop on Max OSX.

### Copyright
There is no copyright. Use it as you like, change it, sell it, whatever. I enjoyed making it (so far) and that will do for me.

### Dependencies
* [Mono.ZeroConf](http://github.com/mono/Mono.Zeroconf) library
* Mono 3.0.2
* [PlistCS](https://github.com/animetrics/PlistCS/blob/master/PlistCS/Src/Plist.cs)

I haven't found a good way yet too include the Mono.ZeroConf code. Since it is on [GitHub] 
you can get it there. I didn't perform a real good install (that will set the config file that I include in the solution) but
found a good example file on StackOverflow.
The PlistCS code is just one class that I simply copy/pasted.

### The future
I have no intention to make a full implementation of the protocol. Especially the audio parts look pretty hard to do. I do want 
the ability to play standard slideshows and video (where the AppleTV downloads an MP4 from my computer). Mirroring would be 
nice but requires streaming of video and I have no idea how to do that.

### Third version
Right now I'm working on sending an MP4 using Progressive Download. I have some progress, but am looking for a valuable http-server 
to embed in my code. Currently trying [Griffin.WebServer](https://github.com/jgauffin/griffin.webserver).

### Second version
EPIC FAIL. AppleTV keeps returng "400 BAD REQUEST".
I'm working on the slideshows. This could be done by sending photo-by-photo and specifying a transition, but it can also be done 
by starting a slideshow session whereby the AppleTV requests a photo from the client. That's done using reverse-http and means 
your client has to function as a http-server. It would be nice if I could implement reverse-http with SignalR, but I haven't 
found that out yet.


### First version
This version detects the AppleTV and returns its IP-address. It allows you to send a photo or a series of photos and it 
can query the AppleTV for all the available transitions between photos.




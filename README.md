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

### First version
This version detects the AppleTV and returns its IP-address. It allows you to send a photo or a series of photos and it 
can query the AppleTV for all the available transitions between photos.




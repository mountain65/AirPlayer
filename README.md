AirPlayer
=========

## A Mono based library to access your AppleTV using the AirPlay protocol

### Introduction
This is a set of simple classes that implement the [AirPlay protocol](http://nto.github.com/AirPlay.html), 
allowing you to send pictures and movies to your AppleTV. It was inspired by the code that was published by Sebastian Pouliot 
on http://github.com/spouliot/airplay and described on [his blog](http://spouliot.wordpress.com/2012/12/10/airplay-vs-large-digital-frame). 

It is completely Mono based and has no dependencies on iOS classes like NSNetService for dns-sd.

### Dependencies
This code depends on the Mono.ZeroConfig library

### First version
This version detects the AppleTV and returns its IP-address. It allows you to send a photo or a series of photos and it 
can query the AppleTV for all the available transitions between photos.




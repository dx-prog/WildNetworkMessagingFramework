# WildNetworkMessagingFramework


## Description
A framework to orchestrate the transmission of data across various end points, configurable through IoC.


## Project Vision

Disclaimer: See "Current Status"

The vision of a complete version of the framework is one that can 

* Handle multiple input and output protocols (File Share, TCP, FTP, HTTP, UDP, etc.)
* Has built-in retry logic and journaling to make sure all data is in sync.
* Can be used for chat, file sync, or any other system that needs orchestration.

## Current Status

## As of 10/1/2017 Proof of concept
Add a basic test case to show that the framework works in principle. The test
shows the ability to create a file, and have the file move through the orchestration.
The test only covers basic file copy operations, and no other protocol.

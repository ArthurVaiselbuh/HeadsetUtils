# HeadsetUtils
Some wireless headsets are set as the default audio device as long as the dongle is connected, even if the headset is off.

This utility is meant to change the system's default audio device when the headset is turned off/on.

Current version supports iCue 4 and iCue 5 and installing [AudioDeviceCmdlets](https://github.com/frgnca/AudioDeviceCmdlets) powershell module

By default, the utility will register itself to run at windows startup. This and other configurations(like headset name) can be changed by modifying `HeadsetUtils.dll.config`.

Disclaimer - this software has no association to `Corsair Gaming, Inc` , is provided as-is without any warranty. See license for details.

# How to setup?

## iCUE 5
1. Install [AudioDeviceCmdlets](https://github.com/frgnca/AudioDeviceCmdlets) powershell module as instructed in the repository (Install to the windows's default powershell otherwise HeadsetUtils won't work!)
2. Edit the config to look something like: 
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="MonitoringIntervalMs" value="1000"/>
		<add key="HeadsetName" value="Headphones"/> //Use your headphone's name
		<add key="SpeakersName" value="Speakers"/> //if you have more than one device named "Speakers"(for example if you use steam remote play you might heve a device named "Steam Streaming Speakers", which will too show up in the command output), then use the full name of the device as reported by get-audiodevice -list | where Type -eq 'Playback' in powershell
		<add key="Autorun" value="true"/>
		<add key="ConnectedRegexOverride" value="\d{4}-\d\d-\d\d\s(?:\d\d:?)+\.\d+\sI\scue\.dev\.connection_controller:\s"\w+"\sDisconnected\s->\s(?:Wireless)|(?:Direct)"/> //this is a "device connected" regex override for iCUE5
		<add key="DisconnectedRegexOverride" value="\d{4}-\d\d-\d\d\s(?:\d\d:?)+\.\d+\sI\scue\.dev\.connection_controller:\s"\w+"\s(?:Wireless)|(?:Direct)\s->\sDisconnected"/> //this is a "device disconnected" regex override for iCUE5
	</appSettings>
</configuration>
```

## iCUE 4
1. Install [AudioDeviceCmdlets](https://github.com/frgnca/AudioDeviceCmdlets) powershell module as instructed in the repository (Install to the windows's default powershell otherwise HeadsetUtils won't work!)
2. Edit the config to look something like: 
```xml
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<add key="MonitoringIntervalMs" value="1000"/>
		<add key="HeadsetName" value="Headphones"/> //Use your headphone's name
		<add key="SpeakersName" value="Speakers"/> //if you have more than one device named "Speakers"(for example if you use steam remote play you might heve a device named "Steam Streaming Speakers", which will too show up in the command output), then use the full name of the desired device as reported by get-audiodevice -list | where Type -eq 'Playback' in powershell
	</appSettings>
</configuration>
```

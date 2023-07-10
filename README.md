# HeadsetUtils
Some wireless headsets are set as the default audio device as long as the dongle is connected, even if the headset is off.

This utility is meant to change the system's default audio device when the headset is turned off/on.

Current version supports iCue 4 and iCue 5 and installing [AudioDeviceCmdlets](https://github.com/frgnca/AudioDeviceCmdlets) powershell module

By default, the utility will register itself to run at windows startup. This and other configurations(like headset name) can be changed by modifying `HeadsetUtils.dll.config`.

Disclaimer - this software has no association to `Corsair Gaming, Inc` , is provided as-is without any warranty. See license for details.

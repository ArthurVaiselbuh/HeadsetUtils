# HeadsetUtils
Some wireless headsets are set as the default audio device as long as the dongle is connected, even if the headset is off.

This utility is meant to change the system's default audio device when the headset is turned off/on.

Current version requires iCue 4 and installing [AudioDeviceCmdlets](https://github.com/frgnca/AudioDeviceCmdlets) powershell module

By default, the utility will register itself to run at windows startup. This and other configurations(like headset name) can be changed by modifying `App.Config` and `HeadsetUtils.dll.config`.

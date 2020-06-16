# Local Deployment to WearOs device

1. Enable dev mode (standard android procedure)
2. Connect device to the same WiFi network as your dev machine
3. In developer options switch on `ADB Debugging`, `Debug over Wi-Fi` and note IPv4 address
4. in terminal run `adb connect <IP>`and approve connection on the device
5. Now device should appear in AVD Manager and you can deploy as usual
 
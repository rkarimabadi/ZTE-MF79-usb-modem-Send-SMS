# ZTE-MF79-usb-modem-Send-SMS
A Simple Code with c# for sending text messages via ZTE MF79U usb modem
The modem does not provide a standard Api for controlling and sending text messages. But it has a web interface for  management & control that can be used to send text messages.
## Config
You must set two parameters in App.config
- ModemIP: is your modem's IP, default is 192.168.0.1
- Password: is you Web login pass word, don't convert password to base64 encode or ...
## How the program works
There are at least three steps to send a text message:
1. Login:
Since the modem does not have, you have to simulate the steps of sending SMS via the web. First we log in and save the cookie
```
Method: POST

curl -s --header "Referer: http://<modem_ip>/index.html" -d 'isTest=false&goformId=LOGIN&password=<Password>' http://<modem_ip>/goform/goform_set_cmd_process


if is OK {"result":"3"}
if is BAD {"result":"1"}
```
at this step uri is **http://<modem_ip>/goform/goform_set_cmd_process** , and as you see: 
  | parameter | value |
|------------|------------|
|   isTest    | false     |
|   goformId       |  LOGIN          |
|	  password       | <Password>  base64 encoded         | 

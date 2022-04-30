# ZTE MF79 usb modem Send SMS
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
We save a cookie named "stok". However, this is done automatically by **cookieContainer**.
2. Second step is Sending SMS.
```
Method: POST

curl -s --header "Referer: http://<modem_ip>/index.html" -d "isTest=false&goformId=SEND_SMS&notCallback=true&Number=<phone_number>&sms_time=<date>&MessageBody=<message>&ID=-1&encode_type=UNICODE&AD=<hashcoded>"
http://<modem_ip>/goform/goform_set_cmd_process 

if is OK {"result":"success"}
```
at this step uri is **http://<modem_ip>/goform/goform_set_cmd_process** , and as you see: 
| parameter | value |
|------------|------------|
|   isTest    | false     |
|   goformId       |  SEND_SMS          |
|	  notCallback       | true         | 
|	  Number       | <phone_number> urlencoded        |
|	  sms_time       | DateTime.UtcNow.ToString("y;m;d;H;i;s;+4.5")         |
|	  MessageBody       | <message> hexencoded,  used a library foe encoding text to gsm encode       |
|	  AD       |  varification code         |   
### How to Calculate AD
to calculate AD, we must obtain   wa_inner_version and cr_version of modem.  wa_inner_version is like "BD_PLKPLMF971R1V1.0.0B06" and cr_version usually is empty. you can find this two parameters in web interface and encode, but you code send a request to modem and retrive these parameters.
```
Method: GET

curl -s --header "Referer: http://<modem_ip>/index.html" -d 'isTest=false&cmd=Language,cr_version,wa_inner_version&multi_data=1' http://<modem_ip>/goform/goform_get_cmd_process

```  
at this step uri is **http://<modem_ip>/goform/goform_get_cmd_process** , and as you see: 
| parameter | value |
|------------|------------|
|   isTest    | false     |
|   cmd       |  Language,cr_version,wa_inner_version          |
  |   multi_data       |  1         |
and there is no need to attache cookie to your request at this step.  

the concat   wa_inner_version and cr_version and hash them with md5, named md5_rd
the get RD feom modem and name it info_rd:  
```
Method: GET

curl -s --header "Referer: http://<modem_ip>/index.html" -d 'isTest=false&goformId=RD' http://<modem_ip>/goform/goform_get_cmd_process

```  
at this step uri is **http://<modem_ip>/goform/goform_get_cmd_process** , and as you see: 
| parameter | value |
|------------|------------|
|   isTest    | false     |
|   goformId       |  RD          |
  you should attache cookie to your request at this step.
  
  concat md5_rd and info_rd and hash this string, this makes AD
  
3.Log out
  Log out is not nessesary, but modem has a protection mechanism, and mybe block you access to modem for 1 minute, and it is good to log out and the log in time to time.
```
Method: POST

curl -s --header "Referer: http://<modem_ip>/index.html" -d 'isTest=false&goformId=LOGOFF&AD=<hashcoded>' http://<modem_ip>/goform/goform_set_cmd_process


if is OK {"result":"sucess"}
```  
you need AD at this stem, "stok" cookie alse provided to requet internally.
  
### Special thanks to:

[https://github.com/paulo-correia/ZTE_API_and_Hack](https://github.com/paulo-correia/ZTE_API_and_Hack) - for list of Requests and PHP Class
  
[https://github.com/aghazadehm](https://github.com/paulo-correia/ZTE_API_and_Hack) - for  MD5 hash and UTF8 Encoding
  
[https://stackoverflow.com/a/25155746/3951494](https://github.com/paulo-correia/ZTE_API_and_Hack) - for  encode unicode string in "UCS2" format  

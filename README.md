# RawPrint

RAW Print allows sending RAW data directly to local printers through http://localhost:9100 (configurable).  This is useful for label printing applications.

RAW Print functions as a Windows Service. You can try it out [here](https://trehoffman.github.io/RawPrint).

I was inspired to write this by programs like [KwickPython](https://kwickpos.com/opensource/index.php) and [Zebra Browser Print](https://www.zebra.com/us/en/products/software/barcode-printers/link-os/browser-print.html).

## Quick Start Instructions

Get list of available printers (GET):
```
http://localhost:9100
```

Send data to printer (GET):
```
http://localhost:9100/?printer=printer1&data=rawdata
```

Send data to printer (POST):
```
endpoint: http://localhost:9100/

body:
{
  "printer": "printer1",
  "data": "rawdata"
}
```

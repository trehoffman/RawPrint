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

## Configuration Instructions

1.  Go to folder of RAW Print install.  Might be:

  ```
  C:\Program Files (x86)\TreHoffman Technologies\RAW Print
  ```

2.  Edit key values in RawPrint.exe.config file:

  ```
  <?xml version="1.0" encoding="utf-8" ?>
  <configuration>
      <startup> 
          <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
      </startup>
    <appSettings>
      <add key="Base Address" value="http://127.0.0.1"/>
      <add key="Port" value="9100"/>
    </appSettings>
  </configuration>
  ```

3.  Restart RAW Print Windows Service via "Services" GUI or run the following commands in a Windows Terminal:

  ```
  net stop "RAW Print"
  net start "RAW Print"
  ```

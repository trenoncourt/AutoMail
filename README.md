# Automail &middot; [![NuGet](https://img.shields.io/nuget/vpre/Automail.AspNetCore.svg?style=flat-square)](https://www.nuget.org/packages/Automail.AspNetCore) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](http://makeapullrequest.com) [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](https://github.com/trenoncourt/AutoMail/blob/master/LICENSE) [![Donate](	https://img.shields.io/beerpay/hashdog/scrapfy-chrome-extension.svg?style=flat-square)](https://www.paypal.me/trenoncourt/5)
> A tiny .net core microservice to send mails ready in 30 seconds.

AutoMail is a tiny API to send mails based on the [MailKit](https://github.com/jstedfast/MailKit) library.

## Installation
Choose between middleware or api

### Middleware
```Powershell
Install-Package Automail.AspNetCore
```

### Api
Download the [last release](https://github.com/trenoncourt/AutoMail/releases), drop it to your server and that's it!

## Configuration
All the configuration can be made in environment variable or appsettings.json file

```json
"Automail": {
  "Providers": [
    {
      "Smtp": {
        "Host": "smtp.abc.com", // Required
        "Port": "587", // Required
        "User": "trenoncourt@abc.com",
        "Password": "Secret",
        "Security": "Tls"
      }
    },
    {
      "Path": "xyz", // Can be ommited for 1 endpoint, empty per default
      "Smtp": {
        "Host": "smtp.xyz.com",
        "Port": "587",
        "User": "trenoncourt@xyz.com",
        "Password": "Secret",
        "Security": "Tls",
        "LocalDomain": "HELO"
      }
    }
  ]
}
```

## Api configuration
### Cors
```json
"Cors": {
  "Enabled": true, // default is false
  "Methods": "...", // default is *
  "Origins": "...", // default is *
  "Headers": "...", // default is *
}
```

### Host
Currently only chose to use IIS integration
```json
"Host": {
  "UseIis": true
}
```

### Kestrel
Kestrel options, see [ms doc](https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.aspnetcore.server.kestrel.core.kestrelserveroptions) for usage
```json
"Kestrel": {
  "AddServerHeader": false
}
```

### Serilog
storfiler use Serilog for logging, see [serilog-settings-configuration](https://github.com/serilog/serilog-settings-configuration) for usage
```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Debug",
    "Override": {
      "Microsoft": "Warning",
      "System": "Warning"
    }
  },
  "WriteTo": [
    { "Name": "Console" }
  ],
  "Enrich": ["FromLogContext"]
}
```

## API
### POST /send
Sends an email.

**JSON Body**
```json
{
  "from": "trenoncourt@abc.fr", // can be omitted if user is in settings
  "fromName": "thibaut renoncourt",
  "to": "trenoncourt@xyz.com", // required
  "subject": "your subject",
  "body": "your body",
  "cc": "cc@abc.fr",
  "isHtml": true
}
```

**FORM-DATA Body**

If you want to pass attachments

files: [attachments]

data: same as json body

*Note: multiple adresses can be passed with ';' separator*

## Auth
Soon...

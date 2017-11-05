# AutoMail
AutoMail is a tiny REST API to send mails based on the [MailKit](https://github.com/jstedfast/MailKit) library.

## Installation
Download the [last release](https://github.com/trenoncourt/AutoMail/releases) and place it on your server.

You can download version **1.1.0** [here](https://github.com/trenoncourt/AutoMail/releases/download/1.1.0/AutoMail-1.1.0.zip)

AutoMail is designed to be as light as possible so it does not contains the *Microsoft.AspNetCore.Server.IISIntegration* package.
If you want to use it on **IIS**, download the **iis-1.1.0** version from [here](https://github.com/trenoncourt/AutoMail/releases/download/1.1.0/AutoMail-iis-1.1.0.zip)

## Configuration
All the configuration can be made in environment variable or appsettings.json file :

| Name              | Description                                   | Type        | Default value |
| ----------------- | --------------------------------------------- | ----------- |--------------:|
| **Cors**          | Cors settings                                 | Object      |               |
| Cors.Enabled      | Define if cors are enabled                    | Boolean     | false         |
| Cors.Methods      | Adds specific methods to the policy           | String      | *             |
| Cors.Origins      | Adds specific origins to the policy           | String      | *             |
| Cors.Headers      | Adds specific headers to the policy           | String      | *             |
| **Smtp**          | Smtp settings                                 | Object      |               |
| Smtp.Host\*       | The host name to connect to                   | String      |               |
| Smtp.Port\*       | The port to connect to                        | Number      |               |
| Smtp.User         | The user name to authenticate                 | String      |               |
| Smtp.Password     | The password to authenticate                  | String      |               |
| Smtp.Security     | The security to use (None, Auto, Tls or Ssl)  | String      | None          |
| Smtp.LocalDomain  | The local domain used in the HELO or EHLO     | String      |               |

Exemple of appsettings.json
```json
{
  "Smtp": {
    "Host": "myhost.com",
    "Port": "25"
  }
}

```

More settings will come.

## API
### POST /send
Sends an email.

**Body**
- ```from*:``` The origin email adress
- ```fromName:``` The origin name
- ```to*:``` The destination email address(es)
- ```cc:``` The cc email address(es)
- ```subject:``` The subject of the message
- ```body:``` The message body
- ```isHtml:``` Define if the body is html

*Note: multiple adresses can be passed with ';' separation*
## Auth
Soon...


## Buy me a beer
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/trenoncourt/5)

# AutoMail
AutoMail is a tiny REST API to send mails based on the [MailKit](https://github.com/jstedfast/MailKit) library.

## Installation
Download the [last release](https://github.com/trenoncourt/AutoMail/releases) and place it on your server.

You can download version **1.1.1** [here](https://github.com/trenoncourt/AutoMail/releases/download/1.1.1/AutoMail-1.1.1.zip)

AutoMail is designed to be as light as possible so it does not contains the *Microsoft.AspNetCore.Server.IISIntegration* package.
If you want to use it on **IIS**, download the **iis-1.1.1** version from [here](https://github.com/trenoncourt/AutoMail/releases/download/1.1.1/AutoMail-iis-1.1.1.zip)

## Configuration
All the configuration can be made in environment variable or appsettings.json file :

| Name                | Description                                   | Type        | Default value |
| ------------------- | --------------------------------------------- | ----------- |--------------:|
| **Cors**            | Cors settings                                 | Object      |               |
| Cors.`Enabled`      | Define if cors are enabled                    | Boolean     | false         |
| Cors.`Methods`      | Adds specific methods to the policy           | String      | *             |
| Cors.`Origins`      | Adds specific origins to the policy           | String      | *             |
| Cors.`Headers`      | Adds specific headers to the policy           | String      | *             |
| **Smtp**            | Smtp settings                                 | Object      |               |
| Smtp.`Host`         | (**Required**) The host name to connect to    | String      |               |
| Smtp.`Port`         | (**Required**) The port to connect to         | Number      |               |
| Smtp.`User`         | The user name to authenticate                 | String      |               |
| Smtp.`Password`     | The password to authenticate                  | String      |               |
| Smtp.`Security`     | The security to use (None, Auto, Tls or Ssl)  | String      | None          |
| Smtp.`LocalDomain`  | The local domain used in the HELO or EHLO     | String      |               |
| **Server**          | Server settings                               | Object      |               |
| Server.`UseIIS`     | Adds IIS integration if you're using it       | Boolean     | false         |

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

| Property | Description | Type |
| --- | --- | --- |
| `from` | (**Required, can be omitted if user is in settings**) The origin email adress. | String |
| `fromName` | The origin name. | String |
| `to` | (**Required**) The destination email address(es). | String |
| `cc` | The cc email address(es). | String |
| `subject` | The subject of the message. | String |
| `body` | The message body. | String |
| `isHtml` | Define if the body is html. | Boolean |

*Note: multiple adresses can be passed with ';' separator*
## Auth
Soon...


## Buy me a beer
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.me/trenoncourt/5)

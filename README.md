# ChatWebApp

Simple browser-based chat application using .NET Core

This application allow several users to talk in a chatroom and also to get stock quotes from an API using a specific command.


## Pre-requisites
RabbitMQ, you can install it, or run with a Docker container:
```bash
docker run -it --rm --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

## Solution components
**ChatWebApp:** ASP.NET Core Web Application, provides communication between users using SignalR, for authentication it stores user accounts in the app using .NET Identity, also stores the messages into local db instance using EntityFramework.

**StockBotWorkerService:** .NET Core console app, reads from RabbitMQ, call the quote stock API and returns the result.

**NUnitTestProject:** Contains unit test classes.

**RPCComunicator:** Provides functionality to call to remote procedure through a client.


## Config files
appsettings.json contains the configuration, it is present on ChatWebApp and StockBotWorkerService:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=aspnet-ChatWebApp-83550D04-CF0B-4B72-AD87-42439F820F55;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*",
  "TopOnSelect": "50",
  "QueueConfiguration": {
    "Name": "rpc_queue",
    "HostName": "192.168.100.35"
  },
  "UrlApiStock": "https://stooq.com/q/l/?s={{StockCode}}&f=sd2t2ohlcv&h&e=csv"
}

```

The following node values must be verified, after running app:

**TopOnSelect:** Number of messages to be retreive from db on load page.

**QueueConfiguration.Name:** Queue name for communication using RabbitMQ

**QueueConfiguration.HostName:** Host name or IP for communication using RabbitMQ

**UrlApiStock:** URL to get stock quotes

## Installation

For release, installers are in the "Installers" folder.

For source code, in the NuGet Package Manager Console:
```bash
update-database
```

## Usage
You can create new users, enable and use in the app


## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)
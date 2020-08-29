using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RPCComunicator
{
    public class RpcServer
    {
        private readonly IConfiguration _configuration;
        public RpcServer(IConfiguration configuration)
        {
            _configuration = configuration;
        }



    }
}

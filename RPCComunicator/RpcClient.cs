using System;
using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Configuration;

namespace RPCComunicator
{
    public class RpcClient
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        private readonly IConfiguration _configuration;
        private readonly string _queueHostName;
        private readonly string _queueName;

        public RpcClient(IConfiguration configuration)
        {
            _configuration = configuration;
            _queueHostName = _configuration["QueueConfiguration:HostName"];
            _queueName = _configuration["QueueConfiguration:Name"];

            var factory = new ConnectionFactory() { HostName = _queueHostName };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);

            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    respQueue.Add(response);
                }
            };
        }

        public string Call(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: _queueName,
                basicProperties: props,
                body: messageBytes);
            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);
            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
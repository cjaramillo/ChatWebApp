using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace StockBotWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private Processor _processor;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _processor = new Processor(_configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string queueName = _configuration["QueueConfiguration:Name"];
            string queueHostName = _configuration["QueueConfiguration:HostName"];
            var factory = new ConnectionFactory() { HostName = queueHostName };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(queue: queueName, durable: false,
                exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);
            channel.BasicConsume(queue: queueName,
                autoAck: false, consumer: consumer);
            consumer.Received += (model, ea) =>
            {
                string response = string.Empty;
                var body = ea.Body.ToArray();
                var props = ea.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;
                try
                {
                    var message = Encoding.UTF8.GetString(body);
                    response = _processor.ProcessMessage(message);
                }
                catch (Exception e)
                {
                    _logger.LogError($"Message: {e.Message}\nStacktrace: {e.StackTrace}");
                }
                finally
                {
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                        basicProperties: replyProps, body: responseBytes);
                    channel.BasicAck(deliveryTag: ea.DeliveryTag,
                        multiple: false);
                }
            };
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _logger.LogInformation(" [x] Awaiting RPC requests");
        }
    }
}

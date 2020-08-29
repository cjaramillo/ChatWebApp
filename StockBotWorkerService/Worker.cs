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
using RPCComunicator;

namespace StockBotWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private Invoker _invoker;
        private Parser _parser;
        private RpcServer _rpcServer;

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _invoker = new Invoker(configuration); 
            _parser = new Parser(configuration);
            _rpcServer = new RpcServer(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //while (!stoppingToken.IsCancellationRequested)
            string queueName = _configuration["QueueConfiguration:Name"];
            string queueHostName = _configuration["QueueConfiguration:HostName"];

            var factory = new ConnectionFactory() { HostName = queueHostName };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            {
                channel.QueueDeclare(queue: queueName, durable: false,
                    exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: queueName,
                    autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC requests");

                consumer.Received += (model, ea) =>
                {
                    string response = null;

                    var body = ea.Body.ToArray();
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;
                    try
                    {
                        string[] commandSplitted;
                        var message = Encoding.UTF8.GetString(body);
                        if (message.StartsWith("/stock="))
                        {
                            commandSplitted = message.Split("/stock=");
                            if (commandSplitted.Length < 2)
                            {
                                response = "Stock code is required.";
                            }
                            else 
                            {
                                string apiResponse = _invoker.InvokeApi(commandSplitted[1]);
                                var parsedResponse = _parser.ParseContent(apiResponse);
                                response = $"{parsedResponse["Symbol"]} quote is ${parsedResponse["Close"]} per share";
                            }
                        }
                        else
                        {
                            response = "Command not recognized";
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [.] " + e.Message);
                        response = "";
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
            }
            
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //await Task.Delay(1000, stoppingToken);
            
        }
    }
}

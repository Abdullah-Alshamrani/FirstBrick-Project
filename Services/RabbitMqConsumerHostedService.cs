/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using Microsoft.Extensions.Hosting; //BackgroundService class for running long-running tasks.
using Microsoft.Extensions.Logging; //logging capabilities for diagnostic purposes.
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events; // provides neccussary claassess for handling RabbitMq events.
using System.Text;

namespace FirstBrickAPI.Services
{

    //background service that listens to messages from RabbitMq.
    public class RabbitMqConsumerHostedService : BackgroundService
    {
        private readonly ILogger<RabbitMqConsumerHostedService> _logger; //logger
        private readonly IConfiguration _configuration;
        private IConnection _connection; //connection to rabbitMq.
        private IModel _channel;
        // initilizing connectiion and channel for rabbitMq.
        public RabbitMqConsumerHostedService(IConfiguration configuration, ILogger<RabbitMqConsumerHostedService> logger)
        {
            _logger = logger;
            _configuration = configuration;

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMq:Username"],
                Password = _configuration["RabbitMq:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _configuration["RabbitMq:QueueName"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        // main logic for consiming from rabbitmq

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation($"Message received: {message}");
                // Add your message processing logic here
            };


            //start consuming messages from the queue
            _channel.BasicConsume(
                queue: _configuration["RabbitMq:QueueName"],
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }


        //cleaning up resources
        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}

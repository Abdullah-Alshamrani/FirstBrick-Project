/*
Developer: Abdullah Alshamrani
FirstBrick Project
VillaCapital
*/



using Microsoft.Extensions.Configuration; //acess to config settings.
using RabbitMQ.Client; //library for mapping connection and messaging.
using System.Text;

namespace FirstBrickAPI.Services
{
    //class for for producing and sending messages to RabbitMq.
    public class RabbitMqProducer
    {
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;


        // Initializes the RabbitMqProducer and sets up the RabbitMQ connection and channel.
        public RabbitMqProducer(IConfiguration configuration)
        {
            _configuration = configuration; //injects the configration dependency

            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMq:Host"],
                Port = int.TryParse(_configuration["RabbitMq:Port"], out var port) ? port : 5672,
                UserName = _configuration["RabbitMq:Username"],
                Password = _configuration["RabbitMq:Password"]
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

             // Declare a queue with specified settings.

            _channel.QueueDeclare(
                queue: _configuration["RabbitMq:QueueName"],
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        //publishing a message to the queue
        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: "",
                routingKey: _configuration["RabbitMq:QueueName"],
                basicProperties: null,
                body: body);
        }

        // Cleans up resources by closing the channel and connection.
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}

using GeekShopping.CartAPI.Messages;
using GeekShopping.MessageBus;
using Microsoft.AspNetCore.Components;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.CartAPI.RabbitMQSender
{
    public class RabbitMQMessageSender : IRabbitMQMessageSender
    {

        private readonly string _hostName;
        private readonly string _password;
        private readonly string _userName;

        private IConnection _connection;

        public RabbitMQMessageSender(
            IConfiguration configuration    
        )
        {
            _hostName = configuration["RabbitMQConnectionData:hostName"];
            _password = configuration["RabbitMQConnectionData:password"];
            _userName = configuration["RabbitMQConnectionData:userName"];
        }

        public void SendMessage(BaseMessage message, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName= _userName,
                Password = _password
            };

            _connection = factory.CreateConnection();

            using var channel = _connection.CreateModel();
            channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);

            byte[] body = getMessageAsByteArray(message);
            channel.BasicPublish(
                    exchange: "", 
                    routingKey: queueName, 
                    basicProperties: null,
                    body: body
            );

        }

        private byte[] getMessageAsByteArray(BaseMessage message)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };

            var json = JsonSerializer.Serialize<CheckoutHeaderVO>((CheckoutHeaderVO)message, options);
            var body = Encoding.UTF8.GetBytes(json);
            return body;
        }
    }
}

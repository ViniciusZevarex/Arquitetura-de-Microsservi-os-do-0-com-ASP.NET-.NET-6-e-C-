using GeekShopping.Email.Messages;
using GeekShopping.Email.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {

        private readonly EmailRepository _emailRepository;
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "DirectPaymentUpdateExchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";


        public RabbitMQPaymentConsumer(EmailRepository emailRepository,
                                       IConfiguration configuration)
        {
            _emailRepository = emailRepository;

            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQConnectionData:hostName"],
                UserName = configuration["RabbitMQConnectionData:password"],
                Password = configuration["RabbitMQConnectionData:userName"]
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            _channel.QueueBind(PaymentEmailUpdateQueueName, ExchangeName, "PaymentEmail");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                UpdatePaymentResultMessage message = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);
                processLogs(message).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume(PaymentEmailUpdateQueueName, false, consumer);

            return Task.CompletedTask;
        }



        private async Task processLogs(UpdatePaymentResultMessage message)
        {
            try
            {
                await _emailRepository.LogEmail(message);
            }
            catch(Exception ex) {
                throw ex;
            }

        }


    }
}

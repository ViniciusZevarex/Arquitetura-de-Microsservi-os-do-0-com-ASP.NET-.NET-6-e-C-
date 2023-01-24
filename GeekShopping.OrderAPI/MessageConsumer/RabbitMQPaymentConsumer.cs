﻿using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.RabbitMQSender;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQPaymentConsumer : BackgroundService
    {

        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;

        private const string ExchangeName = "FanoutPaymentUpdateExchange";

        string queueName = "";

        public RabbitMQPaymentConsumer(OrderRepository orderRepository,
                                       IConfiguration configuration)
        {
            _orderRepository = orderRepository;

            var connectionFactory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQConnectionData:hostName"],
                UserName = configuration["RabbitMQConnectionData:password"],
                Password = configuration["RabbitMQConnectionData:userName"]
            };

            _connection = connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(ExchangeName, ExchangeType.Fanout);
            queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queueName, ExchangeName, "");

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                UpdatePaymentResultVO vo = JsonSerializer.Deserialize<UpdatePaymentResultVO>(content);
                updatePaymentStatus(vo).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume(queueName, false, consumer);

            return Task.CompletedTask;
        }



        private async Task updatePaymentStatus(UpdatePaymentResultVO vo)
        {
            try
            {
                await _orderRepository.UpdateOrderPaymentStatus(vo.OrderId,vo.Status);
            }
            catch(Exception ex) {
                throw ex;
            }

        }


    }
}

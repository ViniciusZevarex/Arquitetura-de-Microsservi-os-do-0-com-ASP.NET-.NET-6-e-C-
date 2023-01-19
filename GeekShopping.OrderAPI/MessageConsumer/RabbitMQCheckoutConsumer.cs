using GeekShopping.OrderAPI.Messages;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.MessageConsumer
{
    public class RabbitMQCheckoutConsumer : BackgroundService
    {

        private readonly OrderRepository _orderRepository;
        private IConnection _connection;
        private IModel _channel;


        public RabbitMQCheckoutConsumer(OrderRepository orderRepository,
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
            _channel.QueueDeclare(queue: "checkoutqueue", false, false, false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (channel, evt) =>
            {
                var content = Encoding.UTF8.GetString(evt.Body.ToArray());
                CheckoutHeaderVO vo = JsonSerializer.Deserialize<CheckoutHeaderVO>(content);
                processOrder(vo).GetAwaiter().GetResult();
                _channel.BasicAck(evt.DeliveryTag, false);
            };

            _channel.BasicConsume("checkoutqueue", false, consumer);

            return Task.CompletedTask;
        }

        private async Task processOrder(CheckoutHeaderVO vo)
        {
            OrderHeader order = new()
            {
                UserId = vo.UserId,
                FirstName = vo.FirstName,
                LastName = vo.LastName,
                OrderDetails = new List<OrderDetail>(),
                CardNumber = vo.CardNumber,
                CouponCode = vo.CouponCode,
                CVV = vo.CVV,
                DiscountAmount = vo.DiscountAmount,
                Email = vo.Email,
                ExpiryMonthYear = vo.ExpiryMonthYear,
                OrderTime = DateTime.Now,
                PurchaseAmount = vo.PurchaseAmount,
                PaymentStatus = false,
                Phone = vo.Phone,
                DateTime = vo.Time
            };

            foreach (var details in vo.CartDetails)
            {
                OrderDetail detail = new()
                {
                    ProductId = details.ProductId,
                    ProductName = details.Product.Name,
                    Price = details.Product.Price,
                    Count = details.Count,
                };
                order.CartTotalItens += details.Count;
                order.OrderDetails.Add(detail);
            }

            await _orderRepository.AddOrder(order);
        }
    }
}

using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PlexNotifierrDiscord.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using IConnection = RabbitMQ.Client.IConnection;

namespace PlexNotifierrDiscord.Services
{
    public class MessageReceiver : BackgroundService
    {
        private readonly DiscordShardedClient _client;
        private readonly string _hostName;

        private readonly ILogger<MessageReceiver> _logger;
        private readonly string _password;
        private readonly int _port;
        private readonly string _userName;
        private readonly string _virtualHost;
        private IModel? _channel;
        private IConnection? _connection;

        public MessageReceiver(IOptions<RabbitMqConfig> options, ILogger<MessageReceiver> logger, DiscordShardedClient client)
        {
            _hostName = options.Value.HostName;
            _userName = options.Value.UserName;
            _password = options.Value.Password;
            _port = options.Value.Port;
            _virtualHost = options.Value.VirtualHost;
            _logger = logger;
            _client = client;
            InitializeRabbitMqListener();
        }

        private void InitializeRabbitMqListener()
        {
            var factory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                Port = _port,
                VirtualHost = _virtualHost
            };
            _connection = factory.CreateConnection();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown!;
            _channel = _connection.CreateModel();
            _channel.QueueDeclare("discord", true, false, false, null);
            _channel.BasicQos(0, 1, false);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            stoppingToken.ThrowIfCancellationRequested();
            _logger.LogInformation("Starting RabbitMQ Listener");

            // Listen on a RabbitMQ queue and ack messages
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (_, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                var plexNotification = JsonConvert.DeserializeObject<PlexNotification>(content);

                await HandleMessage(plexNotification);

                _channel?.BasicAck(ea.DeliveryTag, false);
            };
            consumer.Shutdown += OnConsumerShutdown!;
            consumer.Registered += OnConsumerRegistered!;
            consumer.Unregistered += OnConsumerUnregistered!;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled!;
            _channel.BasicConsume("discord", false, consumer);
        }

        private async Task HandleMessage(PlexNotification? plexNotification)
        {
            if (plexNotification == null)
            {
                _logger.LogError("Plex notification is null");
                return;
            }
            try
            {
                var user = await _client.Rest.GetUserAsync(Convert.ToUInt64(plexNotification.DiscordId));
                await user.SendMessageAsync($"Nouvel épisode de la série {plexNotification.Title} disponible !");
            }
            catch (Exception e)
            {
                _logger.LogError("Error while handling message", e);
            }
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            _logger.LogError("Queue connection lost. Trying to reconnect...");
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.ServiceBus;

namespace WorkerServiceBusTopic
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ParametrosExecucao _parametrosExecucao;
        private readonly ISubscriptionClient _client;
        private readonly string _subscription;

        public Worker(ILogger<Worker> logger,
            ParametrosExecucao parametrosExecucao)
        {
            _logger = logger;
            _parametrosExecucao = parametrosExecucao;
            _subscription = parametrosExecucao.Subscription;
            string nomeTopic = parametrosExecucao.Topic;
            
            _client = new SubscriptionClient(
                parametrosExecucao.ConnectionString,
                nomeTopic, _subscription);

            _logger.LogInformation($"Topic = {nomeTopic}");
            _logger.LogInformation($"Subscription = {_subscription}");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Run(() =>
                {
                    _logger.LogInformation("Aguardando mensagens...");
                    RegisterOnMessageHandlerAndReceiveMessages();
                });
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _client.CloseAsync();
            _logger.LogInformation(
                "Conexao com o Azure Service Bus fechada!");
        }

        private void RegisterOnMessageHandlerAndReceiveMessages()
        {
            var messageHandlerOptions = new MessageHandlerOptions(
                ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _client.RegisterMessageHandler(
                ProcessMessagesAsync, messageHandlerOptions);
        }

        private async Task ProcessMessagesAsync(Message message, CancellationToken token)
        {
            _logger.LogInformation($"[{_subscription} | Nova mensagem] " +
                Encoding.UTF8.GetString(message.Body));
            await _client.CompleteAsync(
                message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError($"Message handler - Tratamento - Exception: {exceptionReceivedEventArgs.Exception}.");

            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError("Exception context - informaçoes para resolução de problemas:");
            _logger.LogError($"- Endpoint: {context.Endpoint}");
            _logger.LogError($"- Entity Path: {context.EntityPath}");
            _logger.LogError($"- Executing Action: {context.Action}");

            return Task.CompletedTask;
        }
    }
}
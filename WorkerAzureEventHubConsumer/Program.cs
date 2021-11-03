using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerAzureEventHubConsumer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(
                "Testando o processamento de eventos com Azure Event Hubs");

            if (args.Length != 5)
            {
                Console.WriteLine(
                    "Informe 5 parametros: " +
                    "no primeiro a string de conexao com o Azure Event Hubs, " +
                    "no segundo o nome do Event Hub que recebera as mensagens, " +
                    "no terceiro o nome do Consumer Group da aplicacao, " +
                    "no quarto a string de conexao com o Blob Storage, " +
                    "no quinto o nome do Container do Blob Storage...");
                return;
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<ParametrosExecucao>(
                        new ParametrosExecucao()
                        {
                            EventHubsConnectionString = args[0],
                            EventHub = args[1],
                            ConsumerGroup = args[2],
                            BlobStorageConnectionString = args[3],
                            BlobContainerName = args[4]
                        });

                    services.AddHostedService<Worker>();
                });
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WorkerServiceBusTopic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine(
                "Testando o consumo de mensagens com Azure Service Bus + Topicos");

            if (args.Length != 3)
            {
                Console.WriteLine(
                    "Informe 3 parametros: " +
                    "no primeiro a string de conexao com o Azure Service Bus, " +
                    "no segundo o Topico a ser utilizado no recebimento das mensagens, " +
                    "no terceiro a Subscription da aplicação...");
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
                            ConnectionString = args[0],
                            Topic = args[1],
                            Subscription = args[2]
                        });
                    services.AddHostedService<Worker>();
                });
    }
}
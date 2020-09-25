using Amazon.Lambda.Core;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace LambdaExample
{
    class Program
    {
        static void Main(string[] args)
        {   var _serviceProvider = StartUp.Container.BuildServiceProvider();
            var exampleEnv = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var DBClient = _serviceProvider.GetRequiredService<IAmazonDynamoDB>();
            Console.WriteLine("Hello World!");
        }

    }

    public class StartUp
    {
        public static IServiceCollection Container => ConfigureServices();
        public static IConfigurationRoot Configuration  => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

        private static IServiceCollection ConfigureServices()
        {   
            var services = new ServiceCollection();
            
            // This line is required but perhaps it shouldn't be?
            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.AddAWSService<IAmazonDynamoDB>();
                    
            return services;
        }
    }
    
}

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Amazon.DynamoDBv2;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace HelloWorld
{

    public class Function
    {   
       private readonly IServiceProvider _serviceProvider;

        
        public Function(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            

        }
        public Function() : this(StartUp.Container.BuildServiceProvider())
        {
        }
        private static readonly HttpClient client = new HttpClient();

        private static async Task<string> GetCallingIP()
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "AWS Lambda .Net Client");

            var msg = await client.GetStringAsync("http://checkip.amazonaws.com/").ConfigureAwait(continueOnCapturedContext:false);

            return msg.Replace("\n","");
        }



        public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest apigProxyEvent, ILambdaContext context)
        {   
            //Would be good to log values of DBClient here but would need reflection as debug not supported
            var DBClient = _serviceProvider.GetRequiredService<IAmazonDynamoDB>();  
            var location = await GetCallingIP();
            var body = new Dictionary<string, string>
            {
                { "message", "hello world" },
                { "location", location }
            };

            return new APIGatewayProxyResponse
            {
                Body = JsonConvert.SerializeObject(body),
                StatusCode = 200,
                Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
            };
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

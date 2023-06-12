using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebApi
{
    public class Program
    {
        public static readonly int MaxSqlResultLength = 100;
        public static MySqlConnection? connection = null;

        private static async Task<bool> ConnectToMysql()
        {
            connection = new MySqlConnection("server=localhost;user=root;password=Anyangzyl403!;database=ZTA");
            return await Task.Run(() => {
                try
                {
                    connection.Open();
                }
                catch (Exception)
                {
                    return false;
                }
                return connection.State.ToString() == "Open";
            });
        }
        public static async Task Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);

            Console.WriteLine("Connecting...");
            var succ = await ConnectToMysql();

            if (succ)
            {
                Console.WriteLine("Connection successful.");
                WebApplication app = builder.Build();
                ConfigureWebApplication(app);
                app.Run();
            }
            else
            {
                Console.WriteLine("Connection failed.");
                Environment.Exit(0);
            }
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddControllers()
                .AddNewtonsoftJson();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void ConfigureWebApplication(WebApplication app)
        {
            app.UseHttpsRedirection();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseAuthorization();

            app.MapControllers();
        }
    }
}
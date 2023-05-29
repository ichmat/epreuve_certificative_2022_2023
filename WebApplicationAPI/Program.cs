
using AppCore.Context;
using AppCore.Services;

namespace WebApplicationAPI
{
    public class Program
    {
        public static FTMServerManager serverManager = new FTMServerManager();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfiguration configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json", true, true)
               .Build();

            // Add services to the container.
            builder.Services.AddDbContext<FTDbContext>();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            FTDbContext.TriggerConfigureFinish();

            app.Run();
        }


        public static void Log(string message, TypeLog type = TypeLog.Info)
        {
            switch (type)
            {
                case TypeLog.Info:
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case TypeLog.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case TypeLog.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
            }

            Console.WriteLine(message);
            Console.ResetColor();
        }
    }

    public enum TypeLog
    {
        Info,
        Warning,
        Error,
    }
}
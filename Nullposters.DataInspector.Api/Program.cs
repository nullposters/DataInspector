
using Nullposters.DataInspector.Api.Services;

namespace Nullposters.DataInspector.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // services
            builder.Services.AddSingleton<SchemaService>();

            // cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowBlazorClient",
                    builder =>
                    {
                        builder.WithOrigins("https://localhost:7233")
                               .AllowAnyMethod()
                               .AllowAnyHeader()
                               .AllowCredentials();
                    });
            });

            builder.Services.AddControllers();
            // swaggerUI
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

            app.UseCors("AllowBlazorClient");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

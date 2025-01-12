using BLL.Profiles;
using BLL.Services;
using BLL.ServicesInterfaces;
using BookAPI.Cache;
using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace BookAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var configuration = builder.Configuration;

            builder.Services.AddDbContext<BookDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("BookDbConnection"));
            });

            ///
            /// AddTransient - Создает новый экземпляр каждый раз, когда он запрашивается
            /// AddScoped - Создает один экземпляр на каждый запрос (HTTP request)
            /// AddSingleton - Создает один экземпляр на все время жизни приложения
            ///

            // custom repositories
            builder.Services.AddScoped<IAuthorRepository, AuthorsRepository>();

            // custom services
            builder.Services.AddScoped<IAuthorService, AuthorsService>();

            // redis
            builder.Services.AddSingleton<IConnectionMultiplexer>(servicProvider =>
            {
                return ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("RedisConnection")
                        ?? throw new Exception("Сonnection string for 'RedisConnection' not found.")
                );
            });

            builder.Services.AddScoped<ICacheService, CacheService>();

            // mapping profiles
            builder.Services.AddAutoMapper(
                typeof(AuthorProfile)
            );

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

            app.Run();
        }
    }
}

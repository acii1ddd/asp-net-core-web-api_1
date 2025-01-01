using BLL.Profiles;
using BLL.Services;
using DAL.Context;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;

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

            // custom repositories
            builder.Services.AddTransient<IAuthorRepository, AuthorRepository>();

            // custom services
            builder.Services.AddTransient<AuthorService>();

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

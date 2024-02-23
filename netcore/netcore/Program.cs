using Microsoft.AspNetCore.Hosting;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//���ù���
ConfigHelper.Configuration = builder.Configuration;

var app = builder.Build();

//������̬�ļ�����
app.UseStaticFiles();
// Configure the HTTP request pipeline.
app.UseSwagger();
 
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Nightscout API");
    c.RoutePrefix = "";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
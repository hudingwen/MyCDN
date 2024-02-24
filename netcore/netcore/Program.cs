using Microsoft.AspNetCore.Hosting;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                      });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//配置管理
ConfigHelper.Configuration = builder.Configuration;

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);
//开启静态文件访问
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

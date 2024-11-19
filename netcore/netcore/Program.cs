using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using netcore.Config;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

//内置web大小限制
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1073741824; // 10GB
});
//表单大小限制
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824; // 10GB
});
// Add services to the container.
//var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy(name: MyAllowSpecificOrigins,
//                      policy =>
//                      {
//                          policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//                      });
//});


builder.Services.AddControllers();
//日志开启
builder.SetLog4Net();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//配置管理
ConfigHelper.Configuration = builder.Configuration;

var app = builder.Build();

//app.UseCors(MyAllowSpecificOrigins);
//开启静态文件访问
//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true, // 允许提供未知文件类型
    DefaultContentType = "application/octet-stream" // 指定默认内容类型
});
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

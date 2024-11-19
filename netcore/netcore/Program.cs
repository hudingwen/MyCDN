using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using netcore.Config;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);

//����web��С����
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1073741824; // 10GB
});
//����С����
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
//��־����
builder.SetLog4Net();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//���ù���
ConfigHelper.Configuration = builder.Configuration;

var app = builder.Build();

//app.UseCors(MyAllowSpecificOrigins);
//������̬�ļ�����
//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true, // �����ṩδ֪�ļ�����
    DefaultContentType = "application/octet-stream" // ָ��Ĭ����������
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

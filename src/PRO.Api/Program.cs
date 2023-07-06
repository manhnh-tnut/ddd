using System.Net;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using PRO.Api.Extensions;
using PRO.Api.Features.Grpc.Services;
using PRO.Api.Infrastructure.AutofacModules;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Any, builder.Configuration.GetValue("HTTP_PORT",5001), listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
        });

        options.Listen(IPAddress.Any, builder.Configuration.GetValue("GRPC_PORT",5000), listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        });
    });

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
// Add services to the container.
builder.Services
        .AddCustomGrpc()
        .AddApiVersion()
        .AddLogging(builder)
        .AddCustomController()
        .AddCustomCaching(builder.Configuration)
        .AddElasticSearch(builder.Configuration)
        .AddHealthChecks(builder.Configuration)
        .AddCustomDbContext(builder.Configuration)
        .AddCustomSwagger(builder.Configuration)
        .AddCustomConfiguration(builder.Configuration)
        .AddCustomAuthentication(builder.Configuration);

builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Host.ConfigureContainer<ContainerBuilder>(c =>
{
    c.RegisterModule(new MediatorModule());
    c.RegisterModule(new ApplicationModule());
});

var app = builder.Build();

app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapGrpcReflectionService();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PRO.Api v1");
        c.OAuthClientId("pro.api.swaggerui");
        c.OAuthAppName("PRO.Api Swagger UI");
    });
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();

app.MapGrpcService<GrpcPlcService>();
app.MapGrpcService<GrpcElasticService>();
app.MapGet("/_proto/", async ctx =>
{
    ctx.Response.ContentType = "text/plain";
    using var fs = new FileStream(Path.Combine(app.Environment.ContentRootPath, "Features\\Grpc\\Proto", "plc.proto"), FileMode.Open, FileAccess.Read);
    using var sr = new StreamReader(fs);
    while (!sr.EndOfStream)
    {
        var line = await sr.ReadLineAsync();
        if (line != "/* >>" || line != "<< */")
        {
            await ctx.Response.WriteAsync(line);
        }
    }
});
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGrpcHealthChecksService();
app.Run();
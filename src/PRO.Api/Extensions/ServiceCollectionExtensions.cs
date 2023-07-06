using System.Reflection;
using Elasticsearch.Net;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Nest;
using PRO.Api.Controllers;
using PRO.Api.Infrastructure.Filters;
using PRO.Infrastructure.Data;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace PRO.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomGrpc(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableDetailedErrors = true;
            }).Services
            .AddGrpcReflection()
            .AddGrpcHealthChecks()
                .AddCheck("/grpc-health", () => HealthCheckResult.Healthy());
            return services;
        }

        public static IServiceCollection AddLogging(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Logging.ClearProviders();
            builder.Host.UseSerilog((ctx, lc) => lc
                .WriteTo.Console()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(ctx.Configuration.GetConnectionString("ELKConnection")))
                {
                    AutoRegisterTemplate = true,
                    AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv6
                })
                .ReadFrom.Configuration(ctx.Configuration)
            );
            return services;
        }

        public static IServiceCollection AddApiVersion(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("X-Version"),
                    new MediaTypeApiVersionReader("ver"));
            });
            services.AddVersionedApiExplorer(
             options =>
             {
                 options.GroupNameFormat = "'v'VVV";
                 options.SubstituteApiVersionInUrl = true;
             });
            return services;
        }

        public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var pool = new SingleNodeConnectionPool(new Uri(configuration.GetConnectionString("ELKConnection")));
            var settings = new ConnectionSettings(pool)
                .DefaultIndex($"{Assembly.GetExecutingAssembly().GetName().Name.ToLower().Replace('.','-')}");
            var client = new ElasticClient(settings);
            services.AddSingleton(client);
            return services;
        }

        public static IServiceCollection AddCustomController(this IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers(options =>
                {
                    options.Filters.Add(typeof(ErrorHandlingFilter));
                })
                // Added for functional tests
                .AddApplicationPart(typeof(HomeController).Assembly)
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "PRO.API", Version = "v1" });
                // var filePath = Path.Combine(System.AppContext.BaseDirectory, "GrpcServer.xml");
                // options.IncludeXmlComments(filePath);
                // options.IncludeGrpcXmlComments(filePath, includeControllerXmlComments: true);
                // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                // {
                //     Type = SecuritySchemeType.OAuth2,
                //     Flows = new OpenApiOAuthFlows()
                //     {
                //         Implicit = new OpenApiOAuthFlow()
                //         {
                //             AuthorizationUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/authorize"),
                //             TokenUrl = new Uri($"{configuration.GetValue<string>("IdentityUrlExternal")}/connect/token"),
                //             Scopes = new Dictionary<string, string>()
                //             {
                //             { "orders", "Ordering API" }
                //             }
                //         }
                //     }
                // });

                // options.OperationFilter<AuthorizeOperationFilter>();
            });

            return services;
        }

        public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var builder = services.AddHealthChecks();

            builder.AddCheck("self", () => HealthCheckResult.Healthy());

            builder.AddSqlServer(configuration.GetConnectionString("ApplicationConnection"));
            return services;
        }

        public static IServiceCollection AddCustomCaching(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisConnection");
                options.InstanceName = $"{System.Reflection.Assembly.GetEntryAssembly().GetName().Name}: ";
            });
            return services;
        }

        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EFContext>(options =>
                    {
                        options.UseSqlServer(configuration["ConnectionStrings:ApplicationConnection"],
                            sqlServerOptionsAction: sqlOptions =>
                            {
                                sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                                sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                            });
                    },
                        ServiceLifetime.Scoped  //Showing explicitly that the DbContext is shared across the HTTP request scope (graph of objects started in the HTTP request)
                    );
            return services;
        }

        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            // services.Configure<OrderingSettings>(configuration);
            // services.Configure<ApiBehaviorOptions>(options =>
            // {
            //     options.InvalidModelStateResponseFactory = context =>
            //     {
            //         var problemDetails = new ValidationProblemDetails(context.ModelState)
            //         {
            //             Instance = context.HttpContext.Request.Path,
            //             Status = StatusCodes.Status400BadRequest,
            //             Detail = "Please refer to the errors property for additional details."
            //         };

            //         return new BadRequestObjectResult(problemDetails)
            //         {
            //             ContentTypes = { "application/problem+json", "application/problem+xml" }
            //         };
            //     };
            // });

            return services;
        }

        public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // prevent from mapping "sub" claim to nameidentifier.
            // JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

            // var identityUrl = configuration.GetValue<string>("IdentityUrl");

            // services.AddAuthentication(options =>
            // {
            //     options.DefaultAuthenticateScheme = AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            //     options.DefaultChallengeScheme = AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;

            // }).AddJwtBearer(options =>
            // {
            //     options.Authority = identityUrl;
            //     options.RequireHttpsMetadata = false;
            //     options.Audience = "orders";
            // });
            return services;
        }
    }
}
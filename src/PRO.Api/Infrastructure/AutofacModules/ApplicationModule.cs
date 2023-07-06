using System.Reflection;
using Autofac;
using PRO.Api.Features.User.Commands;
using PRO.Api.Features.User.Responses;
using PRO.Infrastructure.Data.Repositories;
using PRO.Domain.Interfaces;
using PRO.Api.Features.User.Queries;
using MediatR;
using PRO.Domain.Entities.Departments;
using PRO.Domain.Entities.Users;

namespace PRO.Api.Infrastructure.AutofacModules;

public class ApplicationModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<UserInfoResponse>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DapperRepository>()
            .As<IDapperRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<DepartmentRepository>()
            .As<IDepartmentRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterType<UserRepository>()
            .As<IUserRepository>()
            .InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(
                typeof(RegisterUserCommandHandler).GetTypeInfo().Assembly
                , typeof(GetUserQueryHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<>));
    }
}

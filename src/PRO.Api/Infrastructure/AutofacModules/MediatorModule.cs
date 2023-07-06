using System.Reflection;
using Autofac;
using FluentValidation;
using MediatR;
using PRO.Api.Features.User.Commands;
using PRO.Api.Features.User.Events;
using PRO.Api.Features.User.Queries;
using PRO.Api.Features.User.Validations;
using PRO.Api.Infrastructure.Behaviors;

namespace PRO.Api.Infrastructure.AutofacModules;

public class MediatorModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
            .AsImplementedInterfaces();

        // Register all the Command and Query classes (they implement IRequestHandler) in assembly holding the Commands
        builder.RegisterAssemblyTypes(
                typeof(RegisterUserCommand).GetTypeInfo().Assembly,
                typeof(GetUserQuery).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IRequestHandler<,>));

        // // Register the DomainEventHandler classes (they implement INotificationHandler<>) in assembly holding the Domain Events
        builder.RegisterAssemblyTypes(typeof(RegisteredEventHandler).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(INotificationHandler<>));

        // Register the Command's Validators (Validators based on FluentValidation library)
        builder
            .RegisterAssemblyTypes(typeof(RegisterUserCommandValidator).GetTypeInfo().Assembly)
            .Where(t => t.IsClosedTypeOf(typeof(IValidator<>)))
            .AsImplementedInterfaces();

        builder.Register<ServiceFactory>(context =>
        {
            var componentContext = context.Resolve<IComponentContext>();
            return t => { object o; return componentContext.TryResolve(t, out o) ? o : null; };
        });

        builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(ValidatorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        builder.RegisterGeneric(typeof(TransactionBehaviour<,>)).As(typeof(IPipelineBehavior<,>));
    }
}

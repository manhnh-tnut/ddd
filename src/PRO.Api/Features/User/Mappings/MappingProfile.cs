using AutoMapper;
using PRO.Api.Features.User.Commands;
using PRO.Api.Features.User.Queries;
using PRO.Api.Features.User.Requests;
using PRO.Api.Features.User.Responses;

namespace PRO.Api.Features.User.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<GetUserRequest, GetUserQuery>();
        CreateMap<AddUserRequest, RegisterUserCommand>();
        CreateMap<PRO.Domain.Entities.Users.User, UserInfoResponse>();
    }
}
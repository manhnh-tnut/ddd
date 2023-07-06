using System.Runtime.Serialization;
using AutoMapper;
using MediatR;
using PRO.Api.Features.User.Requests;
using PRO.Api.Features.User.Responses;

namespace PRO.Api.Features.User.Queries;

[DataContract]
[AutoMap(typeof(GetUserRequest))]
public class GetUserQuery : IRequest<IEnumerable<UserInfoResponse>>
{
    [DataMember]
    public string Keyword { get; internal set; }
}
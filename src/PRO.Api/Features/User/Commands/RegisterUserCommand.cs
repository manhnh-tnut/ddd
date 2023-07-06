using System.Runtime.Serialization;
using AutoMapper;
using MediatR;
using PRO.Api.Features.User.Requests;

namespace PRO.Api.Features.User.Commands;

[DataContract]
[AutoMap(typeof(AddUserRequest))]
public class RegisterUserCommand : IRequest<bool>
{
    [DataMember]
    public string userName { get; internal set; }
    [DataMember]
    public string password { get; internal set; }
    [DataMember]
    public string firstName { get; internal set; }
    [DataMember]
    public string lastName { get; internal set; }
    [DataMember]
    public string address { get; internal set; }
    [DataMember]
    public short departmentId { get; internal set; }
    [DataMember]
    public DateTime? birthDate { get; internal set; }
}
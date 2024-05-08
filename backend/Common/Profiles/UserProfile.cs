using AutoMapper;

using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<UserModel, UserEntity>().ReverseMap();
    }
}

using AutoMapper;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Profiles;
public class SkinProfile : Profile
{
    public SkinProfile()
    {
        CreateMap<SkinEntity, SkinModel>();
    }
}

using AutoMapper;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Profiles;
public class SquadProfile : Profile
{
    public SquadProfile()
    {
        CreateMap<SquadModel, SquadEntity>().ReverseMap();

    }
}

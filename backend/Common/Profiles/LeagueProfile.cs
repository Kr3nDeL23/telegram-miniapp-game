using AutoMapper;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models;

namespace Presentation.Common.Profiles;
public class LeagueProfile : Profile
{
    public LeagueProfile()
    {
        CreateMap<LeagueModel, LeagueEntity>().ReverseMap();
    }
}

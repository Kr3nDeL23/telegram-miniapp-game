using AutoMapper;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models;

namespace Presentation.Common.Profiles;
public class ChallengeProfile : Profile
{
    public ChallengeProfile()
    {
        CreateMap<ChallengeModel, ChallengeEntity>().ReverseMap();

    }
}

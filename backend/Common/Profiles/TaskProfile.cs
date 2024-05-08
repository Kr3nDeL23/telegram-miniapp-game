using AutoMapper;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models;

namespace Presentation.Common.Profiles;
public class TaskProfile : Profile
{
    public TaskProfile()
    {
        CreateMap<TaskEntity, TaskModel>().ReverseMap();
    }
}

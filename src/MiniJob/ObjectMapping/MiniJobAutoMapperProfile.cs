using AutoMapper;
using MiniJob.Entities.Jobs;
using MiniJob.Jobs;
using MiniJob.Services.Dtos;

namespace MiniJob.ObjectMapping;

public class MiniJobAutoMapperProfile : Profile
{
    public MiniJobAutoMapperProfile()
    {
        CreateMap<AppInfo, AppInfoDto>();
        CreateMap<CreateUpdateAppInfoDto, AppInfo>();
        CreateMap<AppInfoDto, CreateUpdateAppInfoDto>();

        CreateMap<JobInfo, JobInfoDto>();
        CreateMap<CreateUpdateJobInfoDto, JobInfo>();

        CreateMap<ProcessorDto, ProcessorInfo>();
    }
}

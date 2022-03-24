using AutoMapper;
using MiniJob.Entities.Jobs;
using MiniJob.Jobs;

namespace MiniJob.ObjectMapping;

public class MiniJobAutoMapperProfile : Profile
{
    public MiniJobAutoMapperProfile()
    {
        CreateMap<AppInfo, AppInfoDto>();
        CreateMap<CreateUpdateAppInfoDto, AppInfo>();

        CreateMap<JobInfo, JobInfoDto>();
        CreateMap<CreateUpdateJobInfoDto, JobInfo>();
    }
}

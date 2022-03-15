using AutoMapper;
using MiniJob.Jobs;

namespace MiniJob;

public class MiniJobApplicationAutoMapperProfile : Profile
{
    public MiniJobApplicationAutoMapperProfile()
    {
        CreateMap<AppInfo, AppInfoDto>();
        CreateMap<CreateUpdateAppInfoDto, AppInfo>();

        CreateMap<JobInfo, JobInfoDto>();
        CreateMap<CreateUpdateJobInfoDto, JobInfo>();
    }
}
using AutoMapper;
using MiniJob.Jobs;

namespace MiniJob.Web;

public class MiniJobWebAutoMapperProfile : Profile
{
    public MiniJobWebAutoMapperProfile()
    {
        //Define your AutoMapper configuration here for the Web project.
        CreateMap<AppInfoDto, CreateUpdateAppInfoDto>();
    }
}
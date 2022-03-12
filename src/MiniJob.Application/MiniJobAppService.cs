using MiniJob.Localization;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Services;

namespace MiniJob
{
    /* Inherit your application services from this class.
     */
    public abstract class MiniJobAppService : ApplicationService
    {
        protected MiniJobAppService()
        {
            LocalizationResource = typeof(MiniJobResource);
        }
    }
}
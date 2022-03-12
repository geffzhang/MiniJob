﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniJob.Data;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace MiniJob.EntityFrameworkCore
{
    public class EntityFrameworkCoreMiniJobDbSchemaMigrator
        : IMiniJobDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreMiniJobDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving the MiniJobDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<MiniJobDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}
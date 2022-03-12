using MiniJob;
using MiniJob.Jobs;
using System.Diagnostics.CodeAnalysis;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;

namespace Microsoft.EntityFrameworkCore
{
    public static class MiniJobDbContextModelBuilderExtensions
    {
        public static void ConfigureMiniJob([NotNull] this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            builder.Entity<AppInfo>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "AppInfo", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(p => p.AppName).HasMaxLength(128);
                b.Property(p => p.Description).HasMaxLength(512);
                b.Property(p => p.IsEnabled);
            });

            builder.Entity<JobInfo>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "JobInfo", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(p => p.JobName).HasMaxLength(128).IsRequired();
                b.Property(p => p.Description).HasMaxLength(512);
                b.Property(p => p.JobArgs).HasMaxLength(1024);
                b.Property(p => p.Tags).HasMaxLength(128);
                b.Property(p => p.JobPriority);
                b.Property(p => p.TimeExpression);
                b.Property(p => p.TimeExpressionValue).HasMaxLength(128);
                b.Property(p => p.ExecuteType);
                b.Property(p => p.ProcessorType);
                b.Property(p => p.ExecutorInfo).HasMaxLength(256);
                b.Property(p => p.MaxInstanceCount);
                b.Property(p => p.Concurrency);
                b.Property(p => p.Timeout);
                b.Property(p => p.MaxTryCount);
                b.Property(p => p.BeginTime);
                b.Property(p => p.EndTime);
                b.Property(p => p.NextTriggerTime);
                b.Property(p => p.LastTriggerTime);
                b.Property(p => p.IsEnabled);
                b.Property(p => p.MinCpuCores);
                b.Property(p => p.MinMemorySpace);
                b.Property(p => p.MinDiskSpace);
                b.Property(p => p.MaxWorkerCount);

                b.HasMany(p => p.AlarmUsers).WithOne().HasForeignKey(p => p.JobInfoId).IsRequired();
            });

            builder.Entity<JobInfoIdentityUser>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "JobInfoIdentityUser", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();
                // 定义联合主键
                b.HasKey(p => new { p.JobInfoId, p.UserId });
                // 多对多关系配置
                b.HasOne<JobInfo>().WithMany(p => p.AlarmUsers).HasForeignKey(p => p.JobInfoId).IsRequired();
                b.HasOne<IdentityUser>().WithMany().HasForeignKey(p => p.UserId).IsRequired();

                b.HasIndex(p => new { p.JobInfoId, p.UserId });
            });

            builder.Entity<JobInstance>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "JobInstance", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(p => p.InstanceArgs).HasMaxLength(1024);
                b.Property(p => p.InstanceStatus);
                b.Property(p => p.Result).HasMaxLength(1024);
                b.Property(p => p.ExpectedTriggerTime);
                b.Property(p => p.ActualTriggerTime);
                b.Property(p => p.TryCount);
            });

            builder.Entity<ProcessorInfo>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "ProcessorInfo", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(p => p.FullName).HasMaxLength(256);
                b.Property(p => p.WorkerName).HasMaxLength(128);
                b.Property(p => p.Tags).HasMaxLength(128);
                b.Property(p => p.IsBuiltInExecutor);
                b.Property(p => p.IsEnabled);
            });

            builder.Entity<WorkerInfo>(b =>
            {
                b.ToTable(MiniJobConsts.DbTablePrefix + "WorkerInfo", MiniJobConsts.DbSchema);

                b.ConfigureByConvention();

                b.Property(p => p.Address).HasMaxLength(32);
                b.Property(p => p.Client).HasMaxLength(128);
                b.Property(p => p.Tags).HasMaxLength(128);
                b.Property(p => p.LastActiveTime);
                b.Property(p => p.CpuCores);
                b.Property(p => p.CpuUsed);
                b.Property(p => p.MemoryTotal);
                b.Property(p => p.MemoryUsed);
                b.Property(p => p.DiskTotal);
                b.Property(p => p.DiskUsed);
            });
        }
    }
}
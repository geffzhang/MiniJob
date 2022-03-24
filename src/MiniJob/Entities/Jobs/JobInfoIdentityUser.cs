using Volo.Abp.Domain.Entities;

namespace MiniJob.Entities.Jobs;

/// <summary>
/// 告警用户
/// </summary>
public class JobInfoIdentityUser : Entity
{
    public Guid JobInfoId { get; protected set; }

    public Guid UserId { get; protected set; }

    protected JobInfoIdentityUser() { }

    public JobInfoIdentityUser(Guid jobInfoId, Guid userId)
    {
        JobInfoId = jobInfoId;
        UserId = userId;
    }

    public override object[] GetKeys()
    {
        return new object[] { JobInfoId, UserId };
    }
}
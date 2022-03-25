using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Validation;
using Xunit;

namespace MiniJob.Jobs;

public class AppInfoAppService_Tests : MiniJobTestBase
{
    private readonly IAppInfoAppService _appInfoAppService;

    public AppInfoAppService_Tests()
    {
        _appInfoAppService = GetRequiredService<IAppInfoAppService>();
    }

    [Fact]
    public async Task Should_Get_List_Of_AppInfos()
    {
        //Act
        var result = await _appInfoAppService.GetListAsync(
            new PagedAndSortedResultRequestDto()
        );

        //Assert
        result.TotalCount.ShouldBeGreaterThan(0);
        result.Items.ShouldContain(b => b.AppName == "TestApp");
    }

    [Fact]
    public async Task Should_Create_A_Valid_AppInfo()
    {
        //Act
        var result = await _appInfoAppService.CreateAsync(
            new CreateUpdateAppInfoDto
            {
                AppName = "New test appInfo 42",
                IsEnabled = true
            }
        );

        //Assert
        result.Id.ShouldNotBe(Guid.Empty);
        result.AppName.ShouldBe("New test appInfo 42");
    }

    [Fact]
    public async Task Should_Not_Create_A_Book_Without_AppName()
    {
        var exception = await Assert.ThrowsAsync<AbpValidationException>(async () =>
        {
            await _appInfoAppService.CreateAsync(
                new CreateUpdateAppInfoDto
                {
                    AppName = "",
                    IsEnabled = true
                }
            );
        });

        exception.ValidationErrors
            .ShouldContain(err => err.MemberNames.Any(mem => mem == "AppName"));
    }
}
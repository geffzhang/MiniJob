using Shouldly;
using Xunit;

namespace MiniJob.Pages;

public class Index_Tests : MiniJobWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
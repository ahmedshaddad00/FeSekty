using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FeSekty.Pages;

[Collection(FeSektyTestConsts.CollectionDefinitionName)]
public class Index_Tests : FeSektyWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}

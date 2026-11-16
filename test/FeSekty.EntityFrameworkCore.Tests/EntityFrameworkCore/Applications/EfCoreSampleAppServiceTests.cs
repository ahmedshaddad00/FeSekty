using FeSekty.Samples;
using Xunit;

namespace FeSekty.EntityFrameworkCore.Applications;

[Collection(FeSektyTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FeSektyEntityFrameworkCoreTestModule>
{

}

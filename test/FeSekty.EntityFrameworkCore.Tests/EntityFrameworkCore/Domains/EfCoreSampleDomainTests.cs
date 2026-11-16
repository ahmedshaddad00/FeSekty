using FeSekty.Samples;
using Xunit;

namespace FeSekty.EntityFrameworkCore.Domains;

[Collection(FeSektyTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FeSektyEntityFrameworkCoreTestModule>
{

}

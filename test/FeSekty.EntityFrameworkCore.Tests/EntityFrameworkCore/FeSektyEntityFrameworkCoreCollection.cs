using Xunit;

namespace FeSekty.EntityFrameworkCore;

[CollectionDefinition(FeSektyTestConsts.CollectionDefinitionName)]
public class FeSektyEntityFrameworkCoreCollection : ICollectionFixture<FeSektyEntityFrameworkCoreFixture>
{

}

using Microsoft.AspNetCore.Builder;
using FeSekty;
using Volo.Abp.AspNetCore.TestBase;

var builder = WebApplication.CreateBuilder();
builder.Environment.ContentRootPath = GetWebProjectContentRootPathHelper.Get("FeSekty.Web.csproj"); 
await builder.RunAbpModuleAsync<FeSektyWebTestModule>(applicationName: "FeSekty.Web");

public partial class Program
{
}

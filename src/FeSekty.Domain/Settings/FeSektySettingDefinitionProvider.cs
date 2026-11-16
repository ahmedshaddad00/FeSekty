using Volo.Abp.Settings;

namespace FeSekty.Settings;

public class FeSektySettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FeSektySettings.MySetting1));
    }
}

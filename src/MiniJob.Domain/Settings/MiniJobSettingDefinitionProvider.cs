using Volo.Abp.Settings;

namespace MiniJob.Settings
{
    public class MiniJobSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(MiniJobSettings.MySetting1));
        }
    }
}
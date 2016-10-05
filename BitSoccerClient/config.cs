using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Client.Properties
{
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    [CompilerGenerated]
    internal sealed class Config : ApplicationSettingsBase
    {
        private static Config _config = (Config)SettingsBase.Synchronized((SettingsBase)new Config());

        public static Config Default
        {
            get
            {
                return Config._config;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string Team1Path
        {
            get
            {
                return (string)this["team1Path"];
            }
            set
            {
                this["team1Path"] = (object)value;
            }
        }

        [DefaultSettingValue("")]
        [DebuggerNonUserCode]
        [UserScopedSetting]
        public string Team2Path
        {
            get
            {
                return (string)this["team2Path"];
            }
            set
            {
                this["team2Path"] = (object)value;
            }
        }

        [DefaultSettingValue("True")]
        [UserScopedSetting]
        [DebuggerNonUserCode]
        public bool FirstTimeRunning
        {
            get
            {
                return (bool)this["FirstTimeRunning"];
            }
            set
            {
                this["FirstTimeRunning"] = (object)(value);
            }
        }

        [DefaultSettingValue("False")]
        [DebuggerNonUserCode]
        [UserScopedSetting]
        public bool ShowPrompt
        {
            get
            {
                return (bool)this["showPrompt"];
            }
            set
            {
                this["showPrompt"] = (object)(value);
            }
        }
    }
}

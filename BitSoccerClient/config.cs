using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Client.Properties
{
    [GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "10.0.0.0")]
    [CompilerGenerated]
    internal sealed class config : ApplicationSettingsBase
    {
        private static config _config = (config)SettingsBase.Synchronized((SettingsBase)new config());

        public static config Default
        {
            get
            {
                return config._config;
            }
        }

        [UserScopedSetting]
        [DebuggerNonUserCode]
        [DefaultSettingValue("")]
        public string team1Path
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
        public string team2Path
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
        public bool showPrompt
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

using System;
using System.Configuration;
using System.IO;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    public class ApplicationSettings
    {
        private bool _isAppConfigExists = false;
        private ExeConfigurationFileMap _exeFileMap = new ExeConfigurationFileMap();
        private Configuration config = null;
        public SettingsModel Settings = new SettingsModel();

        public ApplicationSettings()
        {
            _exeFileMap.ExeConfigFilename = Path.Combine(Application.StartupPath, "ApplicationSettings.config");
            config = ConfigurationManager.OpenMappedExeConfiguration(_exeFileMap, ConfigurationUserLevel.None);

            if (!File.Exists(_exeFileMap.ExeConfigFilename))
            {
                _isAppConfigExists = false;
                Settings.Password = "pass";
                Save();
            }
            else
            {
                _isAppConfigExists = true;
                Read();
            }
        }

        public bool IsAppConfigExists()
        {
            return _isAppConfigExists;
        }

        public void Read()
        {
            Settings = config.GetSection("SettingsModel") as SettingsModel;
        }

        public bool Save()
        {
            bool ret = false;

            try
            {
                SettingsModel smodel = new SettingsModel();
                smodel.Password = Settings.Password;
                smodel.HideWhenMinimized = Settings.HideWhenMinimized;
                smodel.HideInformationPopupWindow = Settings.HideInformationPopupWindow;

                config.Sections.Remove("SettingsModel");
                config.Sections.Add("SettingsModel", smodel);

                smodel = null;

                config.Save(ConfigurationSaveMode.Modified);

                // update our configuration
                Read();

                ret = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    string.Format("An error has occured while saving the configuration.\r\n\r\nMessage: {0}\r\n\r\nSource:\r\n{1}",
                        ex.Message, ex.StackTrace
                    ),
                    "ApplicationSettings", MessageBoxButtons.OK, MessageBoxIcon.Information
                );

                ret = false;
            }

            return ret;
        }

        public class SettingsModel : ConfigurationSection
        {
            public SettingsModel()
            {
                HideInformationPopupWindow = true;
            }

            [ConfigurationProperty("Password")]
            public string Password
            {
                get
                {
                    string ret = RijndaelSettings.Decrypt((string)this["Password"]);
                    return ret;
                }
                set
                {
                    string val = RijndaelSettings.Encrypt(value);
                    this["Password"] = val;
                }
            }

            [ConfigurationProperty("HideWhenMinimized")]
            public bool HideWhenMinimized
            {
                get
                {
                    return (bool)this["HideWhenMinimized"];
                }
                set
                {
                    this["HideWhenMinimized"] = value;
                }
            }

            [ConfigurationProperty("HideInformationPopupWindow")]
            public bool HideInformationPopupWindow
            {
                get
                {
                    return (bool)this["HideInformationPopupWindow"];
                }
                set
                {
                    this["HideInformationPopupWindow"] = value;
                }
            }
        }
    }
}
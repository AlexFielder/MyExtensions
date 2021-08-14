using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace MyExtensions
{
    public partial class ConfigEditorMk2 : Form
    {
        private string m_strSettingName;
        private List<ConfigSettings> ConfigSettingsList = new List<ConfigSettings >();

        public ConfigEditorMk2()
        {
            InitializeComponent();
            //this.dgvAppSetting.ReadOnly = false;
            this.dgvAppSetting.AutoGenerateColumns = true;
            this.dgvAppSetting.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void BtnLoadFile_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSettings();

                // Assign the collection to Datasource
                //var bindingList = new BindingList<ConfigSettings>(ConfigSettingsList);
                //var source = new BindingSource(bindingList, null);

                //bindingSource1.DataSource = source;
                bindingSource1.DataSource = ConfigSettingsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadSettings()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(MyExtensionsServer.MyAppConfig.FilePath);
            // Assign  the [applicationSettings] node 
            XmlNode userSettingsNode = xmlDoc.GetElementsByTagName("userSettings")[0];
            // For reading the user settings use below
            //Dim applicationSettingsNode As XmlNode = ConfigDoc.GetElementsByTagName("userSettings")[0];
            ConfigSettingsList.Clear();
            // Run for each child node and create the ConfigSettingsList 
            // collection of [applicationSettings] child nodes
            foreach (XmlNode objXmlNode in userSettingsNode.FirstChild.ChildNodes)
            {
                ConfigSettings objConfigSettings = new ConfigSettings
                {
                    SettingName = objXmlNode.Attributes[0].Value,
                    SerializeAs = objXmlNode.Attributes[1].Value,
                    SettingValue = objXmlNode.FirstChild.InnerText
                };
                ConfigSettingsList.Add(objConfigSettings);
            }
        }

        private bool GetSetting(ConfigSettings  objConfigSettings)
        {
            return m_strSettingName == objConfigSettings.SettingName;
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSettings();
                MessageBox.Show("Updated Successfully.", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void SaveSettings()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(MyExtensionsServer.MyAppConfig.FilePath);
            XmlNode applicationSettingsNode = xmlDoc.GetElementsByTagName("userSettings")[0];
            foreach (XmlNode objXmlNode in applicationSettingsNode.FirstChild.ChildNodes)
            {
                m_strSettingName = objXmlNode.Attributes[0].Value;
                ConfigSettings objConfigSettings = ConfigSettingsList.Find(GetSetting);

                
                if (objConfigSettings != null)
                {
                    objXmlNode.Attributes[0].Value = objConfigSettings.SettingName;
                    objXmlNode.Attributes[1].Value = objConfigSettings.SerializeAs;
                    objXmlNode.FirstChild.InnerText = objConfigSettings.SettingValue;
                }

            }

            xmlDoc.Save(MyExtensionsServer.MyAppConfig.FilePath);
        }
    }
}

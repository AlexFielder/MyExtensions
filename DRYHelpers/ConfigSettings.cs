using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRYHelpers
{
    public class ConfigSettings
    {

        private string _settingName;

        private string _settingValue;

        private string _serializeAs;

        public string SettingName
        {
            get
            {
                return _settingName;
            }
            set
            {
                _settingName = value;
            }
        }

        public string SettingValue
        {
            get
            {
                return _settingValue;
            }
            set
            {
                _settingValue = value;
            }
        }

        private bool? _settingBoolValue = true;
        public bool? SettingBoolValue
        {
            get
            {
                return _settingBoolValue;
            }
            set
            {
                _settingBoolValue = value;
            }
        }

        public string SerializeAs
        {
            get
            {
                return _serializeAs;
            }
            set
            {
                _serializeAs = value;
            }
        }

        public double SettingDoubleValue { get; internal set; }
    }
}

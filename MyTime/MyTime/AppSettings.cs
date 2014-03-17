using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldService.ViewModels;

namespace FieldService
{
    public class AppSettingsProvider
    {
        private static SettingsViewModel _appSettings = new SettingsViewModel();

        public SettingsViewModel AppSettings { get { return _appSettings; } }
    }
}

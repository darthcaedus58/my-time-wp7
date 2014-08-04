using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FieldService.Annotations;

namespace FieldService.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public SettingsViewModel()
        {
            SendMethodCollection = new ObservableCollection<string>();

            SendMethodCollection.Add(StringResources.SettingsPage_Settings_SMS);
            SendMethodCollection.Add(StringResources.SettingsPage_Settings_Email);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> SendMethodCollection { get; set; }

        public string SendMethod
        {
            get
            {
                try {
                    var st = App.AppSettingsProvider["csoEmail"].AddressType;
                    switch (st) {
                        case addressType.Email:
                            return StringResources.SettingsPage_Settings_Email;
                        case addressType.Sms:
                            return StringResources.SettingsPage_Settings_SMS;
                    }
                    return StringResources.SettingsPage_Settings_SMS;
                }
                catch {
                    SetSettingValue("csoEmail", addressType.Sms);
                    return StringResources.SettingsPage_Settings_SMS;
                }
            }

            set
            {
                if (value == StringResources.SettingsPage_Settings_SMS) {
                    SetSettingValue("csoEmail", addressType.Sms);
                    return;
                }
                if (value == StringResources.SettingsPage_Settings_Email) {
                    SetSettingValue("csoEmail", addressType.Email);
                    return;
                }
                SetSettingValue("csoEmail", addressType.Sms);
                OnPropertyChanged();
            }
        }

        public addressType SendMethodEnum
        {
            get
            {
                try {
                    return App.AppSettingsProvider["csoEmail"].AddressType;
                }
                catch {
                    return addressType.Sms;
                }
            }
            set
            {
                try {
                    App.AppSettingsProvider["csoEmail"].AddressType = value;
                    OnPropertyChanged();
                }
                catch (KeyNotFoundException e) {
                    throw e;
                }
            }
        }

        public string csContactDisplayName
        {
            get
            {
                var ds = GetSetting("csDisplayName");
                if (string.IsNullOrEmpty(ds)) return "Choose Contact";
                return ds;
            }
            set
            {
                SetSettingValue("csDisplayName", value);
                OnPropertyChanged();
            }
        }

        public string csoEmail
        {
            get { return GetSetting("csoEmail"); }
            set
            {
                SetSettingValue("csoEmail", value);
                OnPropertyChanged();
            }
        }

        public bool shareFSApp
        {
            get
            {
                try {
                    return bool.Parse(GetSetting("sharefsapp"));
                }
                catch {
                    return true;
                }
            }
            set
            {
                SetSettingValue("sharefsapp", value);
                OnPropertyChanged();
            }
        }

        public string nickname
        {
            get { return GetSetting("NickName"); }
            set
            {
                SetSettingValue("NickName", value);
                OnPropertyChanged();
            }
        }

        public int defaultAgeValue
        {
            get
            {
                try {
                    return int.Parse(GetSetting("dfltAgeValue"));
                }
                catch {
                    SetSettingValue("dftlAgeValue", 0);
                    OnPropertyChanged();
                    return 0;
                }
            }
            set
            {
                SetSettingValue("dfltAgeValue", value.ToString());
                OnPropertyChanged();
            }
        }

        public bool useLocationServices
        {
            get {
                try {
                    return bool.Parse(GetSetting("UseLocationServices"));
                }
                catch {
                    SetSettingValue("UseLocationServices", true);
                    OnPropertyChanged();
                    return true;
                } }
            set
            {
                SetSettingValue("UseLocationServices", value);
                OnPropertyChanged();
            }
        }

        public bool manuallyTrackRvs
        {
            get
            {
                try {
                    return bool.Parse(GetSetting("AddCallPlacements"));
                }
                catch {
                    SetSettingValue("AddCallPlacements", true);
                    OnPropertyChanged();
                    return true;
                }
            }
            set
            {
                SetSettingValue("AddCallPlacements", value);
                OnPropertyChanged();
            }
        }

        public bool beautifyPhoneNumbers
        {
            get
            {
                try {
                    return bool.Parse(GetSetting("beautifyPhoneNumber"));
                }
                catch {
                    SetSettingValue("beautifyPhoneNumber", true);
                    OnPropertyChanged();
                    return true;
                }
            }
            set
            {
                SetSettingValue("beautifyPhoneNumber", value);
                OnPropertyChanged();
            }
        }

        public int roundTimeIncrement
        {
            get
            {
                try {
                    return int.Parse(GetSetting("roundTimeIncrement"));
                }
                catch {
                    SetSettingValue("roundTimeIncrement", 15);
                    OnPropertyChanged();
                    return 15;
                }
            }
            set
            {
                if (value > 60)
                    value = 60;
                if (value <= 0)
                    value = 1;
                SetSettingValue("roundTimeIncrement", value);
                OnPropertyChanged();
            }
        }

        public bool deleteCallsAndRv
        {
            get
            {
                try {
                    return bool.Parse(GetSetting("deleteCallsAndRV"));
                }
                catch {
                    SetSettingValue("deleteCallsAndRV", false);
                    OnPropertyChanged();
                    return false;
                }
            }
            set
            {
                SetSettingValue("deleteCallsAndRV", value);
                OnPropertyChanged();
            }
        }

        public bool askForDonation
        {
            get
            {
                try {
                    return bool.Parse(GetSetting("askForDonation"));
                }
                catch {
                    SetSettingValue("askForDonation", true);
                    OnPropertyChanged();
                    return true;
                }
            }
            set
            {
                SetSettingValue("askForDonation", value);
                OnPropertyChanged();
            }
        }

        public string howToShownVer
        {
            get { return GetSetting("howToShownVer"); }
            set
            {
                SetSettingValue("howToShownVer", value);
                OnPropertyChanged();
            }
        }

        public bool UseCustomDTUrl
        {
            get { 
                try {
                    return bool.Parse(GetSetting("UseCustomDTUrl")) && (!string.IsNullOrEmpty(GetSetting("CustomDT_Lang1")) && !string.IsNullOrEmpty(GetSetting("CustomDT_Lang2")) && !string.IsNullOrEmpty(GetSetting("CustomDT_RType")));
                }
                catch {
                    SetSettingValue("UseCustomDTUrl", true);
                    OnPropertyChanged();
                    return true;
                } 
            }
            set
            {
                if (value) {
                    value = (!string.IsNullOrEmpty(GetSetting("CustomDT_Lang1")) && !string.IsNullOrEmpty(GetSetting("CustomDT_Lang2")) && !string.IsNullOrEmpty(GetSetting("CustomDT_RType")));
                    if (!value)
                        App.ToastMe(StringResources.SettingsPage_Settings_DailyTextErrorUrlNotSet);
                }
                SetSettingValue("UseCustomDTUrl", value);
                OnPropertyChanged();
            }
        }

        public string CustomDTUrl
        {
            get
            {
                return (!string.IsNullOrEmpty(GetSetting("CustomDT_Lang1")) && !string.IsNullOrEmpty(GetSetting("CustomDT_Lang2")) && !string.IsNullOrEmpty(GetSetting("CustomDT_RType")))
                    ? string.Format(StringResources.Application_CustomDailyTextURL, 
                        GetSetting("CustomDT_Lang1"), GetSetting("CustomDT_RType"), GetSetting("CustomDT_Lang2"), 
                        DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day)
                    : string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;
                string dtUrlPattern =
                    @"^http\://(?:m\.)?wol\.jw\.org/(?<lang1>[\w\-]+)/wol/dt/(?<rtype>r\d+)/(?<lang2>lp\-\w+)/\d{4}/\d{1,2}/\d{1,2}$";

                var qry = Regex.Match(value, dtUrlPattern); 

                if (qry.Success && qry.Groups.Count == 4) {
                    SetSettingValue("CustomDT_Lang1", qry.Groups["lang1"].Value);
                    SetSettingValue("CustomDT_Lang2", qry.Groups["lang2"].Value);
                    SetSettingValue("CustomDT_RType", qry.Groups["rtype"].Value);
                }
                else {
                    SetSettingValue("CustomDT_Lang1",string.Empty);
                    SetSettingValue("CustomDT_Lang2",string.Empty);
                    SetSettingValue("CustomDT_RType",string.Empty);
                    UseCustomDTUrl = false;
                    App.ToastMe(StringResources.SettingsPage_Settings_DailyTextErrorUrl);
                }

                OnPropertyChanged();
            }
        }

        private string GetSetting(string settingKey)
        {
            try {
                return App.AppSettingsProvider[settingKey].Value;
            }
            catch (KeyNotFoundException e) {
                throw e;
            }
        }

        private void SetSettingValue(string settingKey, string value)
        {
            try {
                App.AppSettingsProvider[settingKey].Value = value;
                App.AppSettingsProvider.SaveSettings();
            }
            catch (KeyNotFoundException e) {
                throw e;
            }
        }

        private void SetSettingValue(string settingKey, bool value)
        {
            SetSettingValue(settingKey, value ? bool.TrueString : bool.FalseString);
        }

        private void SetSettingValue(string settingKey, int value)
        {
            SetSettingValue(settingKey, value.ToString());
        }

        private void SetSettingValue(string settingKey, addressType value)
        {
            try {
                App.AppSettingsProvider[settingKey].AddressType = value;
                App.AppSettingsProvider.SaveSettings();
            }
            catch (KeyNotFoundException e) {
                throw e;
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

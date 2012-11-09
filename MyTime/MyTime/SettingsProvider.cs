using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using MvvmSettings;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

namespace MyTime
{

    public class SettingsProvider
    {


        internal class EnumValueConverter : IValueConverter
        {
            /// <summary>
            /// Modifies the source data before passing it to the target for display in the UI.
            /// </summary>
            /// <returns>
            /// The value to be passed to the target dependency property.
            /// </returns>
            /// <param name="value">The source data being passed to the target.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the target dependency property.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null) return null;

                string scopeString = value.ToString();

                var enumValue = (addressType) Enum.Parse(typeof (addressType), scopeString, true);
                return scopeString;
            }

            /// <summary>
            /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay"/> bindings.
            /// </summary>
            /// <returns>
            /// The value to be passed to the source object.
            /// </returns>
            /// <param name="value">The target data being passed to the source.</param><param name="targetType">The <see cref="T:System.Type"/> of data expected by the source object.</param><param name="parameter">An optional parameter to be used in the converter logic.</param><param name="culture">The culture of the conversion.</param>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string scopeString = value.ToString();
                var enumValue = (addressType) Enum.Parse(typeof (addressType), scopeString, true);
                return enumValue;
            }
        }

        private Setting _contactChooser;
       

        public SettingsProvider()
        {
            LoadSettings();
        }

        private Settings Settings { get; set; }

        private void LoadSettings()
        {
#if DEBUG
            IsolatedStorageFile.GetUserStoreForApplication().DeleteFile("settings.xml");
#endif
            this.Settings = this.ReadFromFile(IsolatedStorageFile.GetUserStoreForApplication(), "settings.xml");
        }

        public Setting this[string key]
        {
            get
            {
                try {
                    var ss = Settings.SettingsCollection.Single(s => s.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                    return ss;
                } catch {
                    return null;
                }
            }
            set
            {
                try {
                    var ss = Settings.SettingsCollection.Single(s => s.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                    if (ss != null) ss.Value = value.ToString();
                    else throw new NullReferenceException("Key not found");
                } catch (Exception e) {
                    throw e;
                }
            }
        }


        protected Settings ReadFromFile(IsolatedStorageFile isoStore, string fileName)
        {
            try {
                if (isoStore.FileExists(fileName)) {
                    XmlSerializer ser = new XmlSerializer(typeof(Settings), "http://www.inputstudiowp7.com/schemas");
                    using (StreamReader reader = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))) {
                        //buffer used to remove extra characters added by serializer?
                        string buffer = reader.ReadToEnd();
                        buffer = buffer.Substring(0, buffer.IndexOf("/Settings>") + "/Settings>".Length);
                        //using (XmlReader rdr = XmlReader.Create(new StringReader(reader.ReadToEnd())))
                        using (XmlReader rdr = XmlReader.Create(new StringReader(buffer))) {
                            return (Settings)ser.Deserialize(rdr);
                        }
                    }
                } else {
                    Settings settings = (Settings)new XmlSerializer(typeof(Settings))
                               .Deserialize(XmlReader.Create("Model/settings.xml"));
                    this.WriteToFile(isoStore, settings, "settings.xml");
                    return settings;
                }
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK);
            }

            return null;
        }

        protected bool WriteToFile(IsolatedStorageFile isoStore, Settings settings, string fileName)
        {
            try {
                XmlSerializer ser = new XmlSerializer(typeof(Settings), "http://www.inputstudiowp7.com/schemas");
                using (StreamWriter writer = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isoStore))) {
                    XmlWriterSettings setts = new XmlWriterSettings();
                    setts.Indent = true;
                    using (XmlWriter wtr = XmlWriter.Create(writer, setts)) {
                        ser.Serialize(wtr, settings);
                    }

                    writer.Close();
                    return true;
                }
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK);
            }

            return false;
        }

        public bool SaveSettings()
        {
            return this.WriteToFile(IsolatedStorageFile.GetUserStoreForApplication(), this.Settings, "settings.xml");
        }

        public StackPanel BuildXaml() { return BuildXaml<StackPanel>(); }

        public T BuildXaml<T>() where T : new()
        {
            T spContent = new T();
            
            if (!spContent.GetType().GetMember("Children").Any()) {
                throw new TypeLoadException("Type doesn't have children!");
            }
            var children = spContent.GetType().GetProperty("Children");

            var spContentChildren = (UIElementCollection)children.GetGetMethod().Invoke(spContent, new object[]{});
            foreach (Setting setting in Settings.SettingsCollection) {
                if (!setting.ShowInSettingsPage) continue;
                TextBlock tbCaption = MakeCaption(setting);
                switch (setting.Type) {
                    case eDataType.Header:
                        var tbH = MakeHeader(tbCaption, setting);
                        spContentChildren.Add(tbH);
                        break;
                    case eDataType.Note:
                        var tbN = MakeNote(tbCaption, setting);
                        spContentChildren.Add(tbN);
                        break;
                    case eDataType.Boolean:
                        var ts = MakeToggleSwitch(setting);
                        spContentChildren.Add(ts);
                        break;
                    case eDataType.String:
                    case eDataType.Integer:
                    case eDataType.Decimal:
                        var txtValue = MakeTextBox(setting);
                        spContentChildren.Add(tbCaption);
                        spContentChildren.Add(txtValue);
                        break;
                    case eDataType.StringArray:
                        var lpValue = MakeListPicker(setting);
                        spContentChildren.Add(lpValue);
                        break;
                    case eDataType.ContactChooser:
                        StackPanel p;
                        MakeContactChooser(setting, out p);
                        if (p == null) continue; 
                        spContentChildren.Add(p);
                        break;
                }
            }
            return spContent;
        }

        private void MakeContactChooser(Setting setting, out StackPanel p)
        {
            try {
                p = new StackPanel() {
                                         Orientation = Orientation.Vertical
                                     };
                
                HyperlinkButton bButton;
                ListPicker lbAddressType;
                TextBox tbCc;
                TextBlock tbFriendlyName = MakeCaption(setting);
                if (string.IsNullOrEmpty(setting.ContactDisplayName)) setting.ContactDisplayName = "Choose Contact";
                bButton = new HyperlinkButton()
                {
                    HorizontalAlignment= HorizontalAlignment.Left,
                    Margin = new Thickness(-5, 0,0,0)
                };
                bButton.SetBinding(HyperlinkButton.ContentProperty, new Binding("ContactDisplayName") { Mode = BindingMode.TwoWay, Source = setting });
                bButton.Click += new RoutedEventHandler(bButton_Click);
                p.Children.Add(tbFriendlyName);
                p.Children.Add(bButton);

                tbCc = new TextBox()
                {
                    Name = String.Format("txt{0}", setting.Name.Replace(" ", String.Empty)),
                    Height = 72,
                    MaxLength = 50,
                    TextWrapping = TextWrapping.NoWrap
                };
                tbCc.SetBinding(TextBox.TextProperty, new Binding("Value") { Mode = BindingMode.TwoWay, Source = setting });

                tbCc.InputScope = new InputScope() {
                                                       Names = {
                                                                   new InputScopeName() {
                                                                                            NameValue = (setting.AddressType == addressType.Email 
                                                                                                ? 
                                                                                                    InputScopeNameValue.EmailNameOrAddress :
                                                                                                    InputScopeNameValue.TelephoneNumber)
                                                                                        }
                                                               }
                    
                               };
                
                lbAddressType = new ListPicker()
                {
                    Header = "DEFAULT SEND TYPE"
                };
                var fields = (typeof (addressType)).GetFields().Skip(1);
                lbAddressType.ItemsSource = from field in fields select field.Name;
                lbAddressType.SetBinding(ListPicker.SelectedItemProperty, new Binding("AddressType") { Converter = new EnumValueConverter(), Mode = BindingMode.TwoWay, Source = setting});
                p.Children.Add(tbCc);
                p.Children.Add(lbAddressType);
            } catch (Exception e) {
                p = null;
            }
        }

        private void bButton_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton b = ((HyperlinkButton) sender);
            var be = b.GetBindingExpression(HyperlinkButton.ContentProperty);
            _contactChooser = (Setting) be.DataItem;
            if (_contactChooser.AddressType == addressType.Email) {
                var email = new EmailAddressChooserTask();
                email.Completed += new EventHandler<EmailResult>(email_Completed);
                email.Show();
            } else {
                var sms = new PhoneNumberChooserTask();
                sms.Completed += new EventHandler<PhoneNumberResult>(sms_Completed);
                sms.Show();
            }
        }

        private void sms_Completed(object sender, PhoneNumberResult e)
        {
            if (_contactChooser == null) return;
            if (e.TaskResult != TaskResult.OK) {
                _contactChooser = null;
                return;
            }
            _contactChooser.ContactDisplayName = e.DisplayName;
            _contactChooser.Value = e.PhoneNumber;
            SaveSettings();

            _contactChooser = null;
        }

        private void email_Completed(object sender, EmailResult e)
        {
            if (_contactChooser == null) return;
            if (e.TaskResult != TaskResult.OK) {
                _contactChooser = null;
                return;
            }
            _contactChooser.ContactDisplayName = e.DisplayName;
            _contactChooser.Value = e.Email;
            SaveSettings();

            _contactChooser = null;
        }

        private static TextBlock MakeCaption(Setting setting) { return new TextBlock() {Text = String.Format("{0}", setting.FriendlyName.Trim()), Margin = new Thickness(12, 20, 0, 0), TextWrapping = TextWrapping.Wrap}; }

        private static ListPicker MakeListPicker(Setting setting)
        {
            ListPicker lpValue = new ListPicker() {
                                                      Header = String.Format("{0}", setting.FriendlyName.Trim())
                                                  };

            List<string> items = new List<string>();
            foreach (StringItem item in setting.StringItem)
                items.Add(item.Value);

            lpValue.ItemsSource = items;
            lpValue.SetBinding(ListPicker.SelectedItemProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            return lpValue;
        }

        private static TextBox MakeTextBox(Setting setting)
        {
            TextBox txtValue = new TextBox() {
                                                 Name = String.Format("txt{0}", setting.Name.Replace(" ", String.Empty)),
                                                 Height = 72,
                                                 MaxLength = 50,
                                                 TextWrapping = TextWrapping.NoWrap
                                             };
            txtValue.SetBinding(TextBox.TextProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            txtValue.InputScope = new InputScope() {
                                                       Names = {
                                                                   new InputScopeName() {
                                                                                            NameValue = (setting.Type.Equals(eDataType.String)
                                                                                                             ? InputScopeNameValue.Text
                                                                                                             : InputScopeNameValue.Number)
                                                                                        }
                                                               }
                                                   };
            return txtValue;
        }

        private static ToggleSwitch MakeToggleSwitch(Setting setting)
        {
            ToggleSwitch tsOption = new ToggleSwitch() {
                                                           Name = String.Format("ts{0}", setting.Name.Replace(" ", String.Empty)), Header = setting.FriendlyName
                                                       };
            tsOption.SetBinding(ToggleSwitch.IsCheckedProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            return tsOption;
        }

        private static TextBlock MakeNote(TextBlock tbCaption, Setting setting)
        {
            Style sNote = new Style(typeof (TextBlock));
            sNote.Setters.Add(new Setter(FrameworkElement.StyleProperty, new Binding("PhoneTextSubtleStyle")));
            tbCaption.Style = sNote;
            tbCaption.Text = setting.Value.Trim();
            tbCaption.Margin = new Thickness(12, 0, 0, 0);
            return tbCaption;
        }

        private static TextBlock MakeHeader(TextBlock tbCaption, Setting setting)
        {
            Style sHdr = new Style(typeof (TextBlock));
            sHdr.Setters.Add(new Setter(FrameworkElement.StyleProperty, new Binding("PhoneTextLargeStyle")));
            tbCaption.Style = sHdr;
            tbCaption.Text = setting.Value.Trim().ToUpper();
            tbCaption.Margin = new Thickness(12, 40, 0, 0);
            return tbCaption;
        }
    }
}

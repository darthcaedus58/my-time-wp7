// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-08-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-08-2012
// ***********************************************************************
// <copyright file="SettingsProvider.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;

namespace FieldService
{
    /// <summary>
    /// Class SettingsProvider
    /// </summary>
    public class SettingsProvider
    {
        /// <summary>
        /// The _contact chooser
        /// </summary>
        private Setting _contactChooser;


        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsProvider" /> class.
        /// </summary>
        public SettingsProvider() { LoadSettings(); }

        /// <summary>
        /// Gets or sets the settings.
        /// </summary>
        /// <value>The settings.</value>
        private Settings Settings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="FieldService.Setting" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Setting.</returns>
        /// <exception cref="System.NullReferenceException">Key not found</exception>
        public Setting this[string key]
        {
            get
            {
                try {
                    Setting ss = Settings.SettingsCollection.Single(s => s.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                    return ss;
                } catch {
                    return null;
                }
            }
            set
            {
                Setting ss = Settings.SettingsCollection.Single(s => s.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase));
                if (ss != null) ss.Value = value.ToString();
                else throw new NullReferenceException("Key not found");
            }
        }

        #region Events

        /// <summary>
        /// Handles the Click event of the bButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void bButton_Click(object sender, RoutedEventArgs e)
        {
            var b = ((HyperlinkButton) sender);
            BindingExpression be = b.GetBindingExpression(ContentControl.ContentProperty);
            if (be != null) _contactChooser = (Setting) be.DataItem;
            if (_contactChooser.AddressType == addressType.Email) {
                var email = new EmailAddressChooserTask();
                email.Completed += email_Completed;
                email.Show();
            } else {
                var sms = new PhoneNumberChooserTask();
                sms.Completed += sms_Completed;
                sms.Show();
            }
        }

        /// <summary>
        /// Email_s the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        /// <summary>
        /// SMS_s the completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
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

        #endregion

        /// <summary>
        /// Loads the settings.
        /// </summary>
        private void LoadSettings()
        {
            Settings = ReadFromFile(IsolatedStorageFile.GetUserStoreForApplication(), "settings.xml");
        }


        /// <summary>
        /// Reads from file.
        /// </summary>
        /// <param name="isoStore">The iso store.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Settings.</returns>
        protected Settings ReadFromFile(IsolatedStorageFile isoStore, string fileName)
        {
            try {
                if (isoStore.FileExists(fileName)) {
                    var ser = new XmlSerializer(typeof (Settings), "http://www.inputstudiowp7.com/schemas");
                    Settings Current;
                    using (var reader = new StreamReader(new IsolatedStorageFileStream(fileName, FileMode.Open, isoStore))) {
                        //buffer used to remove extra characters added by serializer?
                        string buffer = reader.ReadToEnd();
                        buffer = buffer.Substring(0, buffer.IndexOf("/Settings>", StringComparison.Ordinal) + "/Settings>".Length);
                        //using (XmlReader rdr = XmlReader.Create(new StringReader(reader.ReadToEnd())))
                        using (XmlReader rdr = XmlReader.Create(new StringReader(buffer))) {
                            var current = (Settings) ser.Deserialize(rdr);
                            Current = CheckSettingsUpgrade(isoStore, current); //check for upgraded setttings
                        }
                    }
                    WriteToFile(isoStore, Current, "settings.xml"); //upgrade the settings file
                    return Current;
                }
                var settings = (Settings) new XmlSerializer(typeof (Settings))
                                              .Deserialize(XmlReader.Create("Model/settings.xml"));
                WriteToFile(isoStore, settings, "settings.xml");
                return settings;
            } catch (Exception e) {
                MessageBox.Show(e.StackTrace, e.Message, MessageBoxButton.OK);
            }

            return null;
        }

        private Settings CheckSettingsUpgrade(IsolatedStorageFile isoStore, Settings current)
        {
            var previous = (Settings) new XmlSerializer(typeof (Settings)).Deserialize(XmlReader.Create("Model/settings.xml"));
            if (current.SettingsCollection == null) {
                return previous;
            }
            foreach (var s in previous.SettingsCollection) {
                bool found = false;
                foreach (var c in current.SettingsCollection) {
                    if (s.Name.Equals(c.Name)) {
                        found = true;
                        c.FriendlyName = s.FriendlyName;
                        c.ShowInSettingsPage = s.ShowInSettingsPage;
                        c.Type = s.Type;
                        if (c.Type == eDataType.Header || c.Type == eDataType.Note)
                            c.Value = s.Value;
                        break;
                    }
                }
                if (!found) current.SettingsCollection.Add(s);
            }
            return current;
        }

        /// <summary>
        /// Writes to file.
        /// </summary>
        /// <param name="isoStore">The iso store.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="fileName">Name of the file.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        protected bool WriteToFile(IsolatedStorageFile isoStore, Settings settings, string fileName)
        {
            try {
                var ser = new XmlSerializer(typeof (Settings), "http://www.inputstudiowp7.com/schemas");
                using (var writer = new StreamWriter(new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isoStore))) {
                    var setts = new XmlWriterSettings {Indent = true};
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

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        public bool SaveSettings() { return WriteToFile(IsolatedStorageFile.GetUserStoreForApplication(), Settings, "settings.xml"); }

        /// <summary>
        /// Builds the xaml.
        /// </summary>
        /// <returns>StackPanel.</returns>
        public StackPanel BuildXaml() { return BuildXaml<StackPanel>(); }

        /// <summary>
        /// Builds the xaml.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>``0.</returns>
        /// <exception cref="System.TypeLoadException">Type doesn't have children!</exception>
        public T BuildXaml<T>() where T : new()
        {
            var spContent = new T();

            if (!spContent.GetType().GetMember("Children").Any()) {
                throw new TypeLoadException("Type doesn't have children!");
            }
            PropertyInfo children = spContent.GetType().GetProperty("Children");

            var spContentChildren = (UIElementCollection) children.GetGetMethod().Invoke(spContent, new object[] {});
            foreach (Setting setting in Settings.SettingsCollection) {
                if (!setting.ShowInSettingsPage) continue;
                TextBlock tbCaption = MakeCaption(setting);
                switch (setting.Type) {
                    case eDataType.Header:
                        TextBlock tbH = MakeHeader(tbCaption, setting);
                        spContentChildren.Add(tbH);
                        break;
                    case eDataType.Note:
                        TextBlock tbN = MakeNote(tbCaption, setting);
                        spContentChildren.Add(tbN);
                        break;
                    case eDataType.Boolean:
                        ToggleSwitch ts = MakeToggleSwitch(setting);
                        spContentChildren.Add(ts);
                        break;
                    case eDataType.String:
                    case eDataType.Integer:
                    case eDataType.Decimal:
                        TextBox txtValue = MakeTextBox(setting);
                        spContentChildren.Add(tbCaption);
                        spContentChildren.Add(txtValue);
                        break;
                    case eDataType.StringArray:
                        ListPicker lpValue = MakeListPicker(setting);
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

        /// <summary>
        /// Makes the contact chooser.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <param name="p">The p.</param>
        private void MakeContactChooser(Setting setting, out StackPanel p)
        {
            try {
                p = new StackPanel {
                                       Orientation = Orientation.Vertical
                                   };

                TextBlock tbFriendlyName = MakeCaption(setting);
                if (string.IsNullOrEmpty(setting.ContactDisplayName)) setting.ContactDisplayName = "Choose Contact";
                var bButton = new HyperlinkButton {
                                                      HorizontalAlignment = HorizontalAlignment.Left,
                                                      Margin = new Thickness(-5, 0, 0, 0)
                                                  };
                bButton.SetBinding(ContentControl.ContentProperty, new Binding("ContactDisplayName") {Mode = BindingMode.TwoWay, Source = setting});
                bButton.Click += bButton_Click;
                p.Children.Add(tbFriendlyName);
                p.Children.Add(bButton);

                var tbCc = new TextBox {
                                           Name = String.Format((string) "txt{0}", (object) setting.Name.Replace(" ", String.Empty)),
                                           Height = 72,
                                           MaxLength = 50,
                                           TextWrapping = TextWrapping.NoWrap
                                       };
                tbCc.SetBinding(TextBox.TextProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});

                tbCc.InputScope = new InputScope {
                                                     Names = {
                                                                 new InputScopeName {
                                                                                        NameValue = (setting.AddressType == addressType.Email
                                                                                                         ? InputScopeNameValue.EmailNameOrAddress :
                                                                                                                                                      InputScopeNameValue.TelephoneNumber)
                                                                                    }
                                                             }
                                                 };

                var lbAddressType = new ListPicker {
                                                       Header = "DEFAULT SEND TYPE"
                                                   };
                IEnumerable<FieldInfo> fields = (typeof (addressType)).GetFields().Skip(1);
                lbAddressType.ItemsSource = from field in fields select field.Name;
                lbAddressType.SetBinding(ListPicker.SelectedItemProperty, new Binding("AddressType") {Converter = new EnumValueConverter(), Mode = BindingMode.TwoWay, Source = setting});
                p.Children.Add(tbCc);
                p.Children.Add(lbAddressType);
            } catch (Exception e) {
                p = null;
            }
        }

        /// <summary>
        /// Makes the caption.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>TextBlock.</returns>
        private static TextBlock MakeCaption(Setting setting) { return new TextBlock {Text = String.Format((string) "{0}", (object) setting.FriendlyName.Trim()), Margin = new Thickness(12, 20, 0, 0), TextWrapping = TextWrapping.Wrap}; }

        /// <summary>
        /// Makes the list picker.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>ListPicker.</returns>
        private static ListPicker MakeListPicker(Setting setting)
        {
            var lpValue = new ListPicker {
                                             Header = String.Format((string) "{0}", (object) setting.FriendlyName.Trim())
                                         };

            var items = Enumerable.ToList<string>(setting.StringItem.Select(item => item.Value));

            lpValue.ItemsSource = items;
            lpValue.SetBinding(ListPicker.SelectedItemProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            return lpValue;
        }

        /// <summary>
        /// Makes the text box.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>TextBox.</returns>
        private static TextBox MakeTextBox(Setting setting)
        {
            var txtValue = new TextBox {
                                           Name = String.Format((string) "txt{0}", (object) setting.Name.Replace(" ", String.Empty)),
                                           Height = 72,
                                           MaxLength = 50,
                                           TextWrapping = TextWrapping.NoWrap
                                       };
            txtValue.SetBinding(TextBox.TextProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            txtValue.InputScope = new InputScope {
                                                     Names = {
                                                                 new InputScopeName {
                                                                                        NameValue = (setting.Type.Equals(eDataType.String)
                                                                                                         ? InputScopeNameValue.Text
                                                                                                         : InputScopeNameValue.Number)
                                                                                    }
                                                             }
                                                 };
            return txtValue;
        }

        /// <summary>
        /// Makes the toggle switch.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns>ToggleSwitch.</returns>
        private static ToggleSwitch MakeToggleSwitch(Setting setting)
        {
            var tsOption = new ToggleSwitch {
                                                Name = String.Format((string) "ts{0}", (object) setting.Name.Replace(" ", String.Empty)), Header = setting.FriendlyName
                                            };
            tsOption.SetBinding(ToggleSwitch.IsCheckedProperty, new Binding("Value") {Mode = BindingMode.TwoWay, Source = setting});
            return tsOption;
        }

        /// <summary>
        /// Makes the note.
        /// </summary>
        /// <param name="tbCaption">The tb caption.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>TextBlock.</returns>
        private static TextBlock MakeNote(TextBlock tbCaption, Setting setting)
        {
            var sNote = new Style(typeof (TextBlock));
            sNote.Setters.Add(new Setter(FrameworkElement.StyleProperty, new Binding("PhoneTextSubtleStyle")));
            tbCaption.Style = sNote;
            tbCaption.Text = setting.Value.Trim();
            tbCaption.Margin = new Thickness(12, 0, 0, 0);
            return tbCaption;
        }

        /// <summary>
        /// Makes the header.
        /// </summary>
        /// <param name="tbCaption">The tb caption.</param>
        /// <param name="setting">The setting.</param>
        /// <returns>TextBlock.</returns>
        private static TextBlock MakeHeader(TextBlock tbCaption, Setting setting)
        {
            var sHdr = new Style(typeof (TextBlock));
            sHdr.Setters.Add(new Setter(FrameworkElement.StyleProperty, new Binding("PhoneTextLargeStyle")));
            tbCaption.Style = sHdr;
            tbCaption.Text = setting.Value.Trim().ToUpper();
            tbCaption.Margin = new Thickness(12, 40, 0, 0);
            return tbCaption;
        }

        #region Nested type: EnumValueConverter

        /// <summary>
        /// Class EnumValueConverter
        /// </summary>
        public class EnumValueConverter : IValueConverter
        {
            #region IValueConverter Members

            /// <summary>
            /// Modifies the source data before passing it to the target for display in the UI.
            /// </summary>
            /// <param name="value">The source data being passed to the target.</param>
            /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the target dependency property.</param>
            /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
            /// <param name="culture">The culture of the conversion.</param>
            /// <returns>The value to be passed to the target dependency property.</returns>
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null) return null;

                string scopeString = value.ToString();

                return scopeString;
            }

            /// <summary>
            /// Modifies the target data before passing it to the source object.  This method is called only in <see cref="F:System.Windows.Data.BindingMode.TwoWay" /> bindings.
            /// </summary>
            /// <param name="value">The target data being passed to the source.</param>
            /// <param name="targetType">The <see cref="T:System.Type" /> of data expected by the source object.</param>
            /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
            /// <param name="culture">The culture of the conversion.</param>
            /// <returns>The value to be passed to the source object.</returns>
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                string scopeString = value.ToString();
                var enumValue = (addressType) Enum.Parse(typeof (addressType), scopeString, true);
                return enumValue;
            }

            #endregion
        }

        #endregion
    }
}
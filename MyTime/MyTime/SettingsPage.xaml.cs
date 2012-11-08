using Microsoft.Phone.Controls;
using MvvmSettings.ViewModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;

namespace MyTime
{
    /// <summary>
    /// Description for Settings.
    /// </summary>
    public partial class SettingsPage : PhoneApplicationPage
    {
        public MainViewModel model { get { return this.DataContext as MainViewModel; } }

        /// <summary>
        /// Initializes a new instance of the Settings class.
        /// </summary>
        public SettingsPage()
        {
            DataContext = App.ViewModel;
            InitializeComponent();
            this.Loaded += new System.Windows.RoutedEventHandler(Settings_Loaded);
        }

        void Settings_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = this.FindName("SettingsRoot") as Grid;
            if (null != grid)
            {
                StackPanel spContent = new StackPanel();
                foreach (Setting setting in model.Settings.Setting)
                {
                    TextBlock tbCaption = new TextBlock() { Text = string.Format("{0}", setting.Name.Trim()), Margin = new Thickness(12, 20, 0, 0), TextWrapping= TextWrapping.Wrap };
                    switch (setting.Type)
                    {
                        case eDataType.Header:
                            Style sHdr = new Style(typeof(TextBlock));
                            sHdr.Setters.Add(new Setter(StyleProperty, new Binding("PhoneTextLargeStyle")));
                            tbCaption.Style = sHdr;
                            tbCaption.Text = setting.Value.Trim().ToUpper();
                            tbCaption.Margin = new Thickness(12, 40, 0, 0);
                            spContent.Children.Add(tbCaption);
                            break;
                        case eDataType.Note:
                            Style sNote = new Style(typeof(TextBlock));
                            sNote.Setters.Add(new Setter(StyleProperty, new Binding("PhoneTextSubtleStyle")));
                            tbCaption.Style = sNote;
                            tbCaption.Text = setting.Value.Trim();
                            tbCaption.Margin = new Thickness(12, 0, 0, 0);
                            spContent.Children.Add(tbCaption);
                            break;
                        case eDataType.Boolean:
                            ToggleSwitch tsOption = new ToggleSwitch() 
                            { 
                                Name = string.Format("ts{0}", setting.Name.Replace(" ", string.Empty)), Header = setting.Name 
                            };
                            tsOption.SetBinding(ToggleSwitch.IsCheckedProperty, new Binding("Value") { Mode = BindingMode.TwoWay, Source = setting });
                            spContent.Children.Add(tsOption);
                            break;
                        case eDataType.String:
                        case eDataType.Integer:
                        case eDataType.Decimal:
                            TextBox txtValue = new TextBox()
                            {
                                Name = string.Format("txt{0}", setting.Name.Replace(" ", string.Empty)),
                                Height = 72,
                                MaxLength = 50,
                                TextWrapping = TextWrapping.NoWrap
                            };
                            txtValue.SetBinding(TextBox.TextProperty, new Binding("Value") { Mode = BindingMode.TwoWay, Source = setting });
                            txtValue.InputScope = new InputScope()
                            {
                                Names = { new InputScopeName() { NameValue = (setting.Type.Equals(eDataType.String) 
                                    ? InputScopeNameValue.Text
                                    : InputScopeNameValue.Number) } }
                            };

                            spContent.Children.Add(tbCaption);
                            spContent.Children.Add(txtValue);
                            break;
                        case eDataType.StringArray:
                            ListPicker lpValue = new ListPicker()
                            {
                                Header = string.Format("{0}", setting.Name.Trim())
                            };

                            List<string> items = new List<string>();
                            foreach (StringItem item in setting.StringItem)
                                items.Add(item.Value);

                                lpValue.ItemsSource = items;
                                lpValue.SetBinding(ListPicker.SelectedItemProperty, new Binding("Value") { Mode = BindingMode.TwoWay, Source = setting });
                                spContent.Children.Add(lpValue);
                            break;
                    }
                } 
                grid.Children.Add(spContent);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            model.saveSettings();

            base.OnNavigatedFrom(e);
        }
    }
}
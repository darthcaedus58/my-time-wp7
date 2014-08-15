using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace FieldService.View
{
	public partial class ViewPreviousVisit : PhoneApplicationPage
	{
		public ViewPreviousVisit()
		{
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (!NavigationContext.QueryString.ContainsKey("id") || !NavigationContext.QueryString.ContainsKey("rvid")) {
				return;
			}

			int id, rvid;
			try {
				if (!int.TryParse(NavigationContext.QueryString["id"], out id) || !int.TryParse(NavigationContext.QueryString["rvid"], out rvid)) return;
				(DataContext as PreviousVisitViewModel).PreviousVisitItemId = id;
			} catch (Exception ee) {
				NavigationService.GoBack();
			}
		}

		private void appbar_save_Click_1(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(string.Format("/View/PreviousCall.xaml?id={0}&rvid={1}",
			                                                 (DataContext as PreviousVisitViewModel).PreviousVisitItemId,
			                                                 (DataContext as PreviousVisitViewModel).ReturnVisitItemId), UriKind.Relative));
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			var edit = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
			if (edit != null) {
				edit.Text = StringResources.ViewPreviousVisitPage_Menu_Edit;
			}
		}
	}
}
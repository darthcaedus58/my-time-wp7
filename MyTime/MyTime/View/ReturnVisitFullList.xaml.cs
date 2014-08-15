// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-10-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-11-2012
// ***********************************************************************
// <copyright file="ReturnVisitFullList.xaml.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;
using Telerik.Windows.Controls;

namespace FieldService.View
{
	/// <summary>
	/// Class ReturnVistFullList
	/// </summary>
	public partial class ReturnVistFullList : PhoneApplicationPage
	{
		private ReturnVisitFullListViewModel ViewModel { get { return ((ReturnVisitFullListViewModel) DataContext); } }
		/// <summary>
		/// Initializes a new instance of the <see cref="ReturnVistFullList" /> class.
		/// </summary>
		public ReturnVistFullList()
		{
            this.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name);
		        //DataContext = new ReturnVisitFullListViewModel();
			InitializeComponent();
		}

	        private async void UpdateModel()
	        {
	                var bw = new BackgroundWorker();

                        bw.RunWorkerCompleted += (o,e) =>
                        {
                                if(llsAllReturnVisits.ItemsSource != null) llsAllReturnVisits.ItemsSource.Clear();
                                var rvList = ViewModel.LoadReturnVisitFullList();
                                llsAllReturnVisits.ItemsSource = rvList;

                                racbRvSearchBox.SuggestionsSource = BindSuggestionSource(rvList);
                                racbRvSearchBox.SuggestionSelected += racbRvSearchBox_OnSuggestionSelected;
                                racbRvSearchBox.GotFocus += (sender, args) => {
                                        racbRvSearchBox.SelectAll();
                                };
                                racbRvSearchBox.FilterKeyProvider = (object item) => {
                                        var typedItem = item as ReturnVisitLLItemModel;
                                        return string.Format(
                                                "{0} {1} {2}", typedItem.Text, typedItem.Address1, typedItem.Address2);
                                };
                                racbRvSearchBox.IsEnabled = true;
                        };

                        bw.RunWorkerAsync();
	        }

	        private IEnumerable BindSuggestionSource(List<ReturnVisitFullListViewModel.Group<ReturnVisitLLItemModel>> rvList)
	        {
	                var retList = new List<ReturnVisitLLItemModel>();
	                if (rvList != null) 
                                foreach (var rvgroup in rvList) {
	                                retList.AddRange(rvgroup);
	                        }
	                return retList;
	        }

	        private void racbRvSearchBox_OnSuggestionSelected(object sender, SuggestionSelectedEventArgs e)
	        {
                        var returnVisitLlItemModel = e.SelectedSuggestion as ReturnVisitLLItemModel;
                        if (returnVisitLlItemModel != null) {
                                NavigationService.Navigate(new Uri(string.Format("/View/EditReturnVisit.xaml?id={0}", returnVisitLlItemModel.ItemId), UriKind.Relative));
                        }
	        }

	        #region Events

		/// <summary>
		/// Handles the SelectionChanged event of the llsAllReturnVisits control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
		private void llsAllReturnVisits_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
                        
			var returnVisitLlItemModel = llsAllReturnVisits.SelectedItem as ReturnVisitLLItemModel;
			if (returnVisitLlItemModel != null) {
				NavigationService.Navigate(new Uri(string.Format("/View/EditReturnVisit.xaml?id={0}", returnVisitLlItemModel.ItemId), UriKind.Relative));
			}
		        llsAllReturnVisits.SelectedItem = null;
		}

		#endregion

                private void ReturnVisitFullList_OnLoaded(object sender, System.Windows.RoutedEventArgs e)
                {
                        racbRvSearchBox.IsEnabled = false;
                        UpdateModel();
                }
	}
}
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
using System.Windows.Controls;
using FieldService.Model;
using FieldService.ViewModels;
using Microsoft.Phone.Controls;

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
			//DataContext = new ReturnVisitFullListViewModel();
			InitializeComponent();
			ViewModel.LoadReturnVisitFullList();
			llsAllReturnVisits.ItemsSource = ViewModel.llReturnVisitFullListCategory;
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
	}
}
using System;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;

namespace FieldService.ViewModels
{
        public class HowToDataViewModel : ViewModelBase
        {
                private ObservableCollection<HowToDataItemViewModel> _items;

                /// <summary>
                /// Initializes the items.
                /// </summary>
                private void InitializeItems()
                {
                        this._items = new ObservableCollection<HowToDataItemViewModel>();
                        _items.Add(new HowToDataItemViewModel()
                        {
                            Title = StringResources.HowTo_WhatsNew,
                            Information = StringResources.HowTo_WhatsNew_FilterGPSButtons_I,
                            ImageSource = new Uri("/Images/whatsnew_filter-gps-buttons.png", UriKind.Relative)
                        });
                        _items.Add(new HowToDataItemViewModel() {
                            Title = StringResources.HowTo_WhatsNew,
                            Information = StringResources.HowTo_WhatsNew_GPSMapping_I,
                            ImageSource = new Uri("/Images/whatsnew_gpsmapping.png", UriKind.Relative)
                        });
                        _items.Add(new HowToDataItemViewModel() {
                            Title = StringResources.HowTo_WhatsNew,
                            Information = StringResources.HowTo_WhatsNew_SearchRVs_I,
                            ImageSource = new Uri("/Images/whatsnew_searchrvs.png", UriKind.Relative)
                        });
                        _items.Add(new HowToDataItemViewModel() {
                            Title = StringResources.HowTo_WhatsNew,
                            Information = StringResources.HowTo_WhatsNew_Presentations_I,
                            ImageSource = new Uri("/Images/whatsnew_presentations.png", UriKind.Relative)
                        });
                        _items.Add(new HowToDataItemViewModel() {
                            Title = StringResources.HowTo_WhatsNew,
                            Information = StringResources.HowTo_WhatsNew_ReportEntries_I,
                            ImageSource = new Uri("/Images/whatsnew_reportentries.png", UriKind.Relative)
                        });
                        _items.Add(new HowToDataItemViewModel()
                        {
                                Title = StringResources.HowTo_WhatsNext_T1,
                                Information = StringResources.HowTo_WhatsNext_I1
                        });
                }

                /// <summary>
                /// A collection for <see cref="HowToDataItemViewModel"/> objects.
                /// </summary>
                public ObservableCollection<HowToDataItemViewModel> Items {
                        get
                        {
                                if (this._items == null) {
                                        this.InitializeItems();
                                }
                                return this._items;
                        }
                        private set
                        {
                                this._items = value;
                        }
                }
        }
}

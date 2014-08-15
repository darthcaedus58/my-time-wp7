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
                    //this._items = new ObservableCollection<HowToDataItemViewModel>();
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_ContextMenus,
                    //    ImageSource = new Uri("/Images/whatsnew_contextmenu0.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_ContextMenus,
                    //    ImageSource = new Uri("/Images/whatsnew_contextmenu1.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_ContextMenus,
                    //    ImageSource = new Uri("/Images/whatsnew_contextmenu2.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_ContextMenus,
                    //    ImageSource = new Uri("/Images/whatsnew_contextmenu3.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_CustomDTUrl,
                    //    ImageSource = new Uri("/Images/whatsnew_custom_dt_url.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNew,
                    //    Information = StringResources.HowTo_WhatsNew_RoundTimeInc,
                    //    ImageSource = new Uri("/Images/whatsnew_roundtimeinc.png", UriKind.Relative)
                    //});
                    //_items.Add(new HowToDataItemViewModel() {
                    //    Title = StringResources.HowTo_WhatsNext_T1,
                    //    Information = StringResources.HowTo_WhatsNext_I1
                    //});
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

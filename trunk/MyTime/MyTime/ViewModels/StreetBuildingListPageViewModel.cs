using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FieldService.Annotations;
using FieldService.Model;

namespace FieldService.ViewModels
{
        class StreetBuildingListPageViewModel : INotifyPropertyChanged
        {
                public event PropertyChangedEventHandler PropertyChanged;

                public ObservableCollection<StreetBuildingItemModel> StreetBuildingListEntries;

                public bool IsStreetBuildingListLoading = true;

                public StreetBuildingListPageViewModel()
                {
                        StreetBuildingListEntries = new ObservableCollection<StreetBuildingItemModel>();
                        LoadStreetBuildingListEntries();
                }

                private void LoadStreetBuildingListEntries()
                {
                        if (!IsStreetBuildingListLoading){
                                IsStreetBuildingListLoading = true;
                                StreetBuildingListEntries.Clear();
                        }
                        //TODO: Implement me

                        StreetBuildingListEntries.Add( new StreetBuildingItemModel(1)
                        {
                                BuildingNumber = "13",
                                HouseCount = 32,
                                Street = "Hillendale Dr",
                                TerritoryCardId = 1
                        });
                        IsStreetBuildingListLoading = false;
                }


                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
                }
        }
}

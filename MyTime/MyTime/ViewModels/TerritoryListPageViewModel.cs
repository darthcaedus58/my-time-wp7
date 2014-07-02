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
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
        public class TerritoryListPageViewModel : INotifyPropertyChanged
        {
            private bool _isTerritoryListLoading = true;
                public event PropertyChangedEventHandler PropertyChanged;

                public ObservableCollection<TerritoryCardModel> TerritoryListEntries { get; private set; }

                public bool IsTerritoryListLoading
                {
                        get { return _isTerritoryListLoading; }
                        private set
                        {
                                if (_isTerritoryListLoading == value) return;
                                _isTerritoryListLoading = value;
                                OnPropertyChanged("IsTerritoryListLoading");
                        }
                }

                public TerritoryListPageViewModel()
                {
                        TerritoryListEntries = new ObservableCollection<TerritoryCardModel>();
                        LoadTerritoryList();
                }

                public void LoadTerritoryList()
                {
                        if (!IsTerritoryListLoading){
                                IsTerritoryListLoading = true;
                                TerritoryListEntries = new ObservableCollection<TerritoryCardModel>();
                        }

                        TerritoryCardData[] d = TerritoryCardsInterface.GetTerritoryCards(SortOrder.AscendingGeneric);
                    if (d == null) {
                        IsTerritoryListLoading = false;
                        return;
                    }
                    foreach (var c in d) {
                        TerritoryListEntries.Add(new TerritoryCardModel(c.ItemId)
                        {
                            Image = c.Image,
                            DateCreated = c.DateCreated,
                            Notes = c.Notes,
                            StreetCount = 0,
                            TerritoryNumber = c.TerritoryNumber
                        });
                    }
                        IsTerritoryListLoading = false;
                }

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

                }
        }
}
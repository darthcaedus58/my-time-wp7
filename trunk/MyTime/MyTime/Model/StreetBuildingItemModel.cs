using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FieldService.Annotations;

namespace FieldService.Model
{
        class StreetBuildingItemModel : INotifyPropertyChanged
        {
                private int _itemId;
                private int _territoryCardId;
                private string _street;
                private string _buildingNumber;
                private int _houseCount;
                private DateTime _dateCreated;
                public event PropertyChangedEventHandler PropertyChanged;

                public int ItemId
                {
                        get { return _itemId; }
                        private set
                        {
                                if (value == _itemId) return;
                                _itemId = value;
                                OnPropertyChanged();
                        }
                }

                public int TerritoryCardId
                {
                        get { return _territoryCardId; }
                        internal set
                        {
                                if (value == _territoryCardId) return;
                                _territoryCardId = value;
                                OnPropertyChanged();
                        }
                }

                public string Street
                {
                        get { return _street; }
                        set
                        {
                                if (value == _street) return;
                                _street = value;
                                OnPropertyChanged();
                        }
                }

                public string BuildingNumber
                {
                        get { return _buildingNumber; }
                        set
                        {
                                if (value == _buildingNumber) return;
                                _buildingNumber = value;
                                OnPropertyChanged();
                        }
                }

                public int HouseCount
                {
                        get { return _houseCount; }
                        set
                        {
                                if (value == _houseCount) return;
                                _houseCount = value;
                                OnPropertyChanged();
                        }
                }

                public string HouseCountString
                {
                        get
                        {
                                return string.Format(StringResources.StreetBuildingList_HouseCountFormatString, HouseCount);
                        }
                }

                public DateTime DateCreated
                {
                        get { return _dateCreated; }
                        set
                        {
                                if (value.Equals(_dateCreated)) return;
                                _dateCreated = value;
                                OnPropertyChanged();
                        }
                }

                public StreetBuildingItemModel(int itemId)
                {
                        ItemId = itemId;
                        DateCreated = DateTime.Now;
                }

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));

                }
        }
}

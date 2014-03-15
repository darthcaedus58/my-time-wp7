using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MyTimeDatabaseLib.Annotations;

namespace MyTimeDatabaseLib.Model
{
        #region Territory Cards DB Classes
        [Table]
        internal class TerritoryCardItem : INotifyPropertyChanged
        {
                private int _itemId;
                private byte[] _image;
                private string _territoryNumber;
                private string _notes;
                private DateTime _dateCreated;
                public event PropertyChangedEventHandler PropertyChanged;

                [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false,
                        AutoSync = AutoSync.OnInsert)]
                public int ItemId
                {
                        get { return _itemId; }
                        set
                        {
                                if (value == _itemId) return;
                                _itemId = value;
                                OnPropertyChanged();
                        }
                }

                [Column(DbType = "IMAGE")]
                public byte[] Image
                {
                        get { return _image; }
                        set
                        {
                                if (Equals(value, _image)) return;
                                _image = value;
                                OnPropertyChanged();
                        }
                }

                public int[] ImageSrc
                {
                        get
                        {
                                var result2 = new int[_image.Length / sizeof(int)];
                                Buffer.BlockCopy(_image, 0, result2, 0, _image.Length);
                                return result2;
                        }
                        set
                        {
                                var result = new byte[value.Length * sizeof(int)];
                                Buffer.BlockCopy(value, 0, result, 0, result.Length);
                                if (_image != result) {
                                        _image = result;
                                        OnPropertyChanged("Image");
                                }
                        }
                }

                [Column]
                public string TerritoryNumber
                {
                        get { return _territoryNumber; }
                        set
                        {
                                if (value == _territoryNumber) return;
                                _territoryNumber = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public string Notes
                {
                        get { return _notes; }
                        set
                        {
                                if (value == _notes) return;
                                _notes = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
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


                [Column(IsVersion = true)]
                private Binary _version;

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
                }
        }

        internal class TerritoryCardsDataContext : DataContext
        {
                public const string DBConnectionString =
                        "Data Source=isostore:/TerritoryCardsData.sdf;Max Database Size=512";

                public Table<TerritoryCardItem> TerritoryCardItems;

                public TerritoryCardsDataContext() : base(DBConnectionString) { }
        } 
        #endregion

        #region Streets / Buildings DB Classes
        [Table]
        internal class StreetsBuildingItem : INotifyPropertyChanged
        {
                [Column(IsVersion = true)] private Binary _version;
                private int _itemId;
                private int _territoyrCardId;
                private DateTime _dateCreated;
                private string _buildingNumber;
                private string _street;

                /// <summary>
                /// Gets or sets the item id.
                /// </summary>
                /// <value>The item id.</value>
                [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false,
                        AutoSync = AutoSync.OnInsert)]
                public int ItemId
                {
                        get { return _itemId; }
                        set
                        {
                                if (value == _itemId) return;
                                _itemId = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public int TerritoyrCardId
                {
                        get { return _territoyrCardId; }
                        set
                        {
                                if (value == _territoyrCardId) return;
                                _territoyrCardId = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
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

                [Column]
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

                [Column]
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

                public event PropertyChangedEventHandler PropertyChanged;

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));


                }
        }

        internal class StreetsBuildingDataContext : DataContext
        {
                public const string DBConnectionString =
                        "Data Source=isostore:/StreetsBuildingData.sdf;Max Database Size=512";

                public Table<StreetsBuildingItem> StreetsBuildingItems;

                public StreetsBuildingDataContext() : base(DBConnectionString) {}
        }
        #endregion

        #region House2House DB Classses
        [Table]
        internal class House2HouseRecordDataItem : INotifyPropertyChanged
        {
                public event PropertyChangedEventHandler PropertyChanged;

                [Column(IsVersion = true)] private Binary _version;
                private int _itemId;
                private int _symbolInt;
                private string _houseAptNumber;
                private int _territoryItemId;
                private int _streetItemId;
                private string _namePlacementRemarks;
                private DateTime _date;

                /// <summary>
                /// Gets or sets the item id.
                /// </summary>
                /// <value>The item id.</value>
                [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false,
                        AutoSync = AutoSync.OnInsert)]
                public int ItemId
                {
                        get { return _itemId; }
                        set
                        {
                                if (value == _itemId) return;
                                _itemId = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public int SymbolInt
                {
                        get { return _symbolInt; }
                        private set
                        {
                                if (value == _symbolInt) return;
                                _symbolInt = value;
                                OnPropertyChanged();
                                OnPropertyChanged("Symbol");
                        }
                }

                public House2HouseSymbol Symbol
                {
                        get { return (House2HouseSymbol) SymbolInt; }
                        set
                        {
                                SymbolInt = (int) value;
                                OnPropertyChanged();
                                OnPropertyChanged("SymbolInt");
                        }
                }

                [Column]
                public string HouseAptNumber
                {
                        get { return _houseAptNumber; }
                        set
                        {
                                if (value == _houseAptNumber) return;
                                _houseAptNumber = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public DateTime Date
                {
                        get { return _date; }
                        set
                        {
                                if (value.Equals(_date)) return;
                                _date = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public string NamePlacementRemarks
                {
                        get { return _namePlacementRemarks; }
                        set
                        {
                                if (value == _namePlacementRemarks) return;
                                _namePlacementRemarks = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public int StreetItemId
                {
                        get { return _streetItemId; }
                        set
                        {
                                if (value == _streetItemId) return;
                                _streetItemId = value;
                                OnPropertyChanged();
                        }
                }

                [Column]
                public int TerritoryItemId
                {
                        get { return _territoryItemId; }
                        set
                        {
                                if (value == _territoryItemId) return;
                                _territoryItemId = value;
                                OnPropertyChanged();
                        }
                }

                [NotifyPropertyChangedInvocator]
                protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
                }
        }

        internal class House2HouseRecordDataContext : DataContext
        {
                public const string DBConnectionString = "Data Source=isostore:/House2HouseRecordsData.sdf;Max Database Size=512";

                public Table<House2HouseRecordDataItem> House2HouseRecordItems;

                public House2HouseRecordDataContext() : base(DBConnectionString) {}
        }
        #endregion
}

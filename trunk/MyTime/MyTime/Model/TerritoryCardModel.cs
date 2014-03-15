using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FieldService.Annotations;

namespace FieldService.Model
{
        public class TerritoryCardModel : INotifyPropertyChanged
        {
                private string _territoryNumber;
                private int _itemId;
                private int _streetCount;
                private BitmapImage _image;
                private string _notes;
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

                public string  TerritoryNumber
                {
                        get { return _territoryNumber; }
                        set
                        {
                                if (value == _territoryNumber) return;
                                _territoryNumber = value;
                                OnPropertyChanged();
                                OnPropertyChanged("TerritoryNumberString");
                        }
                }

                public string TerritoryNumberString
                {
                        get
                        {
                                return string.Format(
                                        StringResources.TerritoryList_TerritoryNumberFormatString, TerritoryNumber);
                        }
                }

                public int StreetCount
                {
                        get { return _streetCount; }
                        set
                        {
                                if (value == _streetCount) return;
                                _streetCount = value;
                                OnPropertyChanged();
                                OnPropertyChanged("StreetCountString");
                        }
                }

                public string StreetCountString
                {
                        get
                        {
                                return string.Format(StringResources.TerritoryList_StreetCountFormatString, StreetCount);
                        }
                }

                public BitmapImage Image
                {
                        get
                        {
                                if(_image == null) 
                                        _image = new BitmapImage();
                                return _image;
                        }
                        set
                        {
                                if (Equals(value, _image)) return;
                                _image = value;
                                OnPropertyChanged("Image");
                        }
                }

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

                public DateTime DateCreated { get; set; }

                public TerritoryCardModel(int itemId)
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
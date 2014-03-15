using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using FieldService.Annotations;
using FieldService.Model;

namespace FieldService.ViewModels
{
        public class EditTerritoryCardViewModel : INotifyPropertyChanged
        {
                public event PropertyChangedEventHandler PropertyChanged;

                public TerritoryCardModel TerritoryCard;

                public EditTerritoryCardViewModel()
                {
                        TerritoryCard = new TerritoryCardModel(-1);
                }

                public BitmapImage TerritoryCardImage
                {
                        get { return TerritoryCard.Image; }
                        set
                        {
                                if (Equals(value, TerritoryCard.Image)) return;
                                TerritoryCard.Image = value;
                                OnPropertyChanged();
                        }
                }

                public string TerritoryCardTerritoryNumber
                {
                        get { return TerritoryCard.TerritoryNumber; }
                        set
                        {
                                if (value == TerritoryCard.TerritoryNumber) return;
                                TerritoryCard.TerritoryNumber = value;
                                OnPropertyChanged();
                        }
                }

                public string TerritoryCardNotes
                {
                        get { return TerritoryCard.Notes; }
                        set
                        {
                                if (value == TerritoryCard.Notes) return;
                                TerritoryCard.Notes = value;
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
}

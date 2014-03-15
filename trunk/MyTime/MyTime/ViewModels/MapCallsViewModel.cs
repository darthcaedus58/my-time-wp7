using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Device.Location;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FieldService.Annotations;

namespace FieldService.ViewModels
{
    public class MapCallsViewModel : INotifyPropertyChanged
    {
        public void StartLocationService()
        {
            _gcw = new GeoCoordinateWatcher(GeoPositionAccuracy.High) { MovementThreshold = 50 };
            _gcw.PositionChanged += _gcw_PositionChanged;
            _gcw.Start(true);
        }
        private void _gcw_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            if (e.Position == null || e.Position.Location == null) return;
            CurrentLocation = e.Position.Location;
        }

        private GeoCoordinate _currentLocation;
        private GeoCoordinateWatcher _gcw;

        public GeoCoordinate CurrentLocation
        {
            get { return _currentLocation ?? new GeoCoordinate(0,0); }
            set
            {
                if (_currentLocation != value) {
                    _currentLocation = value;
                    OnPropertyChanged();
                }
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
}

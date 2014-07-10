using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FieldService.Model;
using MyTimeDatabaseLib;

namespace FieldService.ViewModels
{
        public class ReturnVisitFullListViewModel : INotifyPropertyChanged
        {
                private bool _isRvFullListLoading;
                public event PropertyChangedEventHandler PropertyChanged;

                public ReturnVisitFullListViewModel()
                {
                        IsRvFullListLoading = true;
                }

                /// <summary>
                /// Gets a value indicating whether this instance is rv full list loaded.
                /// </summary>
                /// <value><c>true</c> if this instance is rv full list loaded; otherwise, <c>false</c>.</value>
                public bool IsRvFullListLoading
                {
                        get { return _isRvFullListLoading; }
                        private set
                        {
                                if (_isRvFullListLoading != value) {
                                        _isRvFullListLoading = value;
                                        OnPropertyChanged("IsRvFullListLoading");
                                }
                        }
                }

                /// <summary>
                /// Loads the return visit full list.
                /// </summary>
                public List<Group<ReturnVisitLLItemModel>> LoadReturnVisitFullList()
                {
                        IsRvFullListLoading = true;

                        var wb = new WriteableBitmap(100, 100);
                        for (int i = 0; i < wb.Pixels.Length; i++) {
                                wb.Pixels[i] = 0xFF3300;
                        }
                        var bmp = new BitmapImage();
                        using (var ms = new MemoryStream()) {
                                wb.SaveJpeg(ms, 100, 100, 0, 100);
                                bmp.SetSource(ms);
                        }

                        var rVs = ReturnVisitsInterface.GetReturnVisits(SortOrder.CityAToZ, -1);
                        if (rVs == null) return null;
                        if (rVs.Length <= 0) return null;
                        var rvList = new List<ReturnVisitLLItemModel>();
                        foreach (ReturnVisitData r in rVs) {

                                var bi = new BitmapImage();
                                if (r.ImageSrc != null && r.ImageSrc.Length >= 0) {
                                        var ris = new WriteableBitmap(450, 250);

                                        //get image from database
                                        for (int i = 0; i < r.ImageSrc.Length; i++) {
                                                ris.Pixels[i] = r.ImageSrc[i];
                                        }

                                        //put the image in a WritableBitmap
                                        using (var ms = new MemoryStream()) {
                                                ris.SaveJpeg(ms, 450, 250, 0, 100);
                                                bi.SetSource(ms);
                                        }

                                        //crop the image to 100x100 and centered
                                        var img = new Image
                                        {
                                                Source = bi,
                                                Width = 450,
                                                Height = 250
                                        };
                                        var wb2 = new WriteableBitmap(100, 100);
                                        var t = new CompositeTransform
                                        {
                                                ScaleX = 0.5,
                                                ScaleY = 0.5,
                                                TranslateX = -((450 / 2) / 2 - 50),
                                                TranslateY = -((250 / 2) / 2 - 50)
                                        };
                                        wb2.Render(img, t);
                                        wb2.Invalidate();
                                        bi = new BitmapImage();
                                        using (var ms = new MemoryStream()) {
                                                wb2.SaveJpeg(ms, 100, 100, 0, 100);
                                                bi.SetSource(ms);
                                        }
                                        //BitmapImage bi is now cropped
                                } else {
                                        bi = bmp; //Default image.
                                }

                                rvList.Add(new ReturnVisitLLItemModel
                                {
                                        Text = string.IsNullOrEmpty(r.FullName) ? string.Format("{0} year old {1}", r.Age, r.Gender) : r.FullName,
                                        Address1 = string.Format("{0} {1}", r.AddressOne, r.AddressTwo),
                                        Address2 = string.Format("{0}, {1} {2}", r.City, r.StateProvince, r.Country),
                                        City = r.City,
                                        ImageSource = bi,
                                        ItemId = r.ItemId,
                                        DaysSinceInt = (DateTime.Now - r.LastVisitDate).Days
                                });
                        }
                        IsRvFullListLoading = false;
                        return GetItemGroups(rvList, c => c.City);
                }

                public class Group<T> : List<T>
                {
                        public Group(string name, IEnumerable<T> items)
                                : base(items)
                        {
                                Title = name;
                        }

                        public string Title { get; set; }
                }

                private List<Group<T>> GetItemGroups<T>(IEnumerable<T> itemList, Func<T, string> getKeyFunc)
                {
                        IEnumerable<Group<T>> groupList = from item in itemList
                                                          group item by getKeyFunc(item) into g
                                                          orderby g.Key
                                                          select new Group<T>(g.Key, g);

                        return groupList.ToList();
                }

                protected virtual void OnPropertyChanged(string propertyName)
                {
                        PropertyChangedEventHandler handler = PropertyChanged;
                        if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
                }
        }
}

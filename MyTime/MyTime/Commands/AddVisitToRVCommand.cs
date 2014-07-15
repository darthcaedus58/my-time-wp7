using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;

namespace FieldService.Commands
{
    public class AddVisitToRVCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is int;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var itemId = (int)parameter;

            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri(string.Format("/View/PreviousCall.xaml?rvid={0}", itemId), UriKind.Relative));
        }

        public event EventHandler CanExecuteChanged;
    }
}

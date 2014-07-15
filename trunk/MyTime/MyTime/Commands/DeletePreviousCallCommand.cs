using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyTimeDatabaseLib;

namespace FieldService.Commands
{
    public class DeletePreviousCallCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is int;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            var itemId = (int) parameter;

            if (itemId < 0 || MessageBox.Show(StringResources.AddCallPage_DeleteCallConfirmation, StringResources.ApplicationName, MessageBoxButton.OKCancel) != MessageBoxResult.OK) return;
            bool v = RvPreviousVisitsDataInterface.DeleteCall(itemId);
            if (!v) {
                App.ToastMe(StringResources.AddCallPage_DeleteCallFailed);
            }
            App.ViewModel.ReturnVisitData.LoadPreviousVisits(App.ViewModel.ReturnVisitData.ItemId);
        }

        public event EventHandler CanExecuteChanged;
    }
}

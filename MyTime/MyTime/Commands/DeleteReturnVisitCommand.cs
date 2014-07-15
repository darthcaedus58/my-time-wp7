using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MyTimeDatabaseLib;
using Telerik.Windows.Controls;

namespace FieldService.Commands
{
    public class DeleteReturnVisitCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is int;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            var itemId = (int) parameter;

            if (MessageBox.Show(StringResources.RVPage_Messages_Delete, 
                StringResources.ApplicationName,
                    MessageBoxButton.OKCancel) == MessageBoxResult.Cancel) return;

            try {
                App.ViewModel.IsRvDataChanged = true;
                var deleteCalls = App.Settings.deleteCallsAndRv;
                App.ToastMe(ReturnVisitsInterface.DeleteReturnVisit(itemId, deleteCalls)
                    ? "Deleted."
                    : StringResources.AddTimePage_Messages_TimeDeleteFailed);
            }
            catch {
                App.ToastMe(ReturnVisitsInterface.DeleteReturnVisit(itemId,
                    (MessageBox.Show(StringResources.RVPage_Messages_IncludeAllVisits) == MessageBoxResult.OK))
                    ? "Deleted"
                    : StringResources.AddTimePage_Messages_TimeDeleteFailed);
            }
            finally {
                App.ViewModel.LoadReturnVisitList(SortOrder.DateNewestToOldest);
            }

        }

        public event EventHandler CanExecuteChanged;
    }
}

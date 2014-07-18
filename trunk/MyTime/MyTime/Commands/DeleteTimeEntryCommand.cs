using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using FieldService.Model;
using MyTimeDatabaseLib;

namespace FieldService.Commands
{
    public class DeleteTimeEntryCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            return parameter is TimeReportEntryViewModel;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;

            var entry = (TimeReportEntryViewModel) parameter;

            if (entry.Type == TimeType.Regular) {
                if (entry.ItemId < 0) return;
                var v = TimeDataInterface.DeleteTime(entry.ItemId);
                App.ToastMe(v
                    ? StringResources.AddTimePage_Messages_TimeDeleteSuccess
                    : StringResources.AddTimePage_Messages_TimeDeleteFailed);
            }
            else {
                if (entry.ItemId < 0) return; 
                bool v = RBCTimeDataInterface.DeleteTime(entry.ItemId);
                App.ToastMe(v ? StringResources.AddRBCTimePage_DeleteConfirmation : StringResources.AddRBCTimePage_DeleteFailed);
            }
        }

        public event EventHandler CanExecuteChanged;
    }
}

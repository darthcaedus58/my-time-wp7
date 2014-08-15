using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Controls;

namespace FieldService
{
    public class TelerikStringLoader : IStringResourceLoader
    {
        public string GetString(string key)
        {
            switch (key) {
                case InputLocalizationManager.LeapYearKey:
                    return StringResources.TelerikRadDatePicker_LeapYear;

                case InputLocalizationManager.EmptyDateContentKey:
                    return StringResources.TelerikRadDatePicker_EmptyDateContent;

                case InputLocalizationManager.DatePickerPopupHeaderKey:
                    return StringResources.TelerikRadDatePicker_DatePickerPopupHeader;

                case InputLocalizationManager.TimePickerPopupHeaderKey:
                    return StringResources.TelerikRadDatePicker_TimePickerPopupHeader;

                case InputLocalizationManager.EmptyTimeContentKey:
                    return StringResources.TelerikRadDatePicker_EmptyTimeContent;

                case InputLocalizationManager.TimeSpanPopupHeaderKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanPickerPopupHeader;

                case InputLocalizationManager.TimeSpanEmptyContentKey:
                    return StringResources.TelerikRadDatePicker_EmptyTimeSpanContent;

                case InputLocalizationManager.TimeSpanDayKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanDay;

                case InputLocalizationManager.TimeSpanHourKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanHour;

                case InputLocalizationManager.TimeSpanMinuteKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanMinute;

                case InputLocalizationManager.TimeSpanSecondKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanSecond;

                case InputLocalizationManager.TimeSpanWeekKey:
                    return StringResources.TelerikRadDatePicker_TimeSpanWeek;

                case InputLocalizationManager.CancelButtonTextKey:
                    return StringResources.TelerikRadDatePicker_CancelButttonText;

                case InputLocalizationManager.OkButtonTextKey:
                    return StringResources.TelerikRadDatePicker_OkButtonText;
            }
            return null;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Tasks;
using MyTimeDatabaseLib;

namespace FieldService
{
    public class Reporting
    {
        public static void SendReport(string body)
        {
            body += "\nThanks";
            var sendType = addressType.Email;
            string sendTo = String.Empty;
            bool includeSig = true;
            try {
                Setting nickName = App.AppSettingsProvider["NickName"];
                body += String.Format(",\n{0}", nickName.Value);
                Setting to = App.AppSettingsProvider["csoEmail"];
                sendType = to.AddressType;
                sendTo = to.Value;
                includeSig = App.AppSettingsProvider["sharefsapp"].Value.Equals(Boolean.TrueString, StringComparison.CurrentCultureIgnoreCase);
            } catch (Exception) {}

            if (sendType == addressType.Email) {
                if (includeSig) {
                    body += "\n\n\nP.S. - This report was generated by the \"Field Service\" App on my Windows Phone! If you would like to try this app you can download it from the Marketplace!";
                }
                var emailcomposer = new EmailComposeTask {Subject = String.Format("{0:MMMM} {0:yyyy} Service Report", DateTime.Today), Body = body, To = sendTo};
                emailcomposer.Show();
                return;
            }
            var composeSms = new SmsComposeTask {Body = body, To = sendTo};
            composeSms.Show();
        }

        public static TimeData[] BuildTimeReport(DateTime fromDate, DateTime toDate, SortOrder so)
        {
            TimeData[] entries = TimeDataInterface.GetEntries(fromDate, toDate, so);

            try {
                bool countCalls = bool.Parse(App.AppSettingsProvider["AddCallPlacements"].Value);

                if (countCalls) {
                    RvPreviousVisitData[] calls = RvPreviousVisitsDataInterface.GetCallsByDate(fromDate, toDate);
                    if (calls != null) {
                        List<int> rvList = new List<int>();
                        int month = fromDate.Month;
                        List<TimeData> entriesMore = new List<TimeData>(entries);
                        foreach (var c in calls) {
                            if (c.Date.Month != month) {
                                month = c.Date.Month;
                                rvList = new List<int>();
                            }
                            bool found = false;
                            foreach (var e in entries) {
                                if (e.Date.Date != c.Date.Date) continue; 
                                // Check for call data which happened on the same date as another service day
                                // If it did, add the values, otherwise continue
                                e.Magazines += c.Magazines;
                                e.Books += c.Books;
                                e.Brochures += c.Brochures;
                                if (!rvList.Contains(c.RvItemId)) { // don't double count the rv for the month.
                                    e.ReturnVisits++;
                                    rvList.Add(c.RvItemId);
                                }
                                found = true;
                                break;
                            }
                            if (!found) {        // We found a call, but no service time was recorded on this date
                                var rvCnt = 0;
                                if (!rvList.Contains(c.RvItemId)) {
                                    rvCnt = 1;
                                    rvList.Add(c.RvItemId);
                                }

                                entriesMore.Add(new TimeData()
                                {
                                    Magazines = c.Magazines,
                                    Books = c.Books,
                                    Brochures = c.Brochures,
                                    Date = c.Date,
                                    ReturnVisits = rvCnt
                                });
                            }
                        }
                        entries = entriesMore.OrderBy(s => s.Date).ToArray();
                    }
                }
            } catch {
                return entries;
            }
            return entries;
        }
    }
}

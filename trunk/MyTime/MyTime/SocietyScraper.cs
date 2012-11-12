// ***********************************************************************
// Assembly         : MyTime
// Author           : trevo_000
// Created          : 11-03-2012
//
// Last Modified By : trevo_000
// Last Modified On : 11-03-2012
// ***********************************************************************
// <copyright file="SocietyScraper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using HtmlAgilityPack;

namespace FieldService
{
    /// <summary>
    /// Class DailyTextScraper
    /// </summary>
    public class DailyTextScraper
    {
        #region Delegates

        /// <summary>
        /// Delegate DailyTextRetrievedEventHandler
        /// </summary>
        /// <param name="dt">The dt.</param>
        public delegate void DailyTextRetrievedEventHandler(DailyText dt);

        #endregion

        /// <summary>
        /// The _HW
        /// </summary>
        private HtmlWeb _hw;

        #region Events

        /// <summary>
        /// HW_s the load completed.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The e.</param>
        protected void hw_LoadCompleted(object o, HtmlDocumentLoadCompleted e)
        {
            HtmlDocument doc = e.Document;
            HtmlNode docNode = doc.DocumentNode;

            HtmlNodeCollection tags = docNode.SelectNodes(".//p[@class='sa']");
            string scripture = tags.Count > 0 ? tags[0].InnerText : string.Empty;

            tags = docNode.SelectNodes(".//p[@class='sb']");
            string summaryText = tags.Count > 0 ? tags[0].InnerText : string.Empty;

            var dt = new DailyText {
                                       Scripture = scripture,
                                       SummaryText = summaryText
                                   };

            DailyTextRetrieved.Invoke(dt);
        }

        #endregion

        /// <summary>
        /// Occurs when [daily text retrieved].
        /// </summary>
        public event DailyTextRetrievedEventHandler DailyTextRetrieved;

        /// <summary>
        /// Starts the daily text retrieval.
        /// </summary>
        /// <param name="d">The d.</param>
        public void StartDailyTextRetrieval(DateTime d)
        {
            string url = string.Format("http://wol.jw.org/en/wol/dt/r1/lp-e/{0}/{1}/{2}", d.Year, d.Month, d.Day);

            _hw = new HtmlWeb();
            _hw.LoadCompleted += hw_LoadCompleted;
            _hw.LoadAsync(url);
        }
    }

    /// <summary>
    /// Class DailyText
    /// </summary>
    public class DailyText
    {
        /// <summary>
        /// Gets the scripture.
        /// </summary>
        /// <value>The scripture.</value>
        public string Scripture { get; internal set; }

        /// <summary>
        /// Gets the summary text.
        /// </summary>
        /// <value>The summary text.</value>
        public string SummaryText { get; internal set; }
    }
}
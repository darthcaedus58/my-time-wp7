using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Xml;

namespace MyTime
{
	public class DailyTextScraper
	{
		public delegate void DailyTextRetrievedEventHandler(DailyText dt);
		public event DailyTextRetrievedEventHandler DailyTextRetrieved;

		private HtmlWeb _hw;

		public void StartDailyTextRetrieval(DateTime d)
		{
			string url = string.Format("http://wol.jw.org/en/wol/dt/r1/lp-e/{0}/{1}/{2}", d.Year, d.Month, d.Day);

			_hw = new HtmlWeb();
			_hw.LoadCompleted += new EventHandler<HtmlDocumentLoadCompleted>(hw_LoadCompleted);
			_hw.LoadAsync(url);
		}

		protected void hw_LoadCompleted(object o, HtmlDocumentLoadCompleted e)
		{
			HtmlDocument doc = e.Document;
			HtmlNode docNode = doc.DocumentNode;

			HtmlNodeCollection tags = docNode.SelectNodes(".//p[@class='sa']");
			string Scripture = tags.Count > 0 ? tags[0].InnerText : string.Empty;

			tags = docNode.SelectNodes(".//p[@class='sb']");
			string SummaryText = tags.Count > 0 ? tags[0].InnerText : string.Empty;

			var dt = new DailyText()
			{
				Scripture = Scripture,
				SummaryText = SummaryText
			};

			DailyTextRetrieved.Invoke(dt);
		}
	}

	public class DailyText
	{
		public string Scripture
		{
			get;
			internal set;
		}

		public string SummaryText
		{
			get;
			internal set;
		}
	}
}

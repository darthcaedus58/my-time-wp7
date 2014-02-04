using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace FieldService.SocietyScraper
{
        class PresentationsScraper
        {
                public delegate void PresentationTextRetrievedEventHandler(string text);

                public event PresentationTextRetrievedEventHandler TextRetrieved;

                protected virtual void OnTextRetrieved(string text)
                {
                        PresentationTextRetrievedEventHandler handler = TextRetrieved;
                        if (handler != null) handler(text);
                }

                private HtmlWeb _hw;

                public void StartPresentationTextRetrieval(string url)
                {
                        _hw = new HtmlWeb();
                        
                    _hw.LoadCompleted += hw_LoadCompleted;
                    _hw.LoadAsync(url);
                }

                private void hw_LoadCompleted(object sender, HtmlDocumentLoadCompleted e)
                {
                        if (e.Document != null) {
                                var txt = e.Document;

                                OnTextRetrieved(txt.DocumentNode.InnerText.Replace("\\n","\n").Replace("&nbsp;"," ").Replace("&quot;","\""));
                        }
                }
        }
}

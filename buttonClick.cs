using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mshtml;
using System.Windows.Forms;
using EO.WebBrowser;
using System.Net.Http;

namespace HashHandler
{

    public static class HtmlElementHelper
    {
        public static bool IsNotNull(object variable)
        {
            if (variable == null)
            {

                return true;
            }
            else if (variable != null) {

                return false;
            }
            return false;
        }

        public static bool IsNull(object variable) {

            if (variable == null)
            {

                return true;
            }
            else if (variable != null)
            {

                return false;
            }
            return false;

        }

        public static string Val(this WebBrowser wb, string id, string Val = null)
        {
            HtmlElement ele = wb.Document.GetElementById(id);
            if (IsNotNull(ele))
            {
                HTMLInputElement txtele = (HTMLInputElement)ele.DomElement;
                if (IsNull(Val))
                    Val = txtele.value;
                else
                    txtele.value = Val;
            }
            return Val;
        }
        public static void eleClick(WebBrowser wb, string id)
        {
            HtmlElement ele = wb.Document.All[id];
            if (IsNotNull(ele))
            {
                switch (ele.TagName.ToLower())
                {
                    case "input":
                        HTMLButtonElement btnele = (HTMLButtonElement)ele.DomElement;
                        btnele.click();
                        break;
                    case "a":
                        HTMLAnchorElement ancele = (HTMLAnchorElement)ele.DomElement;
                        ancele.click();
                        break;
                }
            }
        }
        public static void eleClick(this WebBrowser wb, string tag, string data)
        {
            HtmlElementCollection eleCollection = wb.Document.GetElementsByTagName(tag);
            foreach (HtmlElement ele in eleCollection)
            {
                if (ele.InnerHtml.ToString().ToLower() == data)
                {
                    switch (tag)
                    {
                        case "a":
                            HTMLAnchorElement ancele = (HTMLAnchorElement)ele.DomElement;
                            ancele.click();
                            break;
                    }
                    break;
                }
            }
        }
        public static string Html(this WebBrowser wb, string id, string val = null)
        {
            HtmlElement ele = wb.Document.GetElementById(id);
            if (IsNotNull(ele))
            {
                switch (ele.TagName.ToLower())
                {
                    case "span":
                        HTMLSpanElement spanEle = (HTMLSpanElement)ele.DomElement;
                        if (IsNull(val))
                            val = spanEle.innerHTML;
                        else
                            spanEle.innerHTML = val;
                        break;
                    case "div":
                        HTMLDivElement divEle = (HTMLDivElement)ele.DomElement;
                        if (IsNull(val))
                            val = divEle.innerHTML;
                        else
                            divEle.innerHTML = val;
                        break;
                }
            }
            return val;
        }
        public static string Style(this WebBrowser wb, string id)
        {
            HtmlElement ele = wb.Document.All[id];
            return ele.Style.ToString();
        }
        public static string Attr(this WebBrowser wb, string id, string key, string val = null, bool isTag = false, string data = null)
        {
            if (!isTag)
            {
                HtmlElement ele = wb.Document.All[id];
                if (IsNull(val))
                    val = ele.GetAttribute(key);
                else
                    ele.SetAttribute(key, val);
            }
            else
            {
                HtmlElementCollection eleCollection = wb.Document.GetElementsByTagName(id);
                foreach (HtmlElement ele in eleCollection)
                {
                    if (ele.InnerHtml.ToString().ToLower().Contains(data))
                    {
                        switch (id)
                        {
                            case "a":
                                if (IsNull(val))
                                    val = ele.GetAttribute(key);
                                else
                                    ele.SetAttribute(key, val);
                                break;
                        }
                        break;
                    }
                }
            }
            return val;
        }

    }

}

using HtmlAgilityPack;
using MilasLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DigiKala
{
   public class DigikalaScraper
    {
        const string url = "https://www.digikala.com/search/category-mobile-phone/?attribute[A2255][0]=19561&price[min]=8430154&price[max]=10657120&pageno=1&last_filter=2255&last_value=19561&sortby=4";
        List<string> ProductLinks = new List<string>();
        List<DTO> MyDto = new List<DTO>();
        private string conecctionString="server=.;DataBase=SqlTutorial;Trusted_Connection=True;";

        public void Scrap()
        {
            var mainPage = LoadPage(url);
            GetProductLink(mainPage);
            foreach(var l in ProductLinks)
            {
                var doc = LoadPage(l);
                var rows = GetRowItem(doc);
                foreach(var row in rows)
                {
                    var key = row.SelectSingleNode("div[@class='c-params__list-key']").InnerText.Trim();
                    var value = row.SelectSingleNode("div[@class='c-params__list-value']/span").InnerText.Trim();
                    var productName = doc.DocumentNode.SelectSingleNode("//h1[@class='c-product__title']").InnerText.Trim();
                    var dto = new DTO()
                    {
                        ItemKey = key,
                        ItemValue = value,
                        ProductName = productName
                    };
                    MyDto.Add(dto);
                }

            }
            DtoToSql sql = new DtoToSql(MyDto);
            sql.StoreDto("Digikala",conecctionString );

        }
        public HtmlDocument LoadPage(string url)
        {
            HtmlWeb web = new HtmlWeb();
             var page =web.Load(url);
            return page;
        }
        public void GetProductLink(HtmlDocument page)
        {
            string preLink = "https://www.digikala.com";
            var links = page.DocumentNode.SelectNodes("//a[@class='c-product-box__img c-promotion-box__image js-url js-product-item js-product-url']");
            foreach( var l in links )
            {
                var validLink =preLink + l.GetAttributeValue("href", null);
                ProductLinks.Add(validLink);
            }
        }
        public List<HtmlNode> GetRowItem(HtmlDocument doc)
        {
            var row = doc.DocumentNode.SelectNodes("//ul[@class='c-params__list']/li").ToList();
            return row;

        }
    }
}

using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using TrendYolComments.UI.Models;

namespace TrendYolComments.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public static List<ProductModel> _productlist = new List<ProductModel>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        string urlYorumDetayUrunBazinda = "https://www.trendyol.com/dogo/cocuk-vegan-deri-mor-sandalet-minicity-tasarim-p-42609317/yorumlar";
        string urlUrunDetayURL = "https://www.trendyol.com/dogo/";
        string urunYorum = "https://www.trendyol.com/dogo/kadin-vegan-deri-cok-renkli-sneakers-catch-me-if-you-can-tweety-tasarim-p-49246168/yorumlar";
        public async Task< IActionResult> Index()
        {
       
         //var productCode=   GiveProductCode("parallel-universe-mare-plaj-havlusu-p-705148690");

            //string productCode = GiveProductCode("kadin-vegan-deri-multicolor-sneakers-relationships-tasarim-p-681226088");
            //veritabanından gelecek
            int lastIndexSayac = 22;

            int currentLastIndex;

            for (int i = 1; i < lastIndexSayac+1; i++)
            {

                var url = "https://www.trendyol.com/sr?wb=871&pr=4.5&pi="+i;

                //var client=new HttpClient();
                //var html=await client.GetStringAsync(url);
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                HtmlNode parent = doc.DocumentNode.SelectSingleNode("//div[@class='prdct-cntnr-wrppr']");

                if (parent != null)
                {
                    List<HtmlNode> children = parent.SelectNodes(".//div[@class='p-card-wrppr with-campaign-view']").ToList();

                    foreach (var product in children)
                    {
                        //var child=product.SelectSingleNode("//div[@class='p-card-chldrn-cntnr card-border']/*[1]");
                        var firstchild = product.FirstChild.FirstChild;
                        var attribute = firstchild.GetAttributeValue("href", "yok");
                        var dataatt = attribute.Split('/')[2];
                        var realproduct = dataatt.Split("?")[0];

                        ProductModel productModel = new ProductModel();
                        productModel.Name=realproduct;
                        productModel.ProductCode=GiveProductCode(realproduct);
                        if(_productlist.FirstOrDefault(a=>a.Name==realproduct)== null)
                        _productlist.Add(productModel);

                 

                    }
                }
              
                    currentLastIndex = i;
                //veritabanına update edilecek 

            }
            var productlist = _productlist;

            return View();
        }
        public string GiveProductCode(string product)
        {
            product = product.Replace(" ", "");

            urlUrunDetayURL = urlUrunDetayURL + product;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(urlUrunDetayURL);
            HtmlNode parent = doc.DocumentNode.SelectSingleNode("//div[@class='pr-in-cn']");
           
            //parent ?? parent= doc.DocumentNode.SelectSingleNode("//div[@class='pr - new- br']");

            if (parent != null)
            {
                var divnode = parent.FirstChild;
                var hnode = divnode.SelectSingleNode("//h1[@class='pr-new-br']/*[2]");
                string data=hnode.InnerText;
                data=data.ToLower().Trim();
                var hamdata = data.Trim().Split("-");
                var startpiece = hamdata[hamdata.Count()-2].Split(" ").Last();
                var lastpiece = hamdata.Last();
                var productCode = String.Format("{0}{1}{2}", startpiece,"-", lastpiece);
                //string productCode = hamdata.Split(' ')[2];
                return productCode;
            }
          

            return "Yok";
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
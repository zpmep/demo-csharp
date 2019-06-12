using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ZaloPayDemo.ZaloPay;
using ZaloPayDemo.DAL;
using Newtonsoft.Json;

namespace ZaloPayDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var data = new Dictionary<string, object>();
            Request.Form.CopyTo(data);

            data["embeddata"] = NgrokHelper.CreateEmbeddataWithPublicUrl();

            var orderData = ZaloPayHelper.NewCreateOrderData(data);
            var order = await ZaloPayHelper.CreateOrder(orderData);
            var orderurl = order["orderurl"].ToString();

            using(var db = new ZaloPayDemoContext())
            {
                db.Orders.Add(new Models.Order
                {
                    ApptransID = orderData["apptransid"],
                    Amount = long.Parse(orderData["amount"]),
                    Timestamp = long.Parse(orderData["apptime"]),
                    Description = orderData["description"],
                    Status = 0
                });

                await db.SaveChangesAsync();
            }

            ViewData["orderurl"] = orderurl;
            ViewData["QRCodeBase64Image"] = QRCodeHelper.CreateQRCodeBase64Image(orderurl);

            return View("Index");
        }
    }
}
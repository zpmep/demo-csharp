using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using ZaloPayDemo.ZaloPay;
using ZaloPayDemo.DAL;

namespace ZaloPayDemo.Controllers
{
    public class GatewayController : Controller
    {
        // GET: Gateway
        public async Task<ActionResult> Index()
        {
            var banklistResponse = await ZaloPayHelper.GetBankList();
            var banklist = ZaloPayHelper.ParseBankList(banklistResponse);

            ViewData["banklist"] = banklist;

            return View();
        }

        [HttpPost]
        public void Post()
        {
            var data = new Dictionary<string, object>();
            Request.Form.CopyTo(data);

            data["embeddata"] = NgrokHelper.CreateEmbeddataWithPublicUrl();
            
            var orderData = ZaloPayHelper.NewCreateOrderData(data);
            var orderurl = ZaloPayHelper.Gateway(orderData);

            using (var db = new ZaloPayDemoContext())
            {
                db.Orders.Add(new Models.Order
                {
                    ApptransID = orderData["apptransid"],
                    Amount = long.Parse(orderData["amount"]),
                    Timestamp = long.Parse(orderData["apptime"]),
                    Description = orderData["description"],
                    Status = 0
                });

                db.SaveChanges();
            }

            Response.Redirect(orderurl);
        }
    }
}
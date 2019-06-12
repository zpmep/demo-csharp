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
    public class QuickpayController : Controller
    {
        // GET: Quickpay
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

            var orderData = ZaloPayHelper.NewQuickPayOrderData(data);
            var result = await ZaloPayHelper.QuickPay(orderData);

            var returncode = int.Parse(result["returncode"].ToString());

            if (returncode > 0)
            {
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
            }
            
            ViewData["result"] = result;

            return View("Index");
        }
    }
}
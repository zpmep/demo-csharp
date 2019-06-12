using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZaloPayDemo.ZaloPay;
using ZaloPayDemo.DAL;
using System.Threading.Tasks;

namespace ZaloPayDemo.Controllers
{
    public class RefundController : Controller
    {
        [HttpPost]
        public async Task Index()
        {
            try
            {
                var data = new Dictionary<string, object>();
                Request.Form.CopyTo(data);

                var refundData = ZaloPayHelper.NewRefundData(data);
                var result = await ZaloPayHelper.Refund(refundData);

                var returncode = int.Parse(result["returncode"].ToString());

                if (returncode >= 1)
                {
                    while (true)
                    {
                        var refundStatus = await ZaloPayHelper.GetRefundStatus(refundData["mrefundid"]);
                        var c = int.Parse(refundStatus["returncode"].ToString());

                        if (c < 2)
                        {
                            if (c == 1)
                            {
                                using(var db = new ZaloPayDemoContext())
                                {
                                    db.Refunds.Add(new Models.Refund
                                    {
                                        Amount = long.Parse(refundData["amount"]),
                                        ZptransID = refundData["zptransid"].ToString(),
                                        MrefundID = refundData["mrefundid"].ToString()
                                    });

                                    db.SaveChanges();
                                }
                            }

                            Session["refundResult"] = refundStatus;

                            break;
                        }

                        System.Threading.Thread.Sleep(1000);
                    }
                }
                else
                {
                    Session["refundResult"] = result;
                }
            } catch (Exception ex)
            {
                var result = new Dictionary<string, object>();
                result["returncode"] = -1;
                result["returnmessage"] = "Exception: " + ex.Message;

                Session["refundResult"] = result;
            }

            Response.Redirect("/History");
        }
    }
}
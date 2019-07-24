using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using ZaloPayDemo.Utils;

namespace ZaloPayDemo.Controllers
{
    public class SubscribeController : Controller
    {
        private static long WAIT_TIME = 1200; // 1200 seconds = 20 minutes
        // Long polling
        public ActionResult Index()
        {
            var apptransid = Request.QueryString["apptransid"];
            object payload = null;
            long seconds = 0;

            while (seconds < WAIT_TIME)
            {
                payload = Storage.Get(apptransid);
                if (payload != null)
                {
                    Storage.Remove(apptransid);
                    break;
                }

                Thread.Sleep(1000);
                seconds += 1;
            }

            return Json(payload, JsonRequestBehavior.AllowGet);
        }
    }
}
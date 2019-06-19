using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ZaloPayDemo.ZaloPay.Crypto;
using Newtonsoft.Json;

namespace ZaloPayDemo.ZaloPay.Models
{
    public class OrderData
    {
        public string Appid { get; set; }
        public string Apptransid { get; set; }
        public long Apptime { get; set; }
        public string Appuser { get; set; }
        public string Item { get; set; }
        public string Embeddata { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public string Bankcode { get; set; }
        public string Mac { get; set; }
        
        public OrderData(long amount, string description = "", string bankcode = "", object embeddata = null, object item = null, string appuser = "")
        {
            Appid = ConfigurationManager.AppSettings["Appid"];
            Apptransid = ZaloPayHelper.GenTransID();
            Apptime = Util.GetTimeStamp();
            Appuser = appuser;
            Amount = amount;
            Description = description;
            Embeddata = JsonConvert.SerializeObject(embeddata);
            Item = JsonConvert.SerializeObject(item);
            Mac = ComputeMac();
        }

        public virtual string GetMacData()
        {
            return Appid + "|" + Apptransid + "|" + Appuser + "|" + Amount + "|" + Apptime + "|" + Embeddata + "|" + Item;
        }

        public string ComputeMac()
        {
            return HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ConfigurationManager.AppSettings["Key1"], GetMacData());
        }
    }

    public class QuickPayOrderData : OrderData
    {
        public string Paymentcode { get; set; }

        public QuickPayOrderData(long amount, string paymentcodeRaw, string description = "", object embeddata = null, object item = null, string appuser = "")
            : base(amount, description, "", embeddata, item, appuser)
        {
            Paymentcode = RSAHelper.Encrypt(paymentcodeRaw, ConfigurationManager.AppSettings["RSAPublicKey"]);
            Mac = ComputeMac();
        }

        public override string GetMacData()
        {
            return base.GetMacData() + "|" + Paymentcode;
        }
    }
}
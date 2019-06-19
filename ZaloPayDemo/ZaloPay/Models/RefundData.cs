using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ZaloPayDemo.ZaloPay.Crypto;

namespace ZaloPayDemo.ZaloPay.Models
{
    public class RefundData
    {
        public string Appid { get; set; }
        public string Zptransid { get; set; }
        public long Amount { get; set; }
        public string Description { get; set; }
        public long Timestamp { get; set; }
        public string Mrefundid { get; set; }
        public string Mac { get; set; }

        public RefundData(long amount, string zptransid, string description = "")
        {
            Appid = ConfigurationManager.AppSettings["Appid"];
            Zptransid = zptransid;
            Amount = amount;
            Description = description;
            Mrefundid = ZaloPayHelper.GenTransID();
            Timestamp = Util.GetTimeStamp();
            Mac = ComputeMac();
        }

        public string GetMacData()
        {
            return Appid + "|" + Zptransid + "|" + Amount + "|" + Description + "|" + Timestamp;
        }

        public string ComputeMac()
        {
            return HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ConfigurationManager.AppSettings["Key1"], GetMacData());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ZaloPayDemo.ZaloPay.Crypto;
using ZaloPayDemo.ZaloPay.Models;
using ZaloPayDemo.ZaloPay.Extension;

namespace ZaloPayDemo.ZaloPay
{
    public class ZaloPayHelper
    {
        private static long uid = Util.GetTimeStamp();

        public static bool VerifyCallback(string data, string requestMac)
        {
            try
            {
                string mac = HmacHelper.Compute(ZaloPayHMAC.HMACSHA256, ConfigurationManager.AppSettings["Key2"], data);

                return requestMac.Equals(mac);
            } catch
            {
                return false;
            }
        }

        public static bool VerifyRedirect(Dictionary<string, object> data)
        {
            try
            {
                string reqChecksum = data["checksum"].ToString();
                string checksum = ZaloPayMacGenerator.Redirect(data);

                return reqChecksum.Equals(checksum);
            } catch
            {
                return false;
            }
        }

        public static string GenTransID()
        {
            return DateTime.Now.ToString("yyMMdd") + "_" + ConfigurationManager.AppSettings["Appid"] + "_" + (++uid); 
        }

        public static Task<Dictionary<string, object>> CreateOrder(Dictionary<string, string> orderData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiCreateOrder"], orderData);
        }

        public static Task<Dictionary<string, object>> CreateOrder(OrderData orderData)
        {
            return CreateOrder(orderData.AsParams());
        }

        public static Task<Dictionary<string, object>> QuickPay(Dictionary<string, string> orderData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiQuickPay"], orderData);
        }

        public static Task<Dictionary<string, object>> QuickPay(QuickPayOrderData orderData)
        {
            return QuickPay(orderData.AsParams());
        }

        public static Task<Dictionary<string, object>> GetOrderStatus(string apptransid)
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("apptransid", apptransid);
            data.Add("mac", ZaloPayMacGenerator.GetOrderStatus(data));

            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiGetOrderStatus"], data);
        }

        public static Task<Dictionary<string, object>> Refund(Dictionary<string, string> refundData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiRefund"], refundData);
        }

        public static Task<Dictionary<string, object>> Refund(RefundData refundData)
        {
            return Refund(refundData.AsParams());
        }

        public static Task<Dictionary<string, object>> GetRefundStatus(string mrefundid)
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("mrefundid", mrefundid);
            data.Add("timestamp", Util.GetTimeStamp().ToString());
            data.Add("mac", ZaloPayMacGenerator.GetRefundStatus(data));

            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiGetRefundStatus"], data);
        }

        public static Task<Dictionary<string, object>> GetBankList()
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("reqtime", Util.GetTimeStamp().ToString());
            data.Add("mac", ZaloPayMacGenerator.GetBankList(data));

            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiGetBankList"], data);
        }

        public static List<BankDTO> ParseBankList(Dictionary<string, object> banklistResponse)
        {
            var banklist = new List<BankDTO>();
            var bankMap = (banklistResponse["banks"] as JObject);

            foreach (var bank in bankMap)
            {
                var bankDTOs = bank.Value.ToObject<List<BankDTO>>();
                foreach (var bankDTO in bankDTOs)
                {
                    banklist.Add(bankDTO);
                }
            }

            return banklist;
        }
    }
}
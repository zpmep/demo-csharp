using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Text;
using ZaloPayDemo.ZaloPay.Crypto;
using ZaloPayDemo.ZaloPay.Models;

namespace ZaloPayDemo.ZaloPay
{
    public class ZaloPayHelper
    {
        private static long uid = GetTimeStamp();

        static long GetTimeStamp(DateTime date)
        {
            return (long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }

        static long GetTimeStamp()
        {
            return GetTimeStamp(DateTime.Now);
        }

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

        public static Dictionary<string, string> NewCreateOrderData(Dictionary<string, object> data)
        {
            var order = new Dictionary<string, string>();

            order.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            order.Add("apptime", GetTimeStamp().ToString());
            order.Add("apptransid", GenTransID());
            order.Add("appuser", data.ContainsKey("appuser") ? data["appuser"].ToString() : "demo");
            order.Add("item", data.ContainsKey("item") ? data["item"].ToString() : "");
            order.Add("embeddata", data.ContainsKey("embeddata") ? data["embeddata"].ToString() : "");
            order.Add("amount", data["amount"].ToString());
            order.Add("description", data["description"].ToString());
            order.Add("bankcode", data.ContainsKey("bankcode") ? data["bankcode"].ToString() : "zalopayapp");
            order.Add("mac", ZaloPayMacGenerator.CreateOrder(order));

            return order;
        }

        public static Dictionary<string, string> NewQuickPayOrderData(Dictionary<string, object> data)
        {
            var order = NewCreateOrderData(data);
            var paymentcodeRaw = data["paymentcodeRaw"].ToString();
            var paymentcode = RSAHelper.Encrypt(paymentcodeRaw, ConfigurationManager.AppSettings["RSAPublicKey"]);

            order.Add("userip", data.ContainsKey("userip") ? data["userip"].ToString() : "127.0.0.1");
            order.Add("paymentcode", paymentcode);
            order["mac"] = ZaloPayMacGenerator.QuickPay(order, paymentcodeRaw);

            return order;
        }

        public static Task<Dictionary<string, object>> CreateOrder(Dictionary<string, string> orderData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiCreateOrder"], orderData);
        }

        public static string Gateway(Dictionary<string, string> orderData)
        {
            var orderJSON = JsonConvert.SerializeObject(orderData);
            var orderJSONBytes = Encoding.ASCII.GetBytes(orderJSON);
            var orderJSONBase64String = Convert.ToBase64String(orderJSONBytes);

            return ConfigurationManager.AppSettings["ZaloPayApiGateway"] + orderJSONBase64String;
        }

        public static Task<Dictionary<string, object>> QuickPay(Dictionary<string, string> orderData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiQuickPay"], orderData);
        }

        public static Task<Dictionary<string, object>> GetOrderStatus(string apptransid)
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("apptransid", apptransid);
            data.Add("mac", ZaloPayMacGenerator.GetOrderStatus(data));

            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiGetOrderStatus"], data);
        }

        public static Dictionary<string, string> NewRefundData(Dictionary<string, object> data)
        {
            var refundData = new Dictionary<string, string>();
            refundData.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            refundData.Add("zptransid", data["zptransid"].ToString());
            refundData.Add("amount", data["amount"].ToString());
            refundData.Add("description", data["description"].ToString());
            refundData.Add("timestamp", GetTimeStamp().ToString());
            refundData.Add("mrefundid", GenTransID());
            refundData.Add("mac", ZaloPayMacGenerator.Refund(refundData));

            return refundData;
        }

        public static Task<Dictionary<string, object>> Refund(Dictionary<string, string> refundData)
        {
            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiRefund"], refundData);
        }

        public static Task<Dictionary<string, object>> GetRefundStatus(string mrefundid)
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("mrefundid", mrefundid);
            data.Add("timestamp", GetTimeStamp().ToString());
            data.Add("mac", ZaloPayMacGenerator.GetRefundStatus(data));

            return HttpHelper.PostFormAsync(ConfigurationManager.AppSettings["ZaloPayApiGetRefundStatus"], data);
        }

        public static Task<Dictionary<string, object>> GetBankList()
        {
            var data = new Dictionary<string, string>();
            data.Add("appid", ConfigurationManager.AppSettings["Appid"]);
            data.Add("reqtime", GetTimeStamp().ToString());
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
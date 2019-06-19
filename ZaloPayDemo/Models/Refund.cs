using System.ComponentModel.DataAnnotations;

namespace ZaloPayDemo.Models
{
    public class Refund
    {
        [Key]
        public string Mrefundid { get; set; }
        public string Zptransid { get; set; }
        public long Amount { get; set; }
    }
}
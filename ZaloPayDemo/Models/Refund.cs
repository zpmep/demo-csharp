using System.ComponentModel.DataAnnotations;

namespace ZaloPayDemo.Models
{
    public class Refund
    {
        [Key]
        public string MrefundID { get; set; }
        public string ZptransID { get; set; }
        public long Amount { get; set; }
    }
}
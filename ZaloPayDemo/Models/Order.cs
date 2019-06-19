using System.ComponentModel.DataAnnotations;

namespace ZaloPayDemo.Models
{
    public class Order
    {
        [Key]
        public string Apptransid { get; set; }
        public string Zptransid { get; set; }
        public string Description { get; set; }
        public long Amount { get; set; }
        public long Timestamp { get; set; }
        public int Status { get; set; }
        public int Channel { get; set; }
    }
}
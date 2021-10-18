using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace UI.Models
{
    public partial class TblEVoucher
    {
        public int Id { get; set; }
        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; }
        [StringLength(300, MinimumLength = 10)]
        [Required]
        public string Description { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string Image { get; set; }
        public decimal Amount { get; set; }
        public int PaymentMethodId { get; set; }
        public int Quantity { get; set; }
        public int BuyTypeId { get; set; }
        public bool? InActive { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual TblBuyType BuyType { get; set; }
        public virtual TblPaymentMethod PaymentMethod { get; set; }

        public TblEVoucher()
        {
            //PaymentMethod = new TblPaymentMethod();
            //BuyType = new TblBuyType();
        }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace UI.Models
{
    public partial class TblPaymentMethod
    {
        public TblPaymentMethod()
        {
           // TblEVouchers = new HashSet<TblEVoucher>();
        }

        public int Id { get; set; }
        public string MethodName { get; set; }

        //public virtual ICollection<TblEVoucher> TblEVouchers { get; set; }
    }
}

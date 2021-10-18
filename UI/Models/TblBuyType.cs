using System;
using System.Collections.Generic;

#nullable disable

namespace UI.Models
{
    public partial class TblBuyType
    {
        public TblBuyType()
        {
            //TblEVouchers = new HashSet<TblEVoucher>();
        }

        public int Id { get; set; }
        public string TypeName { get; set; }

       // public virtual ICollection<TblEVoucher> TblEVouchers { get; set; }
    }
}

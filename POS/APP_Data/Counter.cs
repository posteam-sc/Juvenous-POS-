//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POS.APP_Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Counter
    {
        public Counter()
        {
            this.DailyRecords = new HashSet<DailyRecord>();
            this.DeleteLogs = new HashSet<DeleteLog>();
            this.PurchaseDeleteLogs = new HashSet<PurchaseDeleteLog>();
            this.Transactions = new HashSet<Transaction>();
            this.UsePrePaidDebts = new HashSet<UsePrePaidDebt>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public Nullable<bool> IsDelete { get; set; }
    
        public virtual ICollection<DailyRecord> DailyRecords { get; set; }
        public virtual ICollection<DeleteLog> DeleteLogs { get; set; }
        public virtual ICollection<PurchaseDeleteLog> PurchaseDeleteLogs { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<UsePrePaidDebt> UsePrePaidDebts { get; set; }
    }
}

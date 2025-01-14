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
    
    public partial class ReceiptDataLog
    {
        public int Id { get; set; }
        public string TransactionId { get; set; }
        public string InvoiceName { get; set; }
        public string InvoiceDate { get; set; }
        public string BranchName { get; set; }
        public string PatientName { get; set; }
        public string PatientPhoneNo { get; set; }
        public string Address { get; set; }
        public string ShopName { get; set; }
        public string CounterName { get; set; }
        public string CashierName { get; set; }
        public decimal TotalSubTotal { get; set; }
        public decimal SubTotalPriceAfterDis { get; set; }
        public Nullable<decimal> PremiumDiscount { get; set; }
        public Nullable<decimal> LoyaltyDiscount { get; set; }
        public Nullable<decimal> AdditionalDiscount { get; set; }
        public Nullable<decimal> TotalDiscount { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public string Total { get; set; }
        public Nullable<decimal> PreviousOutstanding { get; set; }
        public Nullable<decimal> Netpayable { get; set; }
        public string PaidAmount { get; set; }
        public string Balance { get; set; }
        public string Note { get; set; }
        public string Change { get; set; }
        public string PaymentType { get; set; }
    
        public virtual Transaction Transaction { get; set; }
    }
}

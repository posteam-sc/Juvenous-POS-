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
    
    public partial class PurchasedPackageSharer
    {
        public int Id { get; set; }
        public string PackageInvoiceId { get; set; }
        public int PackageOwnerId { get; set; }
        public int SharedCustomerId { get; set; }
        public System.DateTime SharedDateTime { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<bool> IsExported { get; set; }
    
        public virtual Customer Customer { get; set; }
        public virtual Customer Customer1 { get; set; }
        public virtual PackagePurchasedInvoice PackagePurchasedInvoice { get; set; }
    }
}
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
    
    public partial class ImportExportLog
    {
        public ImportExportLog()
        {
            this.ImportExportLogDetails = new HashSet<ImportExportLogDetail>();
        }
    
        public long Id { get; set; }
        public string ProcessingBatch { get; set; }
        public Nullable<System.DateTime> ProcessingDateTime { get; set; }
        public Nullable<System.DateTime> LastProcessingDateTime { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string ExportedId { get; set; }
        public string ExportedType { get; set; }
    
        public virtual ICollection<ImportExportLogDetail> ImportExportLogDetails { get; set; }
    }
}

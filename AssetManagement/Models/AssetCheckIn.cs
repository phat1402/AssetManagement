//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AssetManagement.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AssetCheckIn
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> CheckInDate { get; set; }
        public Nullable<int> AssetId { get; set; }
        public Nullable<int> AssignedTo { get; set; }
        public string Note { get; set; }
    
        public virtual Asset Asset { get; set; }
        public virtual Staff Staff { get; set; }
    }
}

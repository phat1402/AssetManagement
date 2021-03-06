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
    
    public partial class Asset
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Asset()
        {
            this.Asset_Disposal = new HashSet<Asset_Disposal>();
            this.AssetCheckIns = new HashSet<AssetCheckIn>();
            this.AssetTransfers = new HashSet<AssetTransfer>();
            this.AssetCheckOuts = new HashSet<AssetCheckOut>();
        }
    
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<int> StatusId { get; set; }
        public Nullable<int> CreateById { get; set; }
        public Nullable<int> UsedById { get; set; }
        public Nullable<decimal> UnitPrice { get; set; }
        public string Tag { get; set; }
        public Nullable<int> StoreId { get; set; }
    
        public virtual Department Department { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Asset_Disposal> Asset_Disposal { get; set; }
        public virtual Location Location { get; set; }
        public virtual Store Store { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual Vendor Vendor { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssetCheckIn> AssetCheckIns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssetTransfer> AssetTransfers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssetCheckOut> AssetCheckOuts { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual Staff Staff1 { get; set; }
    }
}

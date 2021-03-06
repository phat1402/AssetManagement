﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AssetManagement.Models.ViewModel
{
    public class CreatNewAssetViewModel
    {
        [Required(ErrorMessage = "Asset name is required")]
        public string AssetName { get; set; }

        [Required(ErrorMessage = "Asset Category is required")]
        public int AssetSubCategoryId { get; set; }

        [Required(ErrorMessage = "Asset quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public int PurchaseAmount { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int VendorId { get; set; }

        public string AssetDescription { get; set; }

        public int DepartmentId { get; set; }

        public int LocationId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Please enter a value bigger than 0")]
        public decimal UnitOfPrice { get; set; }

        public int CreatedBy { get; set; }

        public int UsedBy { get; set; }
    }
}
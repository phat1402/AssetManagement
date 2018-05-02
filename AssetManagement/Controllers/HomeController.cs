using AssetManagement.Models;
using AssetManagement.Models.Common;
using AssetManagement.Models.Dashboard;
using AssetManagement.Models.Error;
using AssetManagement.Models.Setting;
using AssetManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Web;
using System.Web.Mvc;
using static AssetManagement.Models.Common.EnumList;

namespace AssetManagement.Controllers
{
    public class HomeController : Controller
    {
        AssetManagementEntities Db = new AssetManagementEntities();

        public ActionResult Index()
        {
            int numberOfAsset = Db.Assets.Count();
            var assetUnitPriceList = Db.Assets.Select(a => a.UnitPrice).ToList();
            decimal assetTotalValue = assetUnitPriceList.Sum(value => value).Value;
            DashboardViewModel model = new DashboardViewModel()
            {
                NumberOfAsset = numberOfAsset,
                TotalValue = assetTotalValue
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult GetDataForDashboard()
        {
            var categoryList = Db.Assets.Join(Db.SubCategories,
                                    asset => asset.SubCategoryId,
                                    subcategory => subcategory.ID,
                                    (asset, subcategory) => new { asset, subcategory })
                               .Join(Db.Categories,
                                    as_sub => as_sub.subcategory.CategoryId,
                                    ca => ca.ID,
                                    (as_sub, ca) => new { as_sub.asset.ID, ca.Name })
                               .GroupBy(a => a.Name)
                               .Select(item =>
                                            new CategoryDetail
                                            {
                                                CategoryName = item.Key,
                                                CategoryCount = item.Count()
                                            }
                                       )
                               .ToList();
            var departmentList = Db.Assets.Join(Db.Departments,
                                                  a => a.DepartmentId,
                                                  d => d.ID,
                                                  (a, d) => new { a.ID, d.Name })
                                            .GroupBy(x => x.Name)
                                            .Select(item =>
                                                        new DepartmentDetail
                                                        {
                                                            DepartmentName = item.Key,
                                                            DepartmentCount = item.Count()
                                                        })
                                            .ToList();
            var vendorList = Db.Assets.Join(Db.Vendors,
                                              a => a.VendorId,
                                              v => v.ID,
                                              (a, v) => new { a.ID, v.Name })
                                         .GroupBy(x => x.Name)
                                         .Select(item =>
                                                      new VendorDetail
                                                      {
                                                          VendorName = item.Key,
                                                          VendorCount = item.Count()
                                                      })
                                         .ToList();
            DataList dataList = new DataList()
            {
                CategoryDataList = categoryList,
                DepartmentDataList = departmentList,
                VendorDataList = vendorList
            };

            return Json(dataList);
        }

        public ActionResult CreatNewAsset()
        {
            return View("~/Views/Asset/CreatingNewAsset.cshtml");
        }

        private string GenerateAssetTag(int assetId)
        {
            var assetTag = String.Format("{0:D9}", assetId);
            return assetTag;
        }
        [HttpPost]
        public ActionResult CreateNewAsset(CreatNewAssetViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int numberOfAsset = model.PurchaseAmount;
                    List<Asset> assetList = new List<Asset>();
                    for (int i = 0; i < numberOfAsset; i++)
                    {
                        Asset asset = new Asset
                        {
                            Name = model.AssetName,
                            Description = model.AssetDescription,
                            PurchaseDate = model.PurchaseDate,
                            SubCategoryId = model.AssetSubCategoryId,
                            VendorId = model.VendorId,
                            LocationId = model.LocationId,
                            DepartmentId = model.DepartmentId,
                            UnitPrice = model.UnitOfPrice,
                            CreateById = model.CreatedBy,
                            UsedById = model.UsedBy != 0 ? model.UsedBy : default(int?),
                            StatusId = (int)EnumList.AssetStatus.Active
                        };
                        assetList.Add(asset);
                    }
                    Db.Assets.AddRange(assetList);
                    Db.SaveChanges();
                    foreach (var recentAddAsset in assetList)
                    {
                        var entity = Db.Assets.Find(recentAddAsset.ID);
                        entity.Tag = GenerateAssetTag(recentAddAsset.ID);
                    }
                    Db.SaveChanges();
                    return RedirectToAction("ViewAssetList");
                }
                return View("~/Views/Asset/CreatingNewAsset.cshtml");
            }
            catch (Exception e)
            {
                var error = new ErrorViewModel
                {
                    ErrorTitle = " Something wrong",
                    ErrorMessage = e.Message
                };
                return View("~/Views/Error/Page500.cshtml", error);
            }
        }

        public ActionResult ViewAssetList()
        {
            var assetList = Db.Assets.Select(x => new AssetListViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                SubCategory = x.SubCategory.Name,
                Category = x.SubCategory.Category.Name,
                StatusID = x.StatusId.Value,
                UsedByFullname = x.Staff1.Firstname + " " + x.Staff1.Lastname       
            }).ToList();
            return View("~/Views/Asset/AssetList.cshtml", assetList);
        }

        public ActionResult ViewAssetDetail(int assetId)
        {
            var transferHistory = Db.AssetTransfers.Where(x => x.AssetID == assetId).Select(x => new AssetTransferHistoryViewModel
            {
                ID = x.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                TransferDate = x.TransferDate.Value,
                FromEmployee = x.Staff.Firstname + " " + x.Staff.Lastname,
                ToEmployee = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID).ToList();

            var lastCheckIn = Db.AssetCheckIns.Where(x => x.AssetId == assetId).Select(x => new LastAssetCheckInViewModel
            {
                ID = x.ID,
                AssetID = x.Asset.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                CheckInDate = x.CheckInDate.Value,
                AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID).ToList();

            var lastCheckOut = Db.AssetCheckOuts.Where(x => x.AssetId == assetId).Select(x => new LastAssetCheckOutViewModel
            {
                ID = x.ID,
                AssetID = x.Asset.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                CheckOutDate = x.CheckOutDate.Value,
                AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID).ToList();

            var assetInfor = Db.Assets.Where(x => x.ID == assetId).FirstOrDefault();

            var viewModel = new AssetDetailViewModel
            {
                AssetID = assetInfor.ID,
                AssetName = assetInfor.Name,
                AssetDescription = assetInfor.Description,
                PurchaseDate = assetInfor.PurchaseDate,
                SubCategory = assetInfor.SubCategory.Name,
                Category = assetInfor.SubCategory.Category.Name,
                Vendor = assetInfor.Vendor.Name,
                Department = assetInfor.Department.Name,
                Location = assetInfor.Location.Name,
                StatusId = assetInfor.StatusId,
                CreatedBy = assetInfor.CreateById != null? assetInfor.Staff.Firstname + " " + assetInfor.Staff.Lastname : "",
                UsedBy = assetInfor.UsedById != null ? assetInfor.Staff1.Firstname + " " + assetInfor.Staff1.Lastname : "" ,
                UnitOfPrice = assetInfor.UnitPrice,
                AssetTag = assetInfor.Tag,
                TransferHistory = transferHistory,
                CheckInHistory = lastCheckIn,
                CheckOutHistory = lastCheckOut
            };

            return View("~/Views/Asset/AssetDetail.cshtml", viewModel);
        }

        public ActionResult ViewEditAssetPage(int assetId)
        {
            var asset = Db.Assets.Where(x => x.ID == assetId).Select(x => new AssetEditViewModel
            {
                AssetID = x.ID,
                AssetDescription = x.Description,
                AssetName = x.Name,
                AssetTag = x.Tag,
                CreatedBy = x.CreateById,
                CreatedByName = x.Staff.Firstname + " " + x.Staff.Lastname,
                DepartmentId = x.DepartmentId,
                DepartmentName = x.Department.Name,
                LocationId = x.LocationId,
                LocationName = x.Location.Name,
                PurchaseDate = x.PurchaseDate,
                StatusId = x.StatusId,
                SubCategoryId = x.SubCategoryId,
                SubCategoryName = x.SubCategory.Name,
                UnitOfPrice = x.UnitPrice,
                UsedBy = x.UsedById,
                UsedByName = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                VendorId = x.VendorId,
                VendorName = x.Vendor.Name
            }).FirstOrDefault();
            return View("~/Views/Asset/AssetDetailEdit.cshtml", asset);
        }
        [HttpPost]
        public ActionResult EditAssetDetail(AssetEditViewModel viewmodel)
        {
            var asset = Db.Assets.Find(viewmodel.AssetID);
            asset.LocationId = viewmodel.LocationId;
            asset.DepartmentId = viewmodel.DepartmentId;
            asset.Description = viewmodel.AssetDescription;
            asset.Name = viewmodel.AssetName;
            asset.PurchaseDate = viewmodel.PurchaseDate;
            asset.VendorId = viewmodel.VendorId;
            asset.SubCategoryId = viewmodel.SubCategoryId;
            asset.UnitPrice = viewmodel.UnitOfPrice;

            if( Db.SaveChanges() > 0)
            {
                return ViewAssetDetail(asset.ID);
            }
            else
            {
                var error = new ErrorViewModel
                {
                    ErrorMessage = "Can not edit asset detail!"
                };
                return View("~/Views/Error/Page500.cshtml", error);
            }
        }

        public ActionResult ViewVendorList()
        {
            var vendorList = Db.Vendors.Select(x => new VendorListViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Email = x.Email,
                PhoneNo = x.TelephoneNo
            }).ToList();
            return View("~/Views/Vendor/VendorList.cshtml", vendorList);
        }

        public ActionResult CreateNewVendor()
        {
            return View("~/Views/Vendor/CreateNewVendor.cshtml");
        }

        public ActionResult DeleteVendor(int vendorId)
        {
            var vendor = Db.Vendors.Find(vendorId);
            Db.Vendors.Remove(vendor);

            var assetList = Db.Assets.Where(x => x.VendorId == vendorId).ToList();
            foreach(var asset in assetList)
            {
                asset.VendorId = null;
            }

            if(Db.SaveChanges()> 0)
            {
                return Json("Success");
            }

            return Json("Fail");
        }

        [HttpPost]
        public ActionResult CreateOrUpdateVendor(Vendor model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var vendor = Db.Vendors.Find(model.ID);
                    if (vendor != null)
                    {
                        vendor.Name = model.Name;
                        vendor.Email = model.Email;
                        vendor.TelephoneNo = model.TelephoneNo;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = vendor.ID,
                                Name = vendor.Name,
                                Email = vendor.Email,
                                TelephoneNo = vendor.TelephoneNo
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = "",
                                Email = "",
                                TelephoneNo = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = "",
                        Email = "",
                        TelephoneNo = ""
                    });
                }
                else
                {
                    var newVendor = new Vendor();
                    newVendor.Name = model.Name;
                    newVendor.Email = model.Email;
                    newVendor.TelephoneNo = model.TelephoneNo;
                    Db.Vendors.Add(newVendor);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully",
                            ID = newVendor.ID,
                            Name = newVendor.Name,
                            Email = newVendor.Email,
                            TelephoneNo = newVendor.TelephoneNo
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!",
                            ID = 0,
                            Name = "",
                            Email = "",
                            TelephoneNo = ""
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        public ActionResult ViewCategoryList()
        {
            var categoryList = Db.Categories.Select(x => new CategoryListViewModel
            {
                ID = x.ID,
                Name = x.Name
            }).ToList();
            return View("~/Views/CategoryAndSub/CategoryList.cshtml", categoryList);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateCategory(Category model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var category = Db.Categories.Find(model.ID);
                    if (category != null)
                    {
                        category.Name = model.Name;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = category.ID,
                                Name = category.Name
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newCategory = new Category();
                    newCategory.Name = model.Name;
                    Db.Categories.Add(newCategory);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully",
                            ID = newCategory.ID,
                            Name = newCategory.Name
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!",
                            ID = newCategory.ID,
                            Name = newCategory.Name
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            var subCategoryIdList = Db.SubCategories.Where(x => x.CategoryId == categoryId).Select(x => x.ID).ToList();
            var assetList = Db.Assets.Where(x => subCategoryIdList.Contains(x.SubCategoryId.Value)).ToList();
            var assetIdList = assetList.Select(x => x.ID);
            
            // Delete Asset Transfer
            var assetTransferList = Db.AssetTransfers.Where(x => assetIdList.Contains(x.AssetID.Value)).ToList();
            foreach( var transfer in assetTransferList)
            {
                Db.AssetTransfers.Remove(transfer);
            }

            // Delete Asset Check in
            var assetCheckInList = Db.AssetCheckIns.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach(var checkin in assetCheckInList)
            {
                Db.AssetCheckIns.Remove(checkin);
            }

            // Delete Asset Check Out
            var assetCheckOutList = Db.AssetCheckOuts.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach(var checkout in assetCheckOutList)
            {
                Db.AssetCheckOuts.Remove(checkout);
            }

            //Delete Asset Disposal
            var assetDisposal = Db.Asset_Disposal.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach(var disposal in assetDisposal)
            {
                Db.Asset_Disposal.Remove(disposal);
            }

            // Delete Asset
            foreach( var asset in assetList)
            {
                Db.Assets.Remove(asset);
            }

            // Delete all subcategory
            var subcategoryList = Db.SubCategories.Where(x => subCategoryIdList.Contains(x.ID)).ToList();
            foreach( var sub in subcategoryList)
            {
                Db.SubCategories.Remove(sub);
            }

            //Delete category
            var category = Db.Categories.Find(categoryId);
            Db.Categories.Remove(category);

            if(Db.SaveChanges()> 0)
            {
                return Json("Success");
            }

            return Json("Fail");
        
        }

        public ActionResult ViewSubCategoryList(int categoryId)
        {
            var subCategoryList = Db.SubCategories.Where(x => x.CategoryId == categoryId).Select(x => new SubCategoryViewModel
            {
                ID = x.ID,
                Name = x.Name,
                CategoryId = x.CategoryId.Value,
                CategoryName = x.Category.Name

            }).ToList();

            return View("~/Views/CategoryAndSub/SubCategoryList.cshtml", subCategoryList);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateSubCategory(SubCategory model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var subcategory = Db.SubCategories.Find(model.ID);
                    if (subcategory != null)
                    {
                        subcategory.Name = model.Name;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = subcategory.ID,
                                Name = subcategory.Name
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newSubCategory = new SubCategory();
                    newSubCategory.Name = model.Name;
                    newSubCategory.CategoryId = model.CategoryId;
                    Db.SubCategories.Add(newSubCategory);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully",
                            ID = newSubCategory.ID,
                            Name = newSubCategory.Name
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!",
                            ID = newSubCategory.ID,
                            Name = newSubCategory.Name
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteSubCategory(int subCategoryId)
        {
            var assetList = Db.Assets.Where(x => x.SubCategoryId == subCategoryId).ToList();
            var assetIdList = assetList.Select(x => x.ID);

            // Delete Asset Transfer
            var assetTransferList = Db.AssetTransfers.Where(x => assetIdList.Contains(x.AssetID.Value)).ToList();
            foreach (var transfer in assetTransferList)
            {
                Db.AssetTransfers.Remove(transfer);
            }

            // Delete Asset Check in
            var assetCheckInList = Db.AssetCheckIns.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach (var checkin in assetCheckInList)
            {
                Db.AssetCheckIns.Remove(checkin);
            }

            // Delete Asset Check Out
            var assetCheckOutList = Db.AssetCheckOuts.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach (var checkout in assetCheckOutList)
            {
                Db.AssetCheckOuts.Remove(checkout);
            }

            //Delete Asset Disposal
            var assetDisposal = Db.Asset_Disposal.Where(x => assetIdList.Contains(x.AssetId.Value)).ToList();
            foreach (var disposal in assetDisposal)
            {
                Db.Asset_Disposal.Remove(disposal);
            }

            // Delete Asset
            foreach (var asset in assetList)
            {
                Db.Assets.Remove(asset);
            }

            // Delete all subcategory
            var subCategory = Db.SubCategories.Find(subCategoryId);
            Db.SubCategories.Remove(subCategory);


            if (Db.SaveChanges() > 0)
            {
                return Json("Success");
            }

            return Json("Fail");

        }

        public ActionResult ViewAssetFromVendor(int vendorId)
        {
            var vendorName = Db.Vendors.Where(x => x.ID == vendorId).Select(x => x.Name).FirstOrDefault();
            var assetList = Db.Assets.Where(x => x.VendorId == vendorId)
                              .Select(x => new AssetListViewModel
                              {
                                  ID = x.ID,
                                  Name = x.Name,
                                  SubCategory = x.SubCategory.Name,
                                  Category = x.SubCategory.Category.Name,
                                  StatusID = x.StatusId ?? 0
                              }).ToList();
            AssetFromVendorViewModel viewModel = new AssetFromVendorViewModel {
                VendorName = vendorName,
                AssetList = assetList
            };
            return View("~/Views/Vendor/AssetFromVendorList.cshtml", viewModel);
        }

        public ActionResult ViewAssetTransfer()
        {
            var transferHistoryList = Db.AssetTransfers.Select(x => new AssetTransferHistoryViewModel {
                ID = x.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                TransferDate = x.TransferDate.Value,
                FromEmployee = x.Staff.Firstname + " " + x.Staff.Lastname,
                ToEmployee = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID)
            .ToList();
            var viewModel = new AssetTransferViewModel
            {
                TransferHistoryList = transferHistoryList
            };
            return View("~/Views/Asset/AssetTransfer.cshtml", viewModel);
        }

        public ActionResult ViewAssetCheckIn()
        {
            var lastCheckIn = Db.AssetCheckIns.Select(x => new LastAssetCheckInViewModel
            {
                ID = x.ID,
                AssetID = x.Asset.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                CheckInDate = x.CheckInDate.Value,
                AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID).ToList();
            var viewModel = new AssetCheckInViewModel {
                LastCheckIn = lastCheckIn
            };

            return View("~/Views/Asset/AssetCheckIn.cshtml", viewModel);
        }

        public ActionResult ViewAssetCheckOut()
        {
            var lastCheckOut = Db.AssetCheckOuts.Select(x => new LastAssetCheckOutViewModel
            {
                ID = x.ID,
                AssetID = x.Asset.ID,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                CheckOutDate = x.CheckOutDate.Value,
                AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                Note = x.Note
            }).OrderByDescending(x => x.ID).ToList();
            var viewModel = new AssetCheckOutViewModel
            {
                LastCheckOut = lastCheckOut
            };
            return View("~/Views/Asset/AssetCheckOut.cshtml", viewModel);
        }

        public ActionResult ViewAssetDisposal()
        {
            var disposalList = Db.Asset_Disposal.Select(x => new AssetDisposalItem
            {
                ID = x.ID,
                AssetID = x.AssetId.Value,
                AssetName = x.Asset.Name,
                AssetTag = x.Asset.Tag,
                DisposalDate = x.DisposalDate.Value,
                DiposedBy = x.Staff.Firstname + " " + x.Staff.Lastname,
                Comment = x.Comment
            }).OrderByDescending(x => x.ID).ToList();

            var viewModel = new AssetDisposalViewModel
            {
                DisposalHistory = disposalList
            };
            return View("~/Views/Asset/AssetDisposal.cshtml", viewModel);
        }

        [HttpPost]
        public ActionResult CreateAssetTransfer( AssetTransferViewModel model) 
        {
            var isExisted = Db.AssetTransfers.Any(x => x.AssetID == model.AssetID && x.FromEmployeeId == model.FromEmployeeId && x.ToEmployeeId == model.ToEmployeeId);
            if (isExisted)
            {
                return Json("Existed");
            }
            if (ModelState.IsValid)
            {
                var newTransfer = new AssetTransfer()
                {
                    AssetID = model.AssetID,
                    TransferDate = model.TransferDate,
                    FromEmployeeId = model.FromEmployeeId,
                    ToEmployeeId = model.ToEmployeeId,
                    Note = model.Note
                };
                Db.AssetTransfers.Add(newTransfer);

                //Update asset table
                var asset = Db.Assets.Find(model.AssetID);
                asset.UsedById = model.ToEmployeeId;

                if (Db.SaveChanges() > 0)
                {
                    var transferHistoryList = Db.AssetTransfers.Select(x => new AssetTransferHistoryViewModel
                    {
                        ID = x.ID,
                        AssetName = x.Asset.Name,
                        AssetTag = x.Asset.Tag,
                        TransferDate = x.TransferDate.Value,
                        FromEmployee = x.Staff.Firstname + " " + x.Staff.Lastname,
                        ToEmployee = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                        Note = x.Note
                    }).OrderByDescending(x => x.ID).ToList();
                    return PartialView("~/Views/Asset/AssetTransferHistory.cshtml", transferHistoryList);
                }
                else
                {
                    var error = new ErrorViewModel
                    {
                        ErrorMessage = "Can not transfer the asset! "
                    };
                    return PartialView("~/Views/Error/Page500.cshtml", error);
                }
            }
            return View("~/Views/Asset/AssetTransfer.cshtml");
        }

        [HttpPost]
        public ActionResult CheckInAsset(AssetCheckInViewModel model)
        {
            var isExisted = Db.AssetCheckIns.Any(x => x.AssetId == model.AssetID && x.AssignedTo == model.StaffID);
            if (isExisted) {
                return Json("Existed");
            }
            var newCheckIn = new AssetCheckIn
            {
                AssetId = model.AssetID,
                AssignedTo = model.StaffID,
                Note = model.Note,
                CheckInDate = model.CheckInDate
            };
            Db.AssetCheckIns.Add(newCheckIn);
            if (Db.SaveChanges() > 0)
            {
                var lastCheckIn = Db.AssetCheckIns.Select(x => new LastAssetCheckInViewModel
                {
                    ID = x.ID,
                    AssetID = x.Asset.ID,
                    AssetName = x.Asset.Name,
                    AssetTag = x.Asset.Tag,
                    CheckInDate = x.CheckInDate.Value,
                    AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                    Note = x.Note
                }).OrderByDescending(x => x.ID).ToList();
                return PartialView("~/Views/Asset/AssetCheckInHistory.cshtml", lastCheckIn);
            }
            else
            {
                var error = new ErrorViewModel
                {
                    ErrorMessage = "Can not check in asset !"
                };
                return PartialView("~/Views/Error/Page500.cshtml", error);
            }
        }

        [HttpPost]
        public ActionResult CheckOutAsset(AssetCheckOutViewModel model)
        {
            var isExisted = Db.AssetCheckOuts.Any(x => x.AssetId == model.AssetID && x.WhoTake == model.StaffID);
            if (isExisted)
            {
                return Json("Existed");
            }
            var newCheckOut = new AssetCheckOut
            {
                AssetId = model.AssetID,
                WhoTake = model.StaffID,
                Note = model.Note,
                CheckOutDate = model.CheckOutDate
            };
            Db.AssetCheckOuts.Add(newCheckOut);
            if (Db.SaveChanges() > 0)
            {
                var lastCheckOut = Db.AssetCheckOuts.Select(x => new LastAssetCheckOutViewModel
                {
                    ID = x.ID,
                    AssetID = x.Asset.ID,
                    AssetName = x.Asset.Name,
                    AssetTag = x.Asset.Tag,
                    CheckOutDate = x.CheckOutDate.Value,
                    AssignToName = x.Staff.Firstname + x.Staff.Lastname,
                    Note = x.Note
                }).OrderByDescending(x => x.ID).ToList();
                return PartialView("~/Views/Asset/AssetCheckOutHistory.cshtml", lastCheckOut);
            }
            else
            {
                var error = new ErrorViewModel
                {
                    ErrorMessage = "Can not check out asset !"
                };
                return PartialView("~/Views/Error/Page500.cshtml", error);
            }
        }

        [HttpPost]
        public ActionResult DisposeAsset(AssetDisposalViewModel model)
        {
            var isExisted = Db.Assets.Any(x => x.ID == model.AssetID && x.StatusId == (int)AssetStatus.Disposal);
            if (isExisted)
            {
                return Json("Existed");
            }
            var newDisposal = new Asset_Disposal()
            {
                AssetId = model.AssetID,
                Comment = model.Comment,
                DiposeById = model.DisposedBy,
                DisposalDate = model.DisposalDate
            };

            Db.Asset_Disposal.Add(newDisposal);

            var asset = Db.Assets.Find(model.AssetID);
            asset.StatusId = (int) EnumList.AssetStatus.Disposal;

            if( Db.SaveChanges() > 0)
            {
                var disposalList = Db.Asset_Disposal.Select(x => new AssetDisposalItem
                {
                    ID = x.ID,
                    AssetID = x.AssetId.Value,
                    AssetName = x.Asset.Name,
                    AssetTag = x.Asset.Tag,
                    DisposalDate = x.DisposalDate.Value,
                    DiposedBy = x.Staff.Firstname + " " + x.Staff.Lastname,
                    Comment = x.Comment
                }).OrderByDescending(x => x.ID).ToList();
                return PartialView("~/Views/Asset/AssetDisposalHistory.cshtml", disposalList);
            }
            else
            {
                var error = new ErrorViewModel
                {
                    ErrorMessage = "Can not dispose asset!"
                };
                return PartialView("~/Views/Error/Page500.cshtml",error);
            }
        }

        #region Report
        public ActionResult GetAssetByEmployee()
        {
            var assetList = Db.Assets.Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                UsedByFullname = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                StatusID = x.StatusId.Value
            }).ToList();
            return View("~/Views/Report/AssetByEmployee.cshtml", assetList);
        }

        public ActionResult FilterAssetByEmployee (int employeeId)
        {
            var assetList = Db.Assets.Where(x => x.UsedById == employeeId)
                .Select(x => new ReportAssetViewModel
                {
                    ID = x.ID,
                    Tag = x.Tag,
                    UsedByFullname = x.Staff1.Firstname + " " + x.Staff1.Lastname,
                    Name = x.Name,
                    StatusID = x.StatusId.Value
                }).ToList();

            return PartialView("~/Views/Report/ReportTableByEmployee.cshtml", assetList);
        }

        public ActionResult GetAssetByBuilding()
        {
            var assetList = Db.Assets.Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                SubCategory = x.SubCategory.Name,
                Category = x.SubCategory.Category.Name,
                StatusID = x.StatusId.Value,
                BuildingName = x.Location.BuildingName
            }).ToList();
            return View("~/Views/Report/AssetByBuilding.cshtml", assetList) ;
        }

        public ActionResult FilterAssetByBuilding(string buildingName)
        {
            var locationList = Db.Locations.Where(x => x.BuildingName == buildingName).Select(x =>x.ID).ToList();
            var assetList = Db.Assets.Where(x => x.LocationId != null).Where(x => locationList.Contains(x.LocationId.Value))
                .Select(x => new ReportAssetViewModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Tag = x.Tag,
                    SubCategory = x.SubCategory.Name,
                    Category = x.SubCategory.Category.Name,
                    StatusID = x.StatusId.Value,
                    BuildingName = x.Location.BuildingName
                }).ToList();

            return PartialView("~/Views/Report/ReportTableByBuilding.cshtml", assetList);
        }

        public ActionResult GetAssetByDepartment()
        {
            var assetList = Db.Assets.Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                DepartmentName = x.Department.Name,
                StatusID = x.StatusId.Value
            }).ToList();
            return View("~/Views/Report/AssetByDepartment.cshtml", assetList);
        }
        
        public ActionResult FilterAssetByDepartment(int departmentId)
        {
            var assetList = Db.Assets.Where(x => x.DepartmentId == departmentId)
                              .Select(x => new ReportAssetViewModel
                                {
                                  ID = x.ID,
                                  Name = x.Name,
                                  Tag = x.Tag,
                                  DepartmentName = x.Department.Name,
                                  StatusID = x.StatusId.Value
                              }).ToList();

            return PartialView("~/Views/Report/ReportTableByDepartment.cshtml", assetList);
        }

        public ActionResult GetAssetByCategory()
        {
            var assetList = Db.Assets.Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                SubCategory = x.SubCategory.Name,
                Category = x.SubCategory.Category.Name,
                StatusID = x.StatusId.Value
            }).ToList();
            return View("~/Views/Report/AssetByCategory.cshtml", assetList);
        }

        public ActionResult FilterAssetByCategory(int categoryId)
        {
            var subCategoryList = Db.SubCategories.Where(x => x.CategoryId == categoryId).Select(x => x.ID).ToList();
            var assetList = Db.Assets.Where(x => x.SubCategoryId != null).Where(x => subCategoryList.Contains(x.SubCategoryId.Value))
                              .Select(x => new ReportAssetViewModel
                              {
                                  ID = x.ID,
                                  Name = x.Name,
                                  Tag = x.Tag,
                                  SubCategory = x.SubCategory.Name,
                                  Category = x.SubCategory.Category.Name,
                                  StatusID = x.StatusId.Value
                              }).ToList();

            return PartialView("~/Views/Report/ReportTableByCategory.cshtml", assetList);
        }

        public ActionResult GetAssetByCategoAndDept()
        {
            var assetList = Db.Assets.Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                SubCategory = x.SubCategory.Name,
                Category = x.SubCategory.Category.Name,
                StatusID = x.StatusId.Value,
                DepartmentName = x.Department.Name
            }).ToList();
            return View("~/Views/Report/AssetByCategoAndDept.cshtml", assetList);
        }

        public ActionResult FilterAssetByCategoAndDept(int departmentId, int categoryId)
        {
            var subCategoryList = Db.SubCategories.Where(x => x.CategoryId == categoryId).Select(x => x.ID).ToList();
            var assetList = Db.Assets.Where(x => x.SubCategoryId != null && x.DepartmentId != null).Where(x => subCategoryList.Contains(x.SubCategoryId.Value) && x.DepartmentId == departmentId)
                  .Select(x => new ReportAssetViewModel
                  {
                      ID = x.ID,
                      Name = x.Name,
                      Tag = x.Tag,
                      SubCategory = x.SubCategory.Name,
                      Category = x.SubCategory.Category.Name,
                      StatusID = x.StatusId.Value,
                      DepartmentName = x.Department.Name
                  }).ToList();
            return PartialView("~/Views/Report/ReportTableByCategoAndDept.cshtml", assetList);
        }

        public ActionResult GetAssetByStore()
        {
            var assetList = Db.Assets.Where(x => x.StoreId.HasValue).Select(x => new ReportAssetViewModel
            {
                ID = x.ID,
                Name = x.Name,
                Tag = x.Tag,
                SubCategory = x.SubCategory.Name,
                Category = x.SubCategory.Category.Name,
                StatusID = x.StatusId.Value,
                StoreName = x.Store.Name
            }).ToList();
            return View("~/Views/Report/AssetByStore.cshtml", assetList);
        }

        public ActionResult FilterAssetByStore(int storeId)
        {
            var assetList = Db.Assets.Where(x => x.StoreId == storeId)
                .Select(x => new ReportAssetViewModel
                {
                    ID = x.ID,
                    Tag = x.Tag,
                    StoreName = x.Store.Name,
                    Name = x.Name,
                    StatusID = x.StatusId.Value
                }).ToList();

            return PartialView("~/Views/Report/ReportTableByStore.cshtml", assetList);
        }


        #endregion
        public ActionResult GetEditAssetInfor(int assetId)
        {
            var assetInfor = Db.Assets.Where(x => x.ID == assetId).FirstOrDefault();
            var viewModel = new AssetEditViewModel()
            {
                AssetID = assetInfor.ID,
                AssetDescription = assetInfor.Description,
                AssetName = assetInfor.Name,
                AssetTag = assetInfor.Tag,
                CreatedBy = assetInfor.CreateById.Value,
                UsedBy = assetInfor.UsedById,
                DepartmentId = assetInfor.DepartmentId,
                LocationId = assetInfor.LocationId,
                SubCategoryId = assetInfor.SubCategoryId,
                VendorId = assetInfor.VendorId,
                PurchaseDate = assetInfor.PurchaseDate,
                UnitOfPrice = assetInfor.UnitPrice,
                StatusId = assetInfor.StatusId
            };
            return View("~/Views/Asset/AssetDetailEdit.cshtml", viewModel);
        }

        #region Employee Management
        public ActionResult GetEmployeeList()
        {
            var staffList = Db.Staffs.Where(x=> x.IsDeleted == false).Select(
                x => new EmployeeListViewModel {
                    EmployeeId = x.ID,
                    FirstName = x.Firstname,
                    LastName = x.Lastname,
                    Email = x.Email,
                    Phone = x.MobileNo                  
                }
                ).OrderByDescending(x => x.EmployeeId).ToList();

            return View("~/Views/Employee/EmployeeList.cshtml",staffList);
        }

        public ActionResult GetAssetFromEmployee(int employeeId)
        {
            var firstName = Db.Staffs.Where(x => x.ID == employeeId).Select(x => x.Firstname).FirstOrDefault();
            var lastName = Db.Staffs.Where(x => x.ID == employeeId).Select(x => x.Lastname).FirstOrDefault();
            var assetList = Db.Assets.Where(x => x.UsedById == employeeId)
                              .Select(x => new AssetListViewModel
                              {
                                  ID = x.ID,
                                  Name = x.Name,
                                  SubCategory = x.SubCategory.Name,
                                  Category = x.SubCategory.Category.Name,
                                  StatusID = x.StatusId ?? 0
                              }).ToList();
            AssetFromEmployeeViewModel viewModel = new AssetFromEmployeeViewModel
            {
                EmployeeFullName = firstName + " " + lastName,
                AssetList = assetList
            };
            return View("~/Views/Employee/AssetFromEmployee.cshtml", viewModel);

        }

        [HttpPost]
        public ActionResult CreateOrUpdateEmployee(Staff model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var employee = Db.Staffs.Find(model.ID);
                    if (employee != null)
                    {
                        employee.Firstname = model.Firstname;
                        employee.Lastname = model.Lastname;
                        employee.Email = model.Email;
                        employee.MobileNo = model.MobileNo;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = employee.ID,
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newStaff = new Staff();
                    newStaff.Firstname = model.Firstname;
                    newStaff.Lastname = model.Lastname;
                    newStaff.Email = model.Email;
                    newStaff.MobileNo = model.MobileNo;
                    newStaff.CreatedDate = DateTime.Now;
                    newStaff.IsDeleted = false;
                    Db.Staffs.Add(newStaff);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!"
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteEmployee(int employeeId)
        {
            var staff = Db.Staffs.Find(employeeId);
            staff.IsDeleted = true;
            if (Db.SaveChanges() > 0) { 
                return Json("Success");
            }
            return Json("Failed");

        }
        #endregion

        #region Location
        public ActionResult ViewLocationList()
        {
            var locationList = Db.Locations.Select(
                x => new LocationListViewModel {
                    ID = x.ID,
                    BuildingName = x.BuildingName,
                    FloorNo = x.FloorNo,
                    Name = x.Name,
                    RoomNo = x.RoomNo
                }
                ).ToList();

            return View("~/Views/More/LocationList.cshtml", locationList);
        }
        [HttpPost]
        public ActionResult CreateOrUpdateLocation(Location model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var location = Db.Locations.Find(model.ID);
                    if (location != null)
                    {
                        location.Name = model.Name;
                        location.FloorNo = model.FloorNo;
                        location.RoomNo = model.RoomNo;
                        location.BuildingName = model.BuildingName;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = location.ID,
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newLocation = new Location();
                    newLocation.Name= model.Name;
                    newLocation.FloorNo = model.FloorNo;
                    newLocation.RoomNo = model.RoomNo;
                    newLocation.BuildingName = model.BuildingName;
                    Db.Locations.Add(newLocation);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!"
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteLocation(int locationId)
        {
            var assets = Db.Assets.Where(x => x.LocationId == locationId).ToList();
            foreach (var asset in assets)
            {
                asset.LocationId = null;
            }
            var location = Db.Locations.Find(locationId);
            Db.Locations.Remove(location);

            if (Db.SaveChanges() > 0)
            {
                return Json("Success");
            }
            return Json("Fail");
        }
        #endregion

        #region Department
        public ActionResult ViewDepartmentList()
        {
            var departmentList = Db.Departments.Select(
                x => new DepartmentListViewModel
                {
                    ID = x.ID,
                    Name = x.Name
                }
                ).ToList();

            return View("~/Views/More/DepartmentList.cshtml", departmentList);
        }
        [HttpPost]
        public ActionResult CreateOrUpdateDepartment(Department model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var department = Db.Departments.Find(model.ID);
                    if (department != null)
                    {
                        department.Name = model.Name;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = department.ID,
                                Name = department.Name
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newDepartment = new Department();
                    newDepartment.Name = model.Name;
                    Db.Departments.Add(newDepartment);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!"
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteDepartment(int departmentId)
        {
            var assets = Db.Assets.Where(x => x.DepartmentId == departmentId).ToList();
            foreach( var asset in assets)
            {
                asset.DepartmentId = null;
            }
            var department = Db.Departments.Find(departmentId);
            Db.Departments.Remove(department);

            if( Db.SaveChanges() > 0)
            {
                return Json("Success");
            }
            return Json("Fail");
        }
        #endregion

        public ActionResult PrintAssetLabel()
        {
            var assetList = Db.Assets.Where(x => !String.IsNullOrEmpty(x.Tag)).Select(x => new PrintLabelViewModel
            {
                AssetID = x.ID,
                AssetName = x.Name,
                AssetTag = x.Tag
            }).ToList();

            return View("~/Views/Report/PrintLabel.cshtml",assetList);
        }

        [HttpPost]
        public ActionResult GetDataToPrintLabel(List<int> assetIdList)
        {
            List<LabelPrintingModel> model = new List<LabelPrintingModel>();
            foreach(int id in assetIdList)
            {
                var assetLabel = Db.Assets.Where(x => x.ID == id)
                                   .Select(x => new LabelPrintingModel
                                   {
                                       AssetName = x.Name,
                                       AssetTag = x.Tag
                                   }).FirstOrDefault();
                model.Add(assetLabel);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        #region Setting
        public ActionResult GetFooter()
        {
            var setting = Db.CompanySettings.Select(x => new CompanySettingViewModel
            {
                CompanyDescription = x.Description,
                CompanyEmail = x.Email,
                LoginLink = x.LogoLink,
                CompanyPhone = x.Phone,
                CompanyID = x.ID,
                Address = x.Address,
                Country = x.Country
            }).FirstOrDefault();

            return PartialView("~/Views/Shared/_Footer.cshtml", setting);
        }

        public ActionResult GetLogo()
        {
            var setting = Db.CompanySettings.Select(x => new CompanySettingViewModel
            {
                CompanyDescription = x.Description,
                CompanyEmail = x.Email,
                LoginLink = x.LogoLink,
                CompanyPhone = x.Phone,
                CompanyID = x.ID
            }).FirstOrDefault();

            return PartialView("~/Views/Shared/_Logo.cshtml", setting);
        }

        public JsonResult UpdateCompanySetting(CompanySettingViewModel model)
        {
            var setting = Db.CompanySettings.Find(model.CompanyID);
            setting.Description = model.CompanyDescription;
            setting.Email = model.CompanyEmail;
            setting.Phone = model.CompanyPhone;
            setting.Address = model.Address;
            setting.Country = model.Country;
            if( Db.SaveChanges() > 0)
            {
                return Json("Update Setting Successfully!");
            }
            return Json("Update Setting Failed! ");
        }

        [HttpPost]
        public ActionResult UploadLogo(HttpPostedFileBase Content)
        {
            int defaultSettingId = 1;
            string[] validTypes = new[] { "image/jpeg", "image/png" };
            if (Content == null || Content.ContentLength == 0)
            {
                return null;
            }

            if (!validTypes.Contains(Content.ContentType))
            {
                throw new InvalidDataException("Please upload only an image of type JPG or PNG.");
            }
            else
            {
                string fileName = Path.GetFileName(Content.FileName);
                string path = Path.Combine(Server.MapPath("~/UploadedLogo"), fileName);
                Content.SaveAs(path);

                var setting = Db.CompanySettings.Find(defaultSettingId);
                setting.LogoLink = fileName;
                if (Db.SaveChanges() > 0)
                {
                    return RedirectToAction("Index");
                }
                var error = new ErrorViewModel {
                    ErrorTitle = "Upload Logo Failed",
                    ErrorMessage = "Something wrong! You can not upload new Logo"
                
                };
                return View("~/Views/Error/ErrorPage.cshtml", error);
            }
        }

        public ActionResult ViewCompanySetting()
        {
            var setting = Db.CompanySettings.Select(x => new CompanySettingViewModel
            {
                CompanyDescription = x.Description,
                CompanyEmail = x.Email,
                LoginLink = x.LogoLink,
                CompanyPhone = x.Phone,
                CompanyID = x.ID,
                Address = x.Address,
                Country = x.Country
            }).FirstOrDefault();

            return View("~/Views/CompanySetting/Setting.cshtml", setting);
        }


        #endregion

        #region Store
        public ActionResult ViewStoreList()
        {
            var storeList = Db.Stores.Select(
                x => new StoreListViewModel
                {
                    ID = x.ID,
                    Name = x.Name
                }
                ).ToList();

            return View("~/Views/More/StoreList.cshtml", storeList);
        }

        [HttpPost]
        public ActionResult CreateOrUpdateStore(Store model)
        {
            try
            {
                if (model.ID != 0)
                {
                    var store = Db.Stores.Find(model.ID);
                    if (store != null)
                    {
                        store.Name = model.Name;
                        if (Db.SaveChanges() > 0)
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Update successfully",
                                ID = store.ID,
                                Name = store.Name
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                RequestType = "Update",
                                Message = "Can not update",
                                ID = 0,
                                Name = ""
                            });
                        }
                    }
                    return Json(new
                    {
                        RequestType = "Update",
                        Message = "Can not find category",
                        ID = 0,
                        Name = ""
                    });
                }
                else
                {
                    var newStore = new Store();
                    newStore.Name = model.Name;
                    Db.Stores.Add(newStore);
                    if (Db.SaveChanges() > 0)
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Create successfully"
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            RequestType = "New",
                            Message = "Can not create new category!"
                        });
                    }
                }

            }
            catch (Exception e)
            {
                return Json(e.Message);
            }

        }

        [HttpPost]
        public ActionResult DeleteStore(int storeId)
        {
            var assets = Db.Assets.Where(x => x.StoreId == storeId).ToList();
            foreach (var asset in assets)
            {
                asset.StoreId = null;
            }
            var store = Db.Stores.Find(storeId);
            Db.Stores.Remove(store);

            if (Db.SaveChanges() > 0)
            {
                return Json("Success");
            }
            return Json("Fail");
        }

        #endregion

        #region Purchase
        public ActionResult CreateNewPurchase()
        {

            return View("~/Views/Purchase/NewPurchase.cshtml");
        }

        [HttpPost]
        public ActionResult CreateNewPurchase(CreateNewPurchaseViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    int numberOfAsset = model.PurchaseAmount;
                    List<Asset> assetList = new List<Asset>();
                    for (int i = 0; i < numberOfAsset; i++)
                    {
                        Asset asset = new Asset
                        {
                            Name = model.AssetName,
                            PurchaseDate = model.PurchaseDate,
                            SubCategoryId = model.AssetSubCategoryId,
                            VendorId = model.VendorId,
                            UnitPrice = model.UnitOfPrice,
                            StoreId = model.StoreId,
                            CreateById = model.CreatedBy,
                            StatusId = (int)EnumList.AssetStatus.Active
                        };
                        assetList.Add(asset);
                    }
                    Db.Assets.AddRange(assetList);
                    Db.SaveChanges();
                    foreach (var recentAddAsset in assetList)
                    {
                        var entity = Db.Assets.Find(recentAddAsset.ID);
                        entity.Tag = GenerateAssetTag(recentAddAsset.ID);
                    }
                    Db.SaveChanges();
                    return RedirectToAction("ViewAssetList");
                }
                return View("~/Views/Purchase/NewPurchase.cshtml");
            }
            catch (Exception e)
            {
                var error = new ErrorViewModel
                {
                    ErrorTitle = " Something wrong",
                    ErrorMessage = e.Message
                };
                return View("~/Views/Error/Page500.cshtml", error);
            }
        }
        #endregion

        public ActionResult AssignAssetFromStore()
        {
            var assetList = Db.Assets.Where(x => x.StoreId.HasValue && x.UsedById == null).Select(x => new DistributeAssetViewModel
            {
                AssetID = x.ID,
                AssetName = x.Name,
                AssetTag = x.Tag,
                StoreId = x.StoreId.Value,
                StoreName = x.Store.Name
            }).ToList();
            return View("~/Views/Asset/DistributeAsset.cshtml", assetList);
        }

        public ActionResult FilterAssetByStoreForDistribute(int storeId)
        {
            var assetList = Db.Assets.Where(x => x.StoreId == storeId && x.UsedById == null )
                .Select(x => new DistributeAssetViewModel
                {
                    AssetID = x.ID,
                    AssetTag = x.Tag,
                    StoreName = x.Store.Name,
                    AssetName = x.Name,
                    StoreId = x.StoreId.Value
                }).ToList();

            return PartialView("~/Views/Asset/DistributeAssetTable.cshtml", assetList);
        }

        [HttpPost]
        public ActionResult AssignAsset(List<int> assetIdList, int employeeId)
        {
            if( employeeId == 0 || assetIdList.Count() == 0)
            {
                return Json("Failed To Assign. Try again!");
            }
            var assetList = Db.Assets.Where(x => assetIdList.Contains(x.ID)).ToList();
            foreach(var asset in assetList)
            {
                asset.UsedById = employeeId;
            }

            if(Db.SaveChanges() > 0)
            {
                return Json("Assign Successfully");
            }
            return Json("Failed To Assign. Try again!");
        }


    }

}
using AssetManagement.Models;
using AssetManagement.Models.Common;
using AssetManagement.Models.Dashboard;
using AssetManagement.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        [HttpPost]
        public ActionResult CreateNewAsset(CreatNewAssetViewModel model)
        {
            try
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
                        UnitPrice = model.UnitOfPrice
                    };
                    assetList.Add(asset);
                }
                Db.Assets.AddRange(assetList);
                Db.SaveChanges();
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json(e.Message);
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
                StatusID = x.StatusId.Value
            }).ToList();
            return View("~/Views/Asset/AssetList.cshtml", assetList);
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

        public ActionResult ViewSubCategoryList(int id)
        {
            var subCategoryList = Db.SubCategories.Where(x => x.CategoryId == id).Select(x => new SubCategoryViewModel
            {
                ID = x.ID,
                Name = x.Name,
                CategoryId = x.CategoryId.Value
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
            return View("~/Views/Asset/AssetDisposal.cshtml");
        }

        [HttpPost]
        public ActionResult CreateAssetTransfer( AssetTransferViewModel model) 
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
            
            if( Db.SaveChanges() > 0)
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
            else{
                return PartialView("~/Views/Error/Page500.cshtml");
            }
        }

        [HttpPost]
        public ActionResult CheckInAsset(AssetCheckInViewModel model)
        {
            var newCheckIn = new AssetCheckIn
            {
                AssetId = model.AssetID,
                AssignedTo = model.StaffID,
                Note = model.Note,
                CheckInDate = model.CheckInDate
            };
            Db.AssetCheckIns.Add(newCheckIn);
            if( Db.SaveChanges() > 0)
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
                return PartialView("~/Views/Error/Page500.cshtml");
            }
        }

        [HttpPost]
        public ActionResult CheckOutAsset(AssetCheckOutViewModel model)
        {
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
                return PartialView("~/Views/Error/Page500.cshtml");
            }
        }

    }
}
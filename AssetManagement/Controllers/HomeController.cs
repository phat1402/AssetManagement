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
        public ActionResult CreateNewVendor(Vendor vendor)
        {
            try
            {
                Db.Vendors.Add(vendor);
                Db.SaveChanges();
                return Json("Success");
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
    }
}
using AssetManagement.Models;
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
            int numberOfAsset =  Db.Assets.Count();
            var assetUnitPriceList = Db.Assets.Select(a => a.UnitPrice).ToList();
            decimal assetTotalValue = assetUnitPriceList.Sum(value => value).Value;
            var category_count = Db.Assets.Join(Db.SubCategories,
                                                asset => asset.SubCategoryId,
                                                subcategory => subcategory.ID,
                                                (asset, subcategory) => new { asset, subcategory })
                                           .Join(Db.Categories,
                                                as_sub => as_sub.subcategory.CategoryId,
                                                ca => ca.ID,
                                                (as_sub, ca) => new { as_sub.asset.ID, ca.Name })
                                           .GroupBy(a => a.Name)
                                           .Select(item =>
                                                        new { categoryName = item.Key,
                                                              count = item.Count()
                                                            }
                                                   )
                                           .ToList();
            var department_count = Db.Assets.Join(Db.Departments,
                                                  a => a.DepartmentId,
                                                  d => d.ID,
                                                  (a, d) => new { a.ID,d.Name})
                                            .GroupBy(x => x.Name)
                                            .Select(item => new { departmentName = item.Key, count = item.Count()})
                                            .ToList();
            return View();
        }


    }
}
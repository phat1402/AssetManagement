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
            var category_count = Db.Assets.Join(Db.SubCategories,
                                                asset => asset.SubCategoryId,
                                                subcategory => subcategory.ID,
                                                (asset, subcategory) => new { asset, subcategory })
                                           .Join(Db.Categories,
                                                as_sub => as_sub.subcategory.CategoryId,
                                                ca => ca.ID,
                                                (as_sub, ca) => new { as_sub, ca })
                                           .GroupBy(a => a.ca.ID)
                                           .Select(item =>
                                                        new { categoryID = item.Key,
                                                              category_count = item.Count()
                                                            }
                                                   )
                                           .ToList();
            return View();
        }

    }
}
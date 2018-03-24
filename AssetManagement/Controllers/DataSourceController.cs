using AssetManagement.Models;
using AssetManagement.Models.DataSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AssetManagement.Controllers
{
    public class DataSourceController : Controller
    {
        AssetManagementEntities Db = new AssetManagementEntities();
        // GET: DataSource
        public ActionResult GetVendorList(string query)
        {
            List<Select2DataModel> dataList = new List<Select2DataModel>();
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrWhiteSpace(query))
            {
                dataList = Db.Vendors.Where(v => v.Name.StartsWith(query.ToLower()))
                                     .Select(v => new Select2DataModel
                                     {
                                         id = v.ID,
                                         text = v.Name
                                     })
                                     .ToList();
            }
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCategoriesList()
        {
            List<Select2GroupDataModel> dataList = new List<Select2GroupDataModel>();
            List<Category> categoryList = Db.Categories.Select(x => x).ToList();
            List<Select2DataModel> subCategoryList = new List<Select2DataModel>();
            foreach (var category in categoryList){
                subCategoryList = Db.SubCategories.Where(x => x.CategoryId == category.ID)
                                                  .Select(x => new Select2DataModel
                                                  {
                                                      id = x.ID,
                                                      text = x.Name
                                                  }).ToList();

                dataList.Add(new Select2GroupDataModel
                    {
                        text = category.Name,
                        children = subCategoryList
                    });
                }                                                 
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDepartmentList(string query)
        {
            List<Select2DataModel> dataList = new List<Select2DataModel>();
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrWhiteSpace(query))
            {
                dataList = Db.Departments.Where(d => d.Name.StartsWith(query.ToLower()))
                                     .Select(d => new Select2DataModel
                                     {
                                         id = d.ID,
                                         text = d.Name
                                     })
                                     .ToList();
            }
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocationList(string query)
        {
            List<Select2DataModel> dataList = new List<Select2DataModel>();
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrWhiteSpace(query))
            {
                if(!int.TryParse(query, out int a))
                {
                    a = 0;
                }
                dataList = Db.Locations.Where(v => v.Name.Contains(query.ToLower()) || v.RoomNo == a || v.FloorNo == a || v.BuildingName.Contains(query.ToLower()))
                                     .Select(v => new Select2DataModel
                                     {
                                         id = v.ID,
                                         text = v.Name + " -Buidling: " + v.BuildingName + " -Floor No: " + v.FloorNo + " -Room No: "+ v.RoomNo
                                     })
                                     .ToList();
            }
            return Json(new { items = dataList }, JsonRequestBehavior.AllowGet);
        }
    }
}
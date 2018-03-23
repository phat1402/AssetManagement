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

        public ActionResult GetCategoriesList(string query)
        {
            List<Select2DataModel> dataList = new List<Select2DataModel>();
            if (!string.IsNullOrEmpty(query) || !string.IsNullOrWhiteSpace(query))
            {
                dataList = Db.Categories.Where(c => c.Name.StartsWith(query.ToLower()))
                                     .Select(c => new Select2DataModel
                                     {
                                         id = c.ID,
                                         text = c.Name
                                     })
                                     .ToList();
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
    }
}
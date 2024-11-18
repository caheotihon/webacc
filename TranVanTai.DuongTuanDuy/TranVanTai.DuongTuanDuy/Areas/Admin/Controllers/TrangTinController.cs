using SachOnline.Areas.Admin.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranVanTai.DuongTuanDuy.Models;

namespace TranVanTai.DuongTuanDuy.Areas.Admin.Controllers
{
    public class TrangTinController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();

        // GET: Admin/TrangTin
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Home");
            }
            return View(db.TRANGTINs.ToList());
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(TRANGTIN tt)
        {
            if (ModelState.IsValid)
            {
                tt.MetaTitle = tt.TenTrang.RemoveDiacritics().Replace(" ", "-");
                tt.NgayTao = DateTime.Now;
                db.TRANGTINs.Add(tt);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View();
        }
        // GET: Edit
        [HttpGet]
        public ActionResult Edit(int id)
        {
            // Retrieve the specific record from the database based on the ID
            var tt = db.TRANGTINs.SingleOrDefault(t => t.MaTT == id);

            if (tt == null)
            {
                return HttpNotFound();
            }

            return View(tt);
        }

        // POST: Edit
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            if (ModelState.IsValid)
            {
                int id = int.Parse(f["MaTT"]);
                var tt = db.TRANGTINs.Where(t => t.MaTT == id).SingleOrDefault();

                if (tt != null)
                {
                    // Update fields from the form values
                    tt.TenTrang = f["TenTrang"];
                    tt.NoiDung = f["NoiDung"];
                    tt.NgayTao = Convert.ToDateTime(f["NgayTao"]);
                    tt.MetaTitle = f["TenTrang"].RemoveDiacritics().Replace(" ", "-");

                    // Save changes to the database
                    db.SaveChanges();

                    // Redirect to the index page after successful edit
                    return RedirectToAction("Index");
                }
            }

            // If model state is invalid or update fails, redirect to the Edit page
            return RedirectToAction("Edit");
        }
        // GET: Delete
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var tt = db.TRANGTINs.SingleOrDefault(t => t.MaTT == id);

            if (tt == null)
            {
                return HttpNotFound();
            }

            return View(tt);
        }

        // POST: Delete Confirm
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var tt = db.TRANGTINs.SingleOrDefault(t => t.MaTT == id);

            if (tt != null)
            {
                db.TRANGTINs.Remove(tt);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

    }
}
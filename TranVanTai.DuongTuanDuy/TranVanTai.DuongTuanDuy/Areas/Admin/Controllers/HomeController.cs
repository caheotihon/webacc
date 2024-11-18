using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranVanTai.DuongTuanDuy.Models;

namespace TranVanTai.DuongTuanDuy.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        // GET: Admin/Home
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var sTenDN = f["UserName"];
            var sMatKhau = f["Password"];
            ADMIN ad = db.ADMINs.SingleOrDefault(n => n.TenDN == sTenDN && n.MatKhau == sMatKhau);
            if (ad != null)
            {
                Session["Admin"] = ad;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
            }
            return View();
        }
        public ActionResult LoginLogout()
        {
            return PartialView("LoginLogout");
        }
        public ActionResult DangXuat()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using TranVanTai.DuongTuanDuy.Models;

namespace TranVanTai.DuongTuanDuy.Controllers
{
    public class TranVanTaiController : Controller
    {
        
        SachOnlineEntities db = new SachOnlineEntities();

        public ActionResult LoginLogout()
        {
            return PartialView("LoginLogoutPartial");
        }
        public ActionResult Index(int? page)
        {
            var listSachMoi = LaySachMoi(20);

            int iSize = 6;
            int iPageNumber = (page ?? 1);

            return View(listSachMoi.ToPagedList(iPageNumber, iSize));

        }
        private List<SACH> LaySachMoi(int count)
        {
            return db.SACHes.OrderByDescending(a => a.NgayCapNhat).Take(count).ToList();
        }
        private List<SACH> LaySachBanNhieu(int count)
        {
            return db.SACHes.OrderByDescending(a => a.SoLuongBan).Take(count).ToList();
        }
        public ActionResult ChuDePartial()
        {
            var dsChuDe = db.CHUDEs.ToList();
            return PartialView(dsChuDe);
        }
        public ActionResult NhaXuatBanPartial()
        {
            var dsNXB = db.NHAXUATBANs.ToList();
            return PartialView(dsNXB);
        }

        public ActionResult SliderPartial()
        {
            return PartialView();
        }
        public ActionResult SachBanNhieuPartial()
        {
            var listSachBanNhieu = LaySachBanNhieu(6);
            return PartialView(listSachBanNhieu);
        }
        public ActionResult FooterPartial()
        {
            return PartialView();
        }
        public ActionResult ChuDe(int? id, int? page)
        {
            ViewBag.MaCD = id;
            var chuDe = db.CHUDEs.FirstOrDefault(cd => cd.MaCD == id);
            ViewBag.TenChuDe = chuDe.TenChuDe;

            int iSize = 3;
            int iPageNumber = (page ?? 1);
            var kq = (from s in db.SACHes where s.MaCD == id select s).ToList();

            return View(kq.ToPagedList(iPageNumber, iSize));
        }
        public ActionResult NhaXuatBan(int? id, int? page)
        {
            ViewBag.MaNXB = id;
            int iSize = 3;
            int iPageNumber = (page ?? 1);
            var kq = (from s in db.SACHes where s.MaNXB == id select s).ToList();
            return View(kq.ToPagedList(iPageNumber, iSize));
        }

        public ActionResult ChiTietSach(int? id)
        {
            var sach = from s in db.SACHes where s.MaSach == id select s;

            return View(sach.Single());
        }
        [ChildActionOnly]
        public ActionResult NavPartial()
        {
            List<MENU> lst = db.MENUs
                                 .Where(m => m.ParentId == null)
                                 .OrderBy(m => m.OrderNumber)
                                 .ToList();

            int[] a = new int[lst.Count];
            for (int i = 0; i < lst.Count; i++)
            {
                int parentId = lst[i].Id;  // Lưu `Id` vào một biến trước
                List<MENU> l = db.MENUs.Where(m => m.ParentId == parentId).ToList();
                int k = l.Count();
                a[i] = k;
            }

            ViewBag.lst = a;
            return PartialView(lst);
        }
        [ChildActionOnly]
        public ActionResult LoadChildMenu(int parentId)
        {
            List<MENU> lst = new List<MENU> ();
            lst = db.MENUs
                                .Where(m => m.ParentId == parentId)
                                .OrderBy(m => m.OrderNumber)
                                .ToList();

            ViewBag.Count = lst.Count;
            int[] a = new int[lst.Count];

            for(int i=0; i< lst.Count; i++)
            {
                int itemId = lst[i].Id;  // Lưu `Id` của phần tử vào biến cục bộ
                List<MENU> l = db.MENUs.Where(m => m.ParentId == itemId).ToList();
                int k = l.Count();
                a[i] = k;
            }

            ViewBag.lst = a;
            return PartialView("LoadChildMenu", lst);

        }

        public ActionResult TrangTin(string metatitle)
        {
            var tt = (from t in db.TRANGTINs where t.MetaTitle == metatitle select t).Single();
            return View(tt);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using System.IO;

using TranVanTai.DuongTuanDuy.Models;
using System.Data.Entity;

namespace TranVanTai.DuongTuanDuy.Areas.Admin.Controllers
{
    public class SachController : Controller
    {
        // GET: Admin/Sach
        SachOnlineEntities db = new SachOnlineEntities();
        public ActionResult Index(int? page, string strSearch = null)
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("Login", "Home");
            }
            ViewBag.Search = strSearch;

            var books = db.SACHes.AsQueryable();
            if (!string.IsNullOrEmpty(strSearch))
            {
                books = books.Where(s => s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch));
            }

            int iPageNum = (page ?? 1);
            int iPageSize = 7;
            return View(books.OrderBy(n => n.MaSach).ToPagedList(iPageNum, iPageSize));
        }

        public ActionResult Create(int? page)
        {
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(SACH sach, FormCollection f, HttpPostedFileBase FileUpload)
        {
            // Đưa dữ liệu vào DropDown
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB");

            // Kiểm tra xem FileUpload có rỗng không
            if (FileUpload == null || FileUpload.ContentLength == 0)
            {
                ViewBag.ThongBao = "Hãy chọn ảnh bìa!";
                ViewBag.TenSach = f["sTenSach"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
                ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", int.Parse(f["MaCD"]));
                ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", int.Parse(f["MaNXB"]));
                return PartialView("Create",sach);
            }

            if (!ModelState.IsValid)
            {
                // Kiểm tra lỗi ModelState
                return View("Index");
            }

            // Khai báo thư viện: System.IO để xử lý file
            var sFileName = Path.GetFileName(FileUpload.FileName);
            var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

            // Kiểm tra ảnh bìa đã tồn tại chưa để lưu lên thư mục
            if (!System.IO.File.Exists(path))
            {
                FileUpload.SaveAs(path);
            }

            // Lưu thông tin sách vào CSDL
            sach.TenSach = f["sTenSach"];
            sach.MoTa = f["sMoTa"];
            sach.SoLuongBan = int.Parse(f["iSoLuong"]);
            sach.GiaBan = decimal.Parse(f["mGiaBan"]);
            sach.AnhBia = sFileName;
            sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
            sach.MaCD = int.Parse(f["MaCD"]);
            sach.MaNXB = int.Parse(f["MaNXB"]);

            db.SACHes.Add(sach);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);

            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            return PartialView(sach);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            int maSach = int.Parse(f["iMaSach"]);

            // Lấy thông tin sách cần chỉnh sửa từ cơ sở dữ liệu
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == maSach);

            if (sach == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy sách
            }

            // Thiết lập ViewBag cho dropdown list
            ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.MaCD);
            ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNXB), "MaNXB", "TenNXB", sach.MaNXB);

            if (ModelState.IsValid)
            {
                // Xử lý tải lên hình ảnh
                if (fFileUpload != null && fFileUpload.ContentLength > 0)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

                    // Nếu tệp chưa tồn tại thì lưu vào thư mục
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                        sach.AnhBia = sFileName; // Cập nhật ảnh bìa
                    }

                }

                // Cập nhật các thông tin sách
                sach.TenSach = f["sTenSach"];
                sach.MoTa = f["sMoTa"];
                sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
                sach.SoLuongBan = int.Parse(f["iSoLuong"]);
                sach.GiaBan = decimal.Parse(f["mGiaBan"]);
                sach.MaCD = int.Parse(f["MaCD"]);
                sach.MaNXB = int.Parse(f["MaNXB"]);

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();

                return RedirectToAction("Index"); // Chuyển hướng về danh sách sách
            }

            // Nếu ModelState không hợp lệ, trả về view với thông tin sách
            return PartialView(sach);
        }


        public ActionResult Details(int id)
        {
            var sach = db.SACHes.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return PartialView(sach); // Trả về partial view "_Details" cho modal
        }

        public ActionResult Delete(int id)
        {
            var sach = db.SACHes.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return PartialView( sach); // Trả về partial view "_Delete" cho modal
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            // Lấy thông tin sách cần xóa
            var sach = db.SACHes.SingleOrDefault(n => n.MaSach == id);

            // Nếu không tìm thấy sách, trả về mã lỗi 404
            if (sach == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy sách
            }

            // Kiểm tra xem sách có trong bảng Chi tiết đặt hàng hay không
            var ctdh = db.CHITIETDATHANGs.Where(ct => ct.MaSach == id);
            if (ctdh.Any())
            {
                // Hiển thị thông báo nếu sách còn tồn tại trong bảng Chi tiết đặt hàng
                ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" +
                                   "* Nếu muốn xóa thì phải xóa hết mã sách này trong bảng Chi tiết đặt hàng.";
                return PartialView(sach); // Trả về view với thông báo
            }

            // Xóa tất cả các bản ghi liên quan đến sách từ bảng VIETSACH
            var vietsach = db.VIETSACHes.Where(vs => vs.MaSach == id).ToList();
            if (vietsach.Count > 0)
            {
                db.VIETSACHes.RemoveRange(vietsach);
            }

            // Xóa sách
            db.SACHes.Remove(sach);
            db.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu

            // Chuyển hướng về trang danh sách sách
            return RedirectToAction("Index");
        }

    }
}

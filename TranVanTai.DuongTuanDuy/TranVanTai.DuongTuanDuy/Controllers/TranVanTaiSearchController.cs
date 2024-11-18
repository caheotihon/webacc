using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TranVanTai.DuongTuanDuy.Models;
using System.Linq.Dynamic;
using System.Linq.Expressions;

namespace TranVanTai.DuongTuanDuy.Areas.Admin.Controllers
{
    public class TranVanTaiSearchController : Controller
    {
        SachOnlineEntities db = new SachOnlineEntities();
        // GET: Admin/Search
        public ActionResult Search(string strSearch)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                var kq = from s in db.SACHes
                         join cd in db.CHUDEs on s.MaCD equals cd.MaCD
                         join nxb in db.NHAXUATBANs on s.MaNXB equals nxb.MaNXB
                         where s.TenSach.Contains(strSearch)
                            || s.MoTa.Contains(strSearch)
                            || cd.TenChuDe.Contains(strSearch)
                            || nxb.TenNXB.Contains(strSearch)
                         //orderby s.NgayCapNhat descending, s.SoLuongBan asrcending
                         select s;
                return View(kq);
            }
            return View();
        }
        public ActionResult SearchTheoDanhMuc(string strSearch = null, int maCD = 0)
        {
            // 1. Lưu từ khóa tìm kiếm ViewBag.Search = strSearch;

            //2.Tạo câu truy cơ bản
            var kq = db.SACHes.Select(b => b);
            //3. Tìm kiếm theo searchString
            if (!String.IsNullOrEmpty(strSearch))
                kq = kq.Where(b => b.TenSach.Contains(strSearch));

            //4. Tìm kiếm theo MaCD if (maCD != 0)
            {
                kq = kq.Where(b => b.CHUDE.MaCD == maCD);
            }
            //5. Tạo danh sách danh mục để hiển thị ở giao diện View thông qua DropDownList


            ViewBag.MaCD = new SelectList(db.CHUDEs, "MaCD", "TenChuDe"); // danh sách Chủ đề
            //ViewBag.cd = db.CHUDEs.ToList();
            return View(kq.ToList());
        }

        public ActionResult Group()
        {
            //var kq = from s in db.SACHes group s by s.MaCD;
            var kq = db.SACHes.GroupBy(s => s.MaCD);

            return View(kq);
        }
        public ActionResult ThongKe()
        {
            var rawResult = from s in db.SACHes
                            join cd in db.CHUDEs on s.MaCD equals cd.MaCD
                            group s by new { cd.MaCD, cd.TenChuDe } into g
                            select new
                            {
                                MaCD = g.Key.MaCD,
                                TenChuDe = g.Key.TenChuDe,
                                Count = g.Count(),
                                Sum = g.Sum(n => n.SoLuongBan),
                                Max = g.Max(n => n.SoLuongBan),
                                Avg = g.Average(n => n.SoLuongBan)
                            };

            // Now do the conversion in-memory
            var kq = rawResult.ToList().Select(g => new ReportInfo
            {
                Id = g.MaCD.ToString(),
                Name = g.TenChuDe,
                Count = g.Count,
                Sum = g.Sum,
                Max = g.Max,
                Avg = Convert.ToDecimal(g.Avg)
            });

            return View(kq);
        }
        public ActionResult SearchPhanTrang(int? page, string strSearch = null)
        {
            ViewBag.Search = strSearch;
            if (!string.IsNullOrEmpty(strSearch))
            {
                int iSize = 3;
                int iPageNumber = (page ?? 1);
                var kq = (from s in db.SACHes
                          where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch)
                          select s).ToList();
                return View(kq.ToPagedList(iPageNumber, iSize));
            }
            return View();
        }
        public ActionResult SearchPhanTrangTuyChon(int? size, int? page, string strSearch)
        {
            // 1. List để lấy nguồn cho ComboBox chọn số lượng sản phẩm
            var items = new List<SelectListItem>()
            {
                new SelectListItem { Text = "3", Value = "3" },
                new SelectListItem { Text = "5", Value = "5" },
                new SelectListItem { Text = "10", Value = "10" }
            };

            // 1.1 Giữ trạng thái kích thước trang được chọn trên DropDownList
            foreach (var item in items)
            {
                if (item.Value == size.ToString())
                {
                    item.Selected = true;
                }
            }

            // 1.2. Tạo các biến ViewBag
            ViewBag.Size = items; // ViewBag DropDownList
            ViewBag.CurrentSize = size; // Tạo biến kích thước trang hiện tại
            ViewBag.Search = strSearch;

            // 1.3. Mặc định 3 item trên 1 trang nếu không có giá trị
            int iSize = (size ?? 3);
            int iPageNumber = (page ?? 1);

            // 1.4. Kiểm tra nếu có từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(strSearch))
            {
                var kq = (from s in db.SACHes
                          where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch)
                          select s).ToList();

                return View(kq.ToPagedList(iPageNumber, iSize));
            }

            // Nếu không có từ khóa tìm kiếm, trả về toàn bộ sách
            var allBooks = db.SACHes.ToList();
            return View(allBooks.ToPagedList(iPageNumber, iSize));
        }
        public ActionResult SearchPhanTrangSapXep(int? page, string sortProperty, string sortOrder = "*", string strSearch = null)
        {
            ViewBag.Search = strSearch;

            if (string.IsNullOrEmpty(strSearch))
            {
                strSearch = null;
            }

            int iSize = 3;
            int iPageNumber = (page ?? 1);

            // 1. Gán giá trị cho biến sortOrder
            if (sortOrder == "*") ViewBag.SortOrder = "desc";
            if (sortOrder == "desc") ViewBag.SortOrder = "*";
            if (sortOrder == "*") ViewBag.SortOrder = "asc";

            // Tạo thuộc tính sắp xếp mặc định là "Tên Sách"
            if (String.IsNullOrEmpty(sortProperty))
            {
                sortProperty = "TenSach";
            }

            // Gán giá trị cho biến sortProperty
            ViewBag.SortProperty = sortProperty;

            // Truy vấn
            var kq = from s in db.SACHes
                     where s.TenSach.Contains(strSearch) || s.MoTa.Contains(strSearch)
                     select s;

            // Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc")
            {
                kq = kq.OrderBy(sortProperty + " " + sortOrder);
            }
            else
            {
                kq = kq.OrderBy(sortProperty);
            }

            return View(kq.ToPagedList(iPageNumber, iSize));
        }

        

    }
}
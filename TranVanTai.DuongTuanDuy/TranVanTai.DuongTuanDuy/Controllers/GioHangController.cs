using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TranVanTai.DuongTuanDuy.Models;
using System.Text;

namespace TranVanTai.DuongTuanDuy.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        SachOnlineEntities db = new SachOnlineEntities();

        public List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }
        public ActionResult ThemGioHang(int id, string url)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.Find(n => n.iMaSach == id);
            if (sp == null)
            {
                sp = new GioHang(id);
                lstGioHang.Add(sp);
            }
            else
            {
                sp.iSoLuong++;
            }
            return Redirect(url);

        }
        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.iSoLuong);
            }
            return iTongSoLuong;
        }
        private double TongTien()
        {
            double iTongTien = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongTien = lstGioHang.Sum(n => n.dThanhTien);
            }
            return iTongTien;
        }
        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            if (lstGioHang.Count == 0)
            {
                return RedirectToAction("Index", "TranVanTai");
            }
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }
        public ActionResult XoaSPGioHang(int iMaSach)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.iMaSach == iMaSach);
                if (lstGioHang.Count == 0)
                {
                    return RedirectToAction("Index", "TranVanTai");
                }
            }
            return RedirectToAction("GioHang");
        }

        public ActionResult CapNhatGioHang(int iMaSach, FormCollection f)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.iMaSach == iMaSach);
            if (sp != null)
            {
                sp.iSoLuong = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("GioHang");

        }
        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "TranVanTai");

        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return RedirectToAction("DangNhap", "User");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "TranVanTai");
            }

            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }
        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            DONDATHANG ddh = new DONDATHANG();
            KHACHHANG kh = (KHACHHANG)Session["TaiKhoan"];
            List<GioHang> lstgioHangs = LayGioHang();
            if (string.IsNullOrEmpty(f["NgayGiao"]))
            {
                ModelState.AddModelError("NgayGiao", "Bạn phải nhập ngày giao.");
            }
            else
            {
                DateTime ngayDat = DateTime.Now;
                DateTime ngayGiao;

                // Kiểm tra nếu ngày giao có thể chuyển đổi thành DateTime
                if (!DateTime.TryParse(f["NgayGiao"], out ngayGiao))
                {
                    ModelState.AddModelError("NgayGiao", "Ngày giao không hợp lệ.");
                }
                else if (ngayGiao <= ngayDat)
                {
                    ModelState.AddModelError("NgayGiao", "Ngày giao phải sau ngày đặt.");
                }
            }

            // Nếu có lỗi, trả về view với thông báo lỗi
            if (!ModelState.IsValid)
            {
                ViewBag.TongSoLuong = TongSoLuong();
                ViewBag.TongTien = TongTien();
                return View(lstgioHangs);
            }

            ddh.MaKH = kh.MaKH;
            ddh.NgayDat = DateTime.Now;
            var NgayGiao = String.Format("{0:MM/DD/yyyy}", f["NgayGiao"]);
            ddh.NgayGiao = DateTime.Parse(NgayGiao);
            ddh.TinhTrangGiaoHang = 1;
            ddh.DaThanhToan = false;
            db.DONDATHANGs.Add(ddh);
            db.SaveChanges();

            foreach(var item in lstgioHangs)
            {
                CHITIETDATHANG ctdh = new CHITIETDATHANG();
                ctdh.MaDonHang = ddh.MaDonHang;
                ctdh.MaSach = item.iMaSach;
                ctdh.SoLuong = item.iSoLuong;
                ctdh.DonGia = (decimal)item.dDonGia;
                db.CHITIETDATHANGs.Add(ctdh);
                db.SaveChanges();
                
            }
            decimal tongTien = (decimal)TongTien();
            GuiMailXacNhan(kh.Email, kh.HoTen, ddh, tongTien);
            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");
        }
        private void GuiMailXacNhan(string toEmail, string hoTen, DONDATHANG donHang, decimal tongTien)
        {
            List<GioHang> lstGioHang = LayGioHang();
            var mail = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("rainy.vt18@gmail.com", "ggyz jppg chri ihtr"),
                EnableSsl = true
            };
            var message = new MailMessage
            {
                From = new MailAddress("rainy.vt18@gmail.com.com"),
                Subject = "Xác nhận đơn hàng - " + donHang.MaDonHang,
                IsBodyHtml = true 
            };

            message.To.Add(new MailAddress(toEmail));

            // Tạo nội dung email
            var body = new StringBuilder();
            body.AppendLine("<h3>Cảm ơn " + hoTen + " đã đặt hàng tại cửa hàng ABC của chúng tôi!</h3>");
            body.AppendLine("<p>Đơn hàng của bạn đã được xác nhận.</p>");
            body.AppendLine("<p><b>Thông tin đơn hàng:</b></p>");
            body.AppendLine("<ul>");
            body.AppendLine("<li>Mã đơn hàng: " + donHang.MaDonHang + "</li>");
            body.AppendLine("<li>Ngày đặt hàng: " + donHang.NgayDat + "</li>");
            // Thêm các thông tin khác

            // Hiển thị chi tiết các sản phẩm trong đơn hàng
            body.AppendLine("<li><b>Chi tiết sản phẩm:</b></li>");
            body.AppendLine("<table border='1'>");
            body.AppendLine("<tr><th>Tên Sách</th><th>Số Lượng</th><th>Đơn Giá</th><th>Thành Tiền</th></tr>");
            foreach (var item in lstGioHang)
            {
                body.AppendLine("<tr>");
                body.AppendLine("<td>" + item.sTenSach + "</td>");
                body.AppendLine("<td>" + item.iSoLuong + "</td>"); 
                body.AppendLine("<td>" + item.dDonGia + " VNĐ</td>"); 
                body.AppendLine("<td>" + (item.iSoLuong * item.dDonGia) + " VNĐ</td>"); 
                body.AppendLine("</tr>");
            }
            body.AppendLine("</table>");

            body.AppendLine("<li><b>Tổng tiền:</b> " + tongTien + " VNĐ</li>");
            body.AppendLine("</ul>");
            body.AppendLine("<p>Trân trọng,</p>");
            body.AppendLine("<p>Cửa hàng ABC</p>");

            message.Body = body.ToString();

            // Gửi email
            mail.Send(message);
        }
        public ActionResult XacNhanDonHang()
        {
            return View();
        }

    }
}
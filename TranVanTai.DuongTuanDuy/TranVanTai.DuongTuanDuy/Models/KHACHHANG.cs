﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TranVanTai.DuongTuanDuy.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;

    public partial class KHACHHANG
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KHACHHANG()
        {
            this.DONDATHANGs = new HashSet<DONDATHANG>();
        }
        [Key]
        [Required]
        public int MaKH { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [DisplayName("Họ và tên")]
        public string HoTen { get; set; }

        [MinLength(5, ErrorMessage = "Tên đăng nhập phải ít nhất 5 ký tự.")]
        [Required(ErrorMessage = "Tên đăng nhập không được để trống.")]
        public string TaiKhoan { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 tới 15 ký tự.")]
        public string MatKhau { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        public string Email { get; set; }

        public string DiaChi { get; set; }
        [Required(ErrorMessage = "Điện thoại không được để trống.")]
        public string DienThoai { get; set; }

        [Required(ErrorMessage = "Ngày sinh không được để trống ")]
        public Nullable<System.DateTime> NgaySinh { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DONDATHANG> DONDATHANGs { get; set; }
    }
}

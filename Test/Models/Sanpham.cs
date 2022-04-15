using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Test.Models
{
    public partial class Sanpham
    {
        public Sanpham()
        {
            Cthd = new HashSet<Cthd>();
        }
        [Display(Name = "Mã sản phẩm")]
        public string Masp { get; set; }
        [Display(Name = "Màn hình")]
        public string Manhinh { get; set; }
        [Display(Name = "Bộ xử lý")]
        public string Boxuly { get; set; }
        [Display(Name = "RAM")]
        public string Ram { get; set; }
        [Display(Name = "Cổng kết nối")]
        public string Congketnoi { get; set; }
        [Display(Name = "Danh mục")]
        public string Danhmuc { get; set; }
        [Display(Name = "Tên sản phẩm")]
        public string Tensp { get; set; }
        [Display(Name = "Số lượng")]
        public int? Soluong { get; set; }
        [Display(Name = "Màu sắc")]
        public string Mausac { get; set; }
        [Display(Name = "Ổ cứng")]
        public string Ocung { get; set; }
        [Display(Name = "Card màn hình")]
        public string Cardmanhinh { get; set; }
        [Display(Name = "Đặc biệt")]
        public string Dacbiet { get; set; }
        [Display(Name = "Hệ điều hành")]
        public string Hdh { get; set; }
        [Display(Name = "Thiết kế")]
        public string Thietke { get; set; }
        [Display(Name = "Kích thước Trọng lượng")]
        public string KichthuocTrongluong { get; set; }
        [Display(Name = "Webcam")]
        public string Webcam { get; set; }
        [Display(Name = "Pin")]
        public string Pin { get; set; }
        [Display(Name = "Ra mắt")]
        public int? Ramat { get; set; }
        [Display(Name = "Mô tả")]
        public string Mota { get; set; }
        [Display(Name = "Đơn giá")]
        public long? Dongia { get; set; }
        [Display(Name = "Hình ảnh")]
        public string Hinhanh { get; set; }

        public virtual Boxuly BoxulyNavigation { get; set; }
        public virtual Congketnoi CongketnoiNavigation { get; set; }
        public virtual Danhmucsanpham DanhmucNavigation { get; set; }
        public virtual Manhinh ManhinhNavigation { get; set; }
        public virtual Bonhoram RamNavigation { get; set; }
        public virtual ICollection<Cthd> Cthd { get; set; }
        [NotMapped]
        public IFormFile HinhAnhFile { get; set; }
    }
}

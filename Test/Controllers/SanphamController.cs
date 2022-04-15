using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Test.Data;
using Test.Models;

namespace Test.Controllers
{
    public class SanphamController : Controller
    {
        private readonly LapTopContext _context;

        private readonly IConfiguration _configuration;

        private readonly ILogger<SanphamController> _logger;

        private readonly UserManager<AppUser> _userManager;

        private readonly IEmailSender _sendMail;

        // Key lưu chuỗi json của Cart
        public const string CARTKEY = "cart";

        //// Lấy cart từ Session (danh sách giỏ hàng)
        List<GioHang> GetCartItems()
        {

            var session = HttpContext.Session;
            string jsoncart = session.GetString(CARTKEY);
            if (jsoncart != null)
            {
                return JsonConvert.DeserializeObject<List<GioHang>>(jsoncart);
            }
            return new List<GioHang>();
        }

        //// Xóa giỏ khỏi session
        void ClearCart()
        {
            var session = HttpContext.Session;
            session.Remove(CARTKEY);
        }

        //// Lưu Cart (Danh sách giỏ hàng) vào session
        void SaveCartSession(List<GioHang> ls)
        {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(CARTKEY, jsoncart);
        }
        public SanphamController(LapTopContext context, IConfiguration config, UserManager<AppUser> userManager, IEmailSender sendMail)
        {
            _context = context;
            _configuration = config;
            _userManager = userManager;
            _sendMail = sendMail;
        }

        private async Task<AppUser> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }


        // GET: Sanpham
        public async Task<IActionResult> Index()
        {
            var lapTopContext = _context.Sanpham.Include(s => s.BoxulyNavigation).Include(s => s.CongketnoiNavigation).Include(s => s.DanhmucNavigation).Include(s => s.ManhinhNavigation).Include(s => s.RamNavigation);
            return View(await lapTopContext.ToListAsync());
        }

        // GET: Sanpham/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanpham
                .Include(s => s.BoxulyNavigation)
                .Include(s => s.CongketnoiNavigation)
                .Include(s => s.DanhmucNavigation)
                .Include(s => s.ManhinhNavigation)
                .Include(s => s.RamNavigation)
                .FirstOrDefaultAsync(m => m.Masp == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // GET: Sanpham/Create
        public IActionResult Create()
        {
            ViewData["Boxuly"] = new SelectList(_context.Boxuly, "Mabxl", "Mabxl");
            ViewData["Congketnoi"] = new SelectList(_context.Congketnoi, "Mackn", "Mackn");
            ViewData["Danhmuc"] = new SelectList(_context.Danhmucsanpham, "Madm", "Madm");
            ViewData["Manhinh"] = new SelectList(_context.Manhinh, "Mamh", "Mamh");
            ViewData["Ram"] = new SelectList(_context.Bonhoram, "Maram", "Maram");
            return View();
        }

        // POST: Sanpham/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Masp,Manhinh,Boxuly,Ram,Congketnoi,Danhmuc,Tensp,Soluong,Mausac,Ocung,Cardmanhinh,Dacbiet,Hdh,Thietke,KichthuocTrongluong,Webcam,Pin,Ramat,Mota,Dongia,Hinhanh")] Sanpham sanpham)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sanpham);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Boxuly"] = new SelectList(_context.Boxuly, "Mabxl", "Mabxl", sanpham.Boxuly);
            ViewData["Congketnoi"] = new SelectList(_context.Congketnoi, "Mackn", "Mackn", sanpham.Congketnoi);
            ViewData["Danhmuc"] = new SelectList(_context.Danhmucsanpham, "Madm", "Madm", sanpham.Danhmuc);
            ViewData["Manhinh"] = new SelectList(_context.Manhinh, "Mamh", "Mamh", sanpham.Manhinh);
            ViewData["Ram"] = new SelectList(_context.Bonhoram, "Maram", "Maram", sanpham.Ram);
            return View(sanpham);
        }

        // GET: Sanpham/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanpham.FindAsync(id);
            if (sanpham == null)
            {
                return NotFound();
            }
            ViewData["Boxuly"] = new SelectList(_context.Boxuly, "Mabxl", "Mabxl", sanpham.Boxuly);
            ViewData["Congketnoi"] = new SelectList(_context.Congketnoi, "Mackn", "Mackn", sanpham.Congketnoi);
            ViewData["Danhmuc"] = new SelectList(_context.Danhmucsanpham, "Madm", "Madm", sanpham.Danhmuc);
            ViewData["Manhinh"] = new SelectList(_context.Manhinh, "Mamh", "Mamh", sanpham.Manhinh);
            ViewData["Ram"] = new SelectList(_context.Bonhoram, "Maram", "Maram", sanpham.Ram);
            return View(sanpham);
        }

        // POST: Sanpham/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Masp,Manhinh,Boxuly,Ram,Congketnoi,Danhmuc,Tensp,Soluong,Mausac,Ocung,Cardmanhinh,Dacbiet,Hdh,Thietke,KichthuocTrongluong,Webcam,Pin,Ramat,Mota,Dongia,Hinhanh")] Sanpham sanpham)
        {
            if (id != sanpham.Masp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sanpham);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SanphamExists(sanpham.Masp))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Boxuly"] = new SelectList(_context.Boxuly, "Mabxl", "Mabxl", sanpham.Boxuly);
            ViewData["Congketnoi"] = new SelectList(_context.Congketnoi, "Mackn", "Mackn", sanpham.Congketnoi);
            ViewData["Danhmuc"] = new SelectList(_context.Danhmucsanpham, "Madm", "Madm", sanpham.Danhmuc);
            ViewData["Manhinh"] = new SelectList(_context.Manhinh, "Mamh", "Mamh", sanpham.Manhinh);
            ViewData["Ram"] = new SelectList(_context.Bonhoram, "Maram", "Maram", sanpham.Ram);
            return View(sanpham);
        }

        // GET: Sanpham/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = await _context.Sanpham
                .Include(s => s.BoxulyNavigation)
                .Include(s => s.CongketnoiNavigation)
                .Include(s => s.DanhmucNavigation)
                .Include(s => s.ManhinhNavigation)
                .Include(s => s.RamNavigation)
                .FirstOrDefaultAsync(m => m.Masp == id);
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        // POST: Sanpham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var sanpham = await _context.Sanpham.FindAsync(id);
            _context.Sanpham.Remove(sanpham);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SanphamExists(string id)
        {
            return _context.Sanpham.Any(e => e.Masp == id);
        }

        private List<Sanpham> LayDSSPTheoTrang(int page)
        {
            List<Sanpham> list = new List<Sanpham>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string query = "select * from SanPham LIMIT @page,12";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@page", (page - 1) * 12);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Sanpham()
                    {
                        Masp = reader["MaSP"].ToString(),
                        Manhinh = reader["ManHinh"].ToString(),
                        Boxuly = reader["BoXuLy"].ToString(),
                        Ram = reader["RAM"].ToString(),
                        Congketnoi = reader["CongKetNoi"].ToString(),
                        Danhmuc = reader["DanhMuc"].ToString(),
                        Tensp = reader["TenSP"].ToString(),
                        Soluong = Convert.ToInt32(reader["SoLuong"]),
                        Mausac = reader["MauSac"].ToString(),
                        Ocung = reader["OCung"].ToString(),
                        Cardmanhinh = reader["CardManHinh"].ToString(),
                        Dacbiet = reader["DacBiet"]?.ToString(),
                        Hdh = reader["HDH"].ToString(),
                        Thietke = reader["ThietKe"].ToString(),
                        KichthuocTrongluong = reader["KichThuoc_TrongLuong"].ToString(),
                        Webcam = reader["Webcam"]?.ToString(),
                        Pin = reader["Pin"].ToString(),
                        Ramat = Convert.ToInt32(reader["RaMat"]),
                        Mota = reader["MoTa"].ToString(),
                        Dongia = Convert.ToInt64(reader["DonGia"]),
                        Hinhanh = reader["HinhAnh"].ToString(),
                    });

                }
            }

            return list;

        }

        private List<Sanpham> LayDSSPTheoDanhMuc(string madm)
        {
            List<Sanpham> list = new List<Sanpham>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string query = "select * from SanPham where DANHMUC = @madm";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@madm", madm);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Sanpham()
                    {
                        Masp = reader["MaSP"].ToString(),
                        Manhinh = reader["ManHinh"].ToString(),
                        Boxuly = reader["BoXuLy"].ToString(),
                        Ram = reader["RAM"].ToString(),
                        Congketnoi = reader["CongKetNoi"].ToString(),
                        Danhmuc = reader["DanhMuc"].ToString(),
                        Tensp = reader["TenSP"].ToString(),
                        Soluong = Convert.ToInt32(reader["SoLuong"]),
                        Mausac = reader["MauSac"].ToString(),
                        Ocung = reader["OCung"].ToString(),
                        Cardmanhinh = reader["CardManHinh"].ToString(),
                        Dacbiet = reader["DacBiet"]?.ToString(),
                        Hdh = reader["HDH"].ToString(),
                        Thietke = reader["ThietKe"].ToString(),
                        KichthuocTrongluong = reader["KichThuoc_TrongLuong"].ToString(),
                        Webcam = reader["Webcam"]?.ToString(),
                        Pin = reader["Pin"].ToString(),
                        Ramat = Convert.ToInt32(reader["RaMat"]),
                        Mota = reader["MoTa"].ToString(),
                        Dongia = Convert.ToInt64(reader["DonGia"]),
                        Hinhanh = reader["HinhAnh"].ToString(),
                    });

                }
            }

            return list;

        }

        private List<Sanpham> LayDSSPTheoDanhMucTheoTrang(string madm, int page)
        {
            List<Sanpham> list = new List<Sanpham>();
            string connStr = _configuration.GetConnectionString("DefaultConnection");
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();
            string query = "select * from SanPham where DANHMUC = @madm LIMIT @page,6 ;";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@madm", madm);
            cmd.Parameters.AddWithValue("@page", page * 6);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new Sanpham()
                    {
                        Masp = reader["MaSP"].ToString(),
                        Manhinh = reader["ManHinh"].ToString(),
                        Boxuly = reader["BoXuLy"].ToString(),
                        Ram = reader["RAM"].ToString(),
                        Congketnoi = reader["CongKetNoi"].ToString(),
                        Danhmuc = reader["DanhMuc"].ToString(),
                        Tensp = reader["TenSP"].ToString(),
                        Soluong = Convert.ToInt32(reader["SoLuong"]),
                        Mausac = reader["MauSac"].ToString(),
                        Ocung = reader["OCung"].ToString(),
                        Cardmanhinh = reader["CardManHinh"].ToString(),
                        Dacbiet = reader["DacBiet"]?.ToString(),
                        Hdh = reader["HDH"].ToString(),
                        Thietke = reader["ThietKe"].ToString(),
                        KichthuocTrongluong = reader["KichThuoc_TrongLuong"].ToString(),
                        Webcam = reader["Webcam"]?.ToString(),
                        Pin = reader["Pin"].ToString(),
                        Ramat = Convert.ToInt32(reader["RaMat"]),
                        Mota = reader["MoTa"].ToString(),
                        Dongia = Convert.ToInt64(reader["DonGia"]),
                        Hinhanh = reader["HinhAnh"].ToString(),
                    });

                }
            }

            return list;

        }
        
        // GET: Sanpham
        public IActionResult Shop(string madm, int page, string tensp)
        {
            List<Sanpham> spList = new List<Sanpham>();
            var lapTopContext = _context.Sanpham.Include(s => s.BoxulyNavigation).Include(s => s.CongketnoiNavigation).Include(s => s.DanhmucNavigation).Include(s => s.ManhinhNavigation).Include(s => s.RamNavigation);
            double totalPage;
            if (string.IsNullOrEmpty(tensp)) 
            { 
                if (string.IsNullOrEmpty(madm))
                {
                    spList = lapTopContext.ToList();
                    float temp = spList.Count() / (float)12;
                    totalPage = Math.Ceiling(temp);
                    if (page == 0)
                    {
                        spList = spList.Take(12).ToList();
                    }
                    else
                    {
                        spList = LayDSSPTheoTrang(page);
                    }
                }
                else
                {
                    spList = LayDSSPTheoDanhMuc(madm);
                    float temp = spList.Count() / (float)12;
                    totalPage = Math.Ceiling(temp);
                    if (page == 0)
                    {
                        spList = spList.Take(12).ToList();
                    }
                    else
                    {
                        spList = LayDSSPTheoDanhMucTheoTrang(madm, page);
                    }
                }

                ViewBag.dmSP = _context.Danhmucsanpham.ToList();
                ViewBag.totalPage = totalPage;
                ViewBag.pageCurrent = page;
            }
            else
            {
                spList = _context.Sanpham.Where(sp => sp.Tensp.ToLower().Contains(tensp.ToLower())).Select(sp=> new Sanpham {
                    Masp = sp.Masp,
                    Manhinh = sp.Manhinh,
                    Boxuly = sp.Boxuly,
                    Ram = sp.Ram,
                    Congketnoi = sp.Congketnoi,
                    Danhmuc = sp.Danhmuc,
                    Tensp = sp.Tensp,
                    Soluong = sp.Soluong,
                    Mausac = sp.Mausac,
                    Ocung =sp.Ocung,
                    Cardmanhinh = sp.Cardmanhinh,
                    Dacbiet = sp.Dacbiet,
                    Hdh = sp.Hdh,
                    Thietke = sp.Thietke,
                    KichthuocTrongluong = sp.KichthuocTrongluong,
                    Webcam = sp.Webcam,
                    Pin = sp.Pin,
                    Ramat = sp.Ramat,
                    Mota = sp.Mota,
                    Dongia = sp.Dongia,
                    Hinhanh = sp.Hinhanh,
                }).ToList();
                ViewBag.dmSP = _context.Danhmucsanpham.ToList();
            }
            return View(spList);
        }

        // GET: Sanpham/Details/5
        public async Task<IActionResult> ProductSingle(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sanpham = _context.Sanpham.Find(id);

            ViewData["BXL"] = _context.Boxuly.Find(sanpham.Boxuly);
            ViewData["RAM"] = _context.Bonhoram.Find(sanpham.Ram);
            ViewData["MH"] = _context.Manhinh.Find(sanpham.Manhinh);
            ViewData["CKN"] = _context.Congketnoi.Find(sanpham.Congketnoi);
            ViewData["SanPham"] = sanpham;
            if (sanpham == null)
            {
                return NotFound();
            }

            return View(sanpham);
        }

        //Hiển thị card
        public IActionResult Cart()
        {
            return View(GetCartItems());
        }

      // Thêm sản phẩm vô cart
        public IActionResult AddToCart([FromRoute] string id)
        {
           
            var sanpham = _context.Sanpham
                .Where(p => p.Masp == id)
                .FirstOrDefault();

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Sanpham.Masp == id);


            if (sanpham == null)
                return NotFound("Không có sản phẩm");

            // Xử lý đưa vào Cart ...
            if (cartitem != null)
            {
                if (cartitem.SL + 1 > sanpham.Soluong)
                {
                    cartitem.SL = sanpham.Soluong;
                    Console.WriteLine("cart bằng số lượng");
                }
                else
                {
                    cartitem.SL++;
                    Console.WriteLine("ok 1 ");
                }
            }
            else
            {            
                cart.Add(new GioHang() { SL = 1, Sanpham = sanpham });              
                Console.WriteLine("ok 2");
            }

            // Lưu cart vào Session
            SaveCartSession(cart);
            ViewData["Cart"] = "Thêm thành công!";
            // Chuyển đến trang hiện thị Cart
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult AddToCartQuantity([FromForm] string id, [FromForm] int quantity )
        {
            Console.WriteLine("{0} is", id);
            Console.WriteLine("{0} q", quantity);
            var sanpham = _context.Sanpham
                .Where(p => p.Masp == id)
                .FirstOrDefault();

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Sanpham.Masp == id);

            if (sanpham == null)
                return NotFound("Không có sản phẩm");
            if (sanpham.Soluong == 0)
            {
                return Redirect("/SanPham/ProducSingle");
            }
            else
            {
                //Kiểm tra số lượng thỏa mãn hay không
                if (quantity > sanpham.Soluong)
                {
                    cartitem.SL = sanpham.Soluong;
                    Console.WriteLine("quantity lớn hơn soluongsp");
                }
                else
                {
                    // Xử lý đưa vào Cart ...
                    if (cartitem != null)
                    {
                        if (cartitem.SL + quantity > sanpham.Soluong)
                        {
                            cartitem.SL = sanpham.Soluong;
                            Console.WriteLine("quantity + cart lớn hơn soluongsp");
                        }
                        else
                        {
                            cartitem.SL += quantity;
                            Console.WriteLine("ok 1");
                        }
                    }
                    else
                    {
                        cart.Add(new GioHang() { SL = quantity, Sanpham = sanpham });
                        Console.WriteLine("ok 2");
                    }
                }

                // Lưu cart vào Session
                SaveCartSession(cart);

                // Chuyển đến trang hiện thị Cart
                return RedirectToAction(nameof(Cart));
            }
        }
        /// xóa item trong cart

        public IActionResult RemoveCart([FromRoute] string id)
        {
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Sanpham.Masp == id);
            if (cartitem != null)
            {
               
                cart.Remove(cartitem);
            }

            SaveCartSession(cart);
            ViewData["Cart"] = "Xóa thành công";
            return RedirectToAction(nameof(Cart));
        }

        [HttpPost]
        public IActionResult UpdateCart([FromForm] string id, [FromForm] int quantity)
        {
            Console.WriteLine("Voupdate");
            Console.WriteLine("{0}", quantity);
            Console.WriteLine("{0}", id);
            Sanpham sp = _context.Sanpham.Find(id);
            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Sanpham.Masp == id);
            if (quantity > sp.Soluong)
            {
                cartitem.SL = sp.Soluong;
                ViewData["Cart"] = "Số lượng sản phẩm hiện tại không đủ";
            }
            else
            {
                // Cập nhật Cart thay đổi số lượng quantity ...
               
                if (cartitem != null)
                {
                    cartitem.SL = quantity;
                    cart.Find(p => p.Sanpham.Masp == id).SL = cartitem.SL;
                }
                //Console.WriteLine("{0}", cart.Find(p => p.Sanpham.Masp == id).SL); 
                //SaveCartSession(cart);
            }
            SaveCartSession(cart);

            // Trả về mã thành công (không có nội dung gì - chỉ để Ajax gọi)
            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> CheckOutAsync([FromForm] string email, [FromForm] string address)
        {
            var user = await GetCurrentUser();
            if (user==null)
            {
                return Redirect("/Identity/Account/Login");
            } 
            var cart = GetCartItems();

            return View(cart);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOutAsync([FromForm] string hoten, [FromForm] string diachi, [FromForm] string sdt, [FromForm] string email)
        {
            Console.WriteLine("vô hàm thử");
            Console.WriteLine("{0}", hoten);
            Console.WriteLine("{0}", diachi);
            Console.WriteLine("{0}", sdt);
            Console.WriteLine("{0}", email);
         
            //Xử lý khi đặt hàng
            var cart = GetCartItems();
            ViewData["email"] = email;
            ViewData["address"] = diachi;
            ViewData["phone"] = sdt;
            ViewData["cart"] = cart;
            
            foreach (var item in cart)
            {
                Console.WriteLine("{0}", item.SL);
            }

            var id = from hoadon in _context.Hoadon
                      orderby hoadon.Mahd descending
                      select hoadon.Mahd;
            int temp;
            if (id.Count() == 0)
            {
                temp = 1;
            }
            else
            {              
                temp = id.First();
                temp++;
            }
            Console.WriteLine("{0}",temp);
            if (!string.IsNullOrEmpty(email))
            {
                Hoadon hd = new Hoadon();
                long? total = 0;
                hd.Mahd = temp;
                hd.Nguoinhan = hoten;
                hd.Diachigiaohang = diachi;
                hd.Sdt = sdt;
                hd.Ngayhd = DateTime.Now;
                var user = await GetCurrentUser();
                hd.Makh = user.Id;
                foreach (var item in cart)
                {
                    total += item.Sanpham.Dongia * item.SL;
                }
                hd.Tongtien = total;
                if (hd.Mask != null)
                {
                    hd.Thanhtien = total - total * hd.MaskNavigation.Phantramgiamgia;
                }
                else
                {
                    hd.Thanhtien = hd.Tongtien;
                }
                hd.Trangthai = 0; // chờ xử lý
                hd.nhanvienmanv = "";
                _context.Hoadon.Add(hd);
                _context.SaveChanges();
                int lastID = hd.Mahd;

              
                // gửi mail thử
                string subject = "ĐƠN ĐẶT HÀNG";
                string body =  "LaptopStore xin gửi đến bạn thông tin đơn mua hàng" +
                               " <br></br> <div style=\" width: 700px; \">" +
                               " <div style =\"width: 95%; padding: 10px;\" > " +
                               " <h4 style = \"text-align: center;\" > ĐƠN MUA HÀNG</h4>" +
                               " <p> Ngày mua: " + hd.Ngayhd + "</p>" +
                               " <p> Họ tên: " + hoten + " </p> " +
                               " <p> SĐT: " + sdt + "</p>" +
                               " <p> Địa chỉ nhận hàng:  " + diachi + "</p>" +

                               " <h4 style = \"text-align: center;\" > CHI TIẾT ĐƠN HÀNG </h4> " +
                               "<table  style=\"width: 100%; border: 1px solid black; border-collapse: collapse; \"> " +
                                     " <tr style = \" border:1px solid black; border-collapse: collapse;background-color: #fff200;color: #5f5f5f;\" > " +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;min-width:150px;\" > Sản phẩm </th>" +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;min-width:70px;\" > Số lượng </th> " +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;min-width:100px;\" > Đơn giá </th> " +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;min-width:100px;\" > Số tiền </th> " +
                                     " </tr> ";
                foreach (var item in cart)
                {
                    Cthd ct = new Cthd();
                    ct.Mahd = lastID;
                    ct.Masp = item.Sanpham.Masp;
                    ct.Soluong = item.SL;
                    long? tong = item.SL * item.Sanpham.Dongia;
                    _context.Add(ct);
                    _context.SaveChanges();

                    // Cập nhật lại số lượng sản phẩm
                    Sanpham sp = _context.Sanpham.Where(sp => sp.Masp == item.Sanpham.Masp).First();
                    sp.Soluong -= item.SL;                  
                    _context.SaveChanges();

                    body = body +
                             " <tr style = \" border:1px solid black; border-collapse: collapse; \"> " +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;\" > "  + item.Sanpham.Tensp + "</th>" +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + item.SL + "</th>" +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + item.Sanpham.Dongia + "</th>" +
                                     " <th style = \" border:1px solid black; border-collapse: collapse;\" > " + tong + "</th>" +
                             " </tr> ";
                }
                Console.WriteLine("vô rồi đó");

                body = body +
                   " </table> " +
                   " <p> </p>" +
                   " <table style = \"text-align: center;  position:relative; left:70%; width:30%; border:1px solid black; border-collapse: collapse; \" > " +
                        " <tr style = \" border:1px solid black;border-collapse: collapse;\" > " +
                        "     <th style = \" border:1px solid black; border-collapse: collapse;\" > Thành tiền </th> " +
                         "    <th style = \" border:1px solid black;\" > " + hd.Tongtien + " </th>" +
                         " </tr> " +
                         " <tr style = \" border:1px solid black;border-collapse: collapse;\" > "+
                          "   <th style = \" border:1px solid black; border-collapse: collapse; \" > Giảm giá </th> " +
                           "  <th style = \" border:1px solid black; \" > 0 </th> " +
                          " </tr> " +
                          " <tr style = \" border:1px solid black;border-collapse: collapse; background-color: #ff9100;color: white;line-height: 20px; \" > " +
                            "   <th style = \" border:1px solid black; border-collapse: collapse;\" > Tổng tiền </th> " +
                             "  <th style = \" border:1px solid black;\" >" + hd.Thanhtien + " </th> " +
                          " </tr> " +
                   " </table> " +
                   " <p> Cảm ơn quý khách đã mua sắm tại LaptopStore, chúc quý khách một ngày vui vẻ! </p>" +
                   " </div> " +
                   " </div>";


                    await _sendMail.SendEmailAsync(email, subject, body);

                Console.WriteLine("gửi mail ok");
                ClearCart();
                RedirectToAction(nameof(Index));
            }

            return Redirect("/Home/Index");
        }
    }
}

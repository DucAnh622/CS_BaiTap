using BaiTap_Web1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Globalization;
using System.Drawing;
using System.Web.Razor.Tokenizer.Symbols;
using System.Collections;

namespace BaiTap_Web1.Controllers
{
    public class NhanVienController : Controller
    {
        QLNHANVIENDataContext db = new QLNHANVIENDataContext();

        // GET: NhanVien
        public ActionResult Index()
        {
            string keyword = Request["keyword"];
            if(!string.IsNullOrEmpty(keyword))
            {
                // Lấy tổng số bản ghi
                var Total = db.NHANVIENs.Where(o => o.TenNV.Contains(keyword) || o.MaNV.Equals(keyword) || o.ChucVu.Contains(keyword) || o.QueQuan.Contains(keyword)).Count();
                // Tạo biến TotalPage ở view index
                ViewData["Total"] = Total;
            }
            else
            {
                // Lấy tổng số bản ghi
                var Total = db.NHANVIENs.Count();
                // Tạo biến TotalPage ở view index
                ViewData["Total"] = Total;
            }
            return View();
        }

        // Lấy ra danh sách nhân viên
        public string GetList(string pageItem, string keyword, string limit, string orderBy, string orderDir)
        {
            int offset = (int.Parse(pageItem) - 1) * int.Parse(limit); 
            if (!string.IsNullOrEmpty(keyword))
            {
                var dsnv = db.NHANVIENs.Where(o => o.TenNV.Contains(keyword) || o.MaNV.Equals(keyword) || o.ChucVu.Contains(keyword) || o.QueQuan.Contains(keyword)).Skip(offset).Take(int.Parse(limit)).ToList();
                return JsonConvert.SerializeObject(dsnv);
            }
            else
            {
                // var dsnv = db.NHANVIENs.Skip(offset).Take(int.Parse(limit)).ToList();
                // return JsonConvert.SerializeObject(dsnv);
                // Tạo truy vấn ban đầu
                var query = db.NHANVIENs.AsQueryable();

                // Sắp xếp theo trường được chọn và hướng sắp xếp
                switch (orderBy)
                {
                    case "MaNV":
                        query = (orderDir == "asc") ? query.OrderBy(o => o.MaNV) : query.OrderByDescending(o => o.MaNV);
                        break;
                    case "TenNV":
                        query = (orderDir == "asc") ? query.OrderBy(o => o.TenNV) : query.OrderByDescending(o => o.TenNV);
                        break;
                    case "ChucVu":
                        query = (orderDir == "asc") ? query.OrderBy(o => o.ChucVu) : query.OrderByDescending(o => o.ChucVu);
                        break;
                    case "QueQuan":
                        query = (orderDir == "asc") ? query.OrderBy(o => o.QueQuan) : query.OrderByDescending(o => o.QueQuan);
                        break;
                    default:
                        query = query.OrderBy(e => e.MaNV);
                        break;
                }

                // Lấy danh sách nhân viên và chuyển đổi thành chuỗi JSON
                var dsnv = query.Skip(offset).Take(int.Parse(limit)).ToList();
                return JsonConvert.SerializeObject(dsnv);
            }
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }

        public string GetObject(string maNV)
        {
            NHANVIEN nhanvien = new NHANVIEN();
            nhanvien = db.NHANVIENs.Where(o=>o.MaNV.Equals(maNV)).FirstOrDefault();
            return JsonConvert.SerializeObject(nhanvien);
        }

        [HttpPost]
        public string PostCreate()
        {
            Result_ett<string> rs = new Result_ett<string>();
            try
            {
                string tenNV = Request["TenNV"];
                string maNV = Request["MaNV"];
                string chucVu = Request["ChucVu"];
                string queQuan = Request["QueQuan"];
                string ngaySinh = Request["NgaySinh"];
                string gioiTinh = Request["GioiTinh"];

                NHANVIEN newNV = new NHANVIEN();
                newNV.TenNV = tenNV;
                newNV.MaNV = maNV;
                newNV.ChucVu = chucVu;
                newNV.QueQuan = queQuan;
                newNV.NgaySinh = DateTime.ParseExact(ngaySinh,"yyyy-MM-dd",CultureInfo.InvariantCulture);
                newNV.GioiTinh =gioiTinh;
                db.NHANVIENs.InsertOnSubmit(newNV);
                db.SubmitChanges();
                rs.ErrCode = EnumErrCode.Success;
                rs.Message = "Thêm mới sinh viên thành công!";
            }
            catch (Exception ex) {
                rs.ErrCode = EnumErrCode.Fail;
                rs.ErrDetail = ex.Message;
                rs.Message = "Thêm mới không thành công!";
                return JsonConvert.SerializeObject(rs);
            }
            return JsonConvert.SerializeObject(rs);
        }

        [HttpPost]
        public string PostEdit()
        {
            Result_ett<string> rs = new Result_ett<string>();
            try
            {
                string tenNV = Request["TenNV"];
                string maNV = Request["MaNV"];
                string chucVu = Request["ChucVu"];
                string queQuan = Request["QueQuan"];
                string ngaySinh = Request["NgaySinh"];
                string gioiTinh = Request["GioiTinh"];

                NHANVIEN newNV = db.NHANVIENs.Where(o => o.MaNV.Equals(maNV)).FirstOrDefault();
                newNV.TenNV = tenNV;
                newNV.ChucVu = chucVu;
                newNV.QueQuan = queQuan;
                newNV.NgaySinh = DateTime.ParseExact(ngaySinh, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                newNV.GioiTinh = gioiTinh;
                db.SubmitChanges();
                rs.ErrCode = EnumErrCode.Success;
                rs.Message = "Cập nhật thông tin nhân viên thành công";
            }
            catch (Exception ex)
            {
                rs.ErrCode = EnumErrCode.Fail;
                rs.ErrDetail = ex.Message;
                rs.Message = "Cập nhật thông tin nhân viên không thành công";
                return JsonConvert.SerializeObject(rs);
            }
            return JsonConvert.SerializeObject(rs);
        }
        [HttpPost]
        public string Delete(string maNV)
        {
            Result_ett<string> rs = new Result_ett<string>();
            try
            {
                NHANVIEN delObj = db.NHANVIENs.Where(o => o.MaNV.Equals(maNV)).FirstOrDefault();
                db.NHANVIENs.DeleteOnSubmit(delObj);
                db.SubmitChanges();
                rs.ErrCode = EnumErrCode.Success;
                rs.Message = "Xóa nhân viên thành công";
            }
            catch (Exception ex)
            {
                rs.ErrCode = EnumErrCode.Fail;
                rs.ErrDetail = ex.Message;
                rs.Message = "Xóa nhân viên không thành công";
                return JsonConvert.SerializeObject(rs);
            }
            return JsonConvert.SerializeObject(rs);
        }
    }
}
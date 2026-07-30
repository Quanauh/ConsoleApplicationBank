using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
class ThongTinTaiKhoan
{
    public string? SoTK;
    public string? HoTen;
    public string? SDT;
    public string? Email;
    public DateTime? NgaySinh;
    public string? DiaChi;
    public decimal SoDu;
    public ThongTinTaiKhoan(string soTK,string hoTen, string sdt,string email,DateTime? ngaySinh,string diaChi,decimal soDu)
    {
        this.SoTK = soTK;
        this.HoTen = hoTen;
        this.SDT = sdt;
        this.Email = email;
        this.NgaySinh = ngaySinh;
        this.DiaChi = diaChi;
        this.SoDu = soDu;
    }

}
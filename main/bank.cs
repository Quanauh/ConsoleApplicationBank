using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
//Kết quả giao dịch - dùng chung cho mọi thao tác backend trả về
enum KetQuaGiaoDich
{
    ThanhCong,
    SoTaiKhoanDaTonTai,
    SoTaiKhoanKhongTonTai,
    SaiMatKhau,
    SoTienKhongHopLe,
    SoDuKhongDu,
    ChuyenChoChinhMinh,
    SdtDaTonTai,
    EmailDaTonTai,
    LoiHeThong,
    NgaySinhKhongHopLe

}

//Quản lý ngân hàng - toàn bộ logic nghiệp vụ và validate nằm ở đây,
//không Console.WriteLine, chỉ trả về KetQuaGiaoDich cho UI tự quyết định hiển thị gì
class BankManager
{
    AccountDAO acd=new AccountDAO();
    public bool Timstk(string stk)
    {
        return acd.TonTai(stk);
    }

    //Tạo tài khoản
    public KetQuaGiaoDich TaoTk(string ten, string sdt,string email,string ngaysinh,string diachi,string stk, string mk)
{
    try{
     if(acd.TonTai(stk)) return  KetQuaGiaoDich.SoTaiKhoanDaTonTai;  
    else {acd.TaoTk(ten,sdt,email,ngaysinh,diachi,stk,mk);
    return KetQuaGiaoDich.ThanhCong;
    }
    }
        catch (OracleException ex)
    {
        Console.WriteLine($"Error Number: {ex.Number}");
        Console.WriteLine($"Error Message: {ex.Message}");
        if (ex.Number >0) 
        {
            if (ex.Message.Contains("UQ_KHACHHANG_SDT"))
                return KetQuaGiaoDich.SdtDaTonTai;

            if (ex.Message.Contains("UQ_KHACHHANG_EMAIL"))
                return KetQuaGiaoDich.EmailDaTonTai;

            if (ex.Message.Contains("PK_TAI_KHOAN"))
                return KetQuaGiaoDich.SoTaiKhoanDaTonTai;
            if (ex.Message.Contains("month"))
                return KetQuaGiaoDich.NgaySinhKhongHopLe;
        }

        return KetQuaGiaoDich.LoiHeThong;
    }
}   

    //Xử lý đăng nhập (gộp check tồn tại + check mật khẩu)
    public KetQuaGiaoDich DangNhap(string stk, string mk)
    {
        if (!Timstk(stk))
            return KetQuaGiaoDich.SoTaiKhoanKhongTonTai;

        if (!acd.checkMk(stk,mk))
            return KetQuaGiaoDich.SaiMatKhau;

        return KetQuaGiaoDich.ThanhCong;
    }

    //Xử lý nạp tiền (check luôn cả sự tồn tại của tài khoản để dùng được cả khi chưa đăng nhập)
    public KetQuaGiaoDich Naptien(string stk, decimal a)
    {
        if (!Timstk(stk))
            return KetQuaGiaoDich.SoTaiKhoanKhongTonTai;

        if (a <= 0)
            return KetQuaGiaoDich.SoTienKhongHopLe;

        acd.Naptien(stk,a);
        LuuLichsu(stk,null,"NAP",a);
        return KetQuaGiaoDich.ThanhCong;
    }

    //Xử lý rút tiền
    public KetQuaGiaoDich RutTien(string stk, decimal a)
    {
        if (!Timstk(stk))
            return KetQuaGiaoDich.SoTaiKhoanKhongTonTai;

        if (a <= 0)
            return KetQuaGiaoDich.SoTienKhongHopLe;

        if (a > acd.checksd(stk))
            return KetQuaGiaoDich.SoDuKhongDu;

        acd.RutTien(stk,a);
        LuuLichsu(stk,null,"RUT",a);
        return KetQuaGiaoDich.ThanhCong;
    }

    //Xử lý chuyển tiền
    public KetQuaGiaoDich ChuyenTien(string stk1, string stk2, decimal a)
    {
        if (stk1 == stk2)
            return KetQuaGiaoDich.ChuyenChoChinhMinh;

        if (!Timstk(stk2))
            return KetQuaGiaoDich.SoTaiKhoanKhongTonTai;

        if (a <= 0)
            return KetQuaGiaoDich.SoTienKhongHopLe;

        if (a > acd.checksd(stk1))
            return KetQuaGiaoDich.SoDuKhongDu;

        acd.RutTien(stk1,a);
        acd.Naptien(stk2,a);
        LuuLichsu(stk1,stk2,"CHUYEN",a);
        LuuLichsu(stk2,stk1,"NHAN",a);
        return KetQuaGiaoDich.ThanhCong;
    }

    //Xem số dư
    public decimal XemSodu(string stk)
    {
        return acd.checksd(stk);
    }
    public ThongTinTaiKhoan? laythongtin(string stk)
    {
        return acd.LayThongTin(stk);
    }
    public void LuuLichsu(string stk1,string? stk2,string loai,decimal a)
    {
        acd.GhiLichSu(stk1,stk2,loai,a);
    }
    public List<LichSuGiaoDich> LayLichSu(string stk)
    {
        return acd.LayLichSu(stk);
    }
}


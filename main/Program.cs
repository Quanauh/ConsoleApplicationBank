//UI - chỉ nhập liệu, gọi backend, và hiển thị kết quả dựa trên KetQuaGiaoDich trả về.
class Program
{
    static BankManager qlnh = new BankManager();

    //giao diện ban đầu
    static void gdMenu()
    {
        Console.WriteLine("        ===NganHangHuanBank===      ");
        Console.WriteLine("1.Tao tai khoan");
        Console.WriteLine("2.Dang nhap tai khoan");
        Console.WriteLine("3.Nap tien");
        Console.WriteLine("0.Thoat");
        Console.Write("Chon chuc nang: ");
    }

    //Giao diện tạo tài khoản
    static void gdTaoTK()
    {
        Console.WriteLine("        ===TaoTK===      ");
        Console.Write("Nhap ten cua ban:");
        string ten = Console.ReadLine()!;
        Console.Write("Nhap so dien thoai cua ban:");
        string sdt = Console.ReadLine()!;
        Console.Write("Nhap email cua ban:");
        string email = Console.ReadLine()!;
        Console.Write("Nhap ngay sinh cua ban:");
        string ngaysinh = Console.ReadLine()!;
        Console.Write("Nhap dia chi cua ban:");
        string diachi = Console.ReadLine()!;
        Console.Write("Nhap so tai khoan muon tao: ");
        string stk = Console.ReadLine()!;
        Console.Write("Nhap mat khau muon dat: ");
        string mk = Console.ReadLine()!;

        var kq = qlnh.TaoTk(ten,sdt,email,ngaysinh,diachi,stk, mk);
        switch (kq)
        {
            case KetQuaGiaoDich.SdtDaTonTai:
                Console.WriteLine("So dien thoai da ton tai");
                break;
            case KetQuaGiaoDich.EmailDaTonTai:
                Console.WriteLine("Email thoai da ton tai");
                break;
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Tao tai khoan thanh cong");
                break;
            case KetQuaGiaoDich.SoTaiKhoanDaTonTai:
                Console.WriteLine("So tai khoan da ton tai");
                break;
            case KetQuaGiaoDich.NgaySinhKhongHopLe:
                Console.WriteLine("Ngay sinh khong hop le");
                break;
            case KetQuaGiaoDich.LoiHeThong:
                Console.WriteLine("Loi SQL");
                break;
            default:
                Console.WriteLine("Loi");
                break;
        }
    }

    //Giao diện đăng nhập
    static void gdDangNhap()
    {
        Console.WriteLine("        ===Dang Nhap===      ");
        Console.Write("Nhap stk cua ban: ");
        string stk = Console.ReadLine()!;
        Console.Write("Nhap mat khau cua ban: ");
        string mk = Console.ReadLine()!;

        var kq = qlnh.DangNhap(stk, mk);
        switch (kq)
        {
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Ban da dang nhap thanh cong");
                gdXuly(stk);
                break;
            case KetQuaGiaoDich.SoTaiKhoanKhongTonTai:
                Console.WriteLine("Khong tim thay tai khoan");
                break;
            case KetQuaGiaoDich.SaiMatKhau:
                Console.WriteLine("Ban da nhap sai mat khau");
                break;
        }
    }

    //Giao diện xử lý sau khi đăng nhập
    static void gdXuly(string stk)
    {
        Console.WriteLine($"        ===Xin Chao {stk}===      ");
        Console.WriteLine("1. Chuyen tien");
        Console.WriteLine("2. Nap Tien");
        Console.WriteLine("3. Rut tien");
        Console.WriteLine("4. Xem so du");
        Console.WriteLine("5. Xem thong tin tai khoan");
        Console.WriteLine("6. Xem Lich su giao dich");
        Console.WriteLine("0. Quay lai");
        Console.Write("Chon chuc nang: ");
        string t = Console.ReadLine()!;
        switch (t)
        {
            case "1":
                gdChuyentien(stk);
                break;
            case "2":
                gdNapTien2(stk);
                break;
            case "3":
                gdRuttien(stk);
                break;
            case "4":
                gdXemsodu(stk);
                break;
            case "5":
                gdXemThongTin(stk);
                break;
            case "6":
                gdXemLichSugd(stk);
                break;
            case "0":
                break;
            default:
                Console.WriteLine("Lua chon khong hop le!");
                gdXuly(stk);
                break;
        }
    }
    static void gdXemLichSugd(string stk)
    {
        List<LichSuGiaoDich> ds=qlnh.LayLichSu(stk);
        Console.WriteLine("        ===LichSuGiaoDich===      ");
        if (ds.Count == 0)
    {
    Console.WriteLine("Không có lịch sử giao dịch.");
    }
        else {
            foreach(LichSuGiaoDich x in ds){
            Console.WriteLine(x);
        }
        }
        Console.Write("Nhan phim bat ki de thoat");
        Console.ReadKey();
        gdXuly(stk);
    }
    //Giao diện xem thông tin khách hàng
    static void gdXemThongTin(string stk)
    {
        ThongTinTaiKhoan? a=qlnh.laythongtin(stk);
        Console.WriteLine("        ===Thong tin tai khoan===      ");
        Console.WriteLine($"Ten chu tai khoan: {a?.HoTen}");
        Console.WriteLine($"SDT chu tai khoan:: {a?.SDT}");
        Console.WriteLine($"Email chu tai khoan: {a?.Email}");
        Console.WriteLine($"Ngay Sinh chu tai khoan: {a?.NgaySinh:dd/MM/yyyy}");
        Console.WriteLine($"Dia chi chu tai khoan: {a?.DiaChi}");
        Console.Write("Nhan phim bat ki de thoat");
        Console.ReadKey();
        Console.WriteLine();
        gdXuly(stk);
    }

    //Giao diện chuyển tiền
    static void gdChuyentien(string stk)
    {
        Console.WriteLine("        ===Chuyen Tien===      ");
        Console.Write("Nhap so tai khoan can chuyen: ");
        string stk2 = Console.ReadLine()!;
        Console.Write("Nhap so tien can chuyen: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal a))
        {
            Console.WriteLine("Vui long nhap so tien hop le");
            gdXuly(stk);
            return;
        }

        var kq = qlnh.ChuyenTien(stk, stk2, a);
        switch (kq)
        {
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Chuyen tien thanh cong");
                Console.WriteLine($"So du cua ban la: {qlnh.XemSodu(stk)}");
                break;
            case KetQuaGiaoDich.ChuyenChoChinhMinh:
                Console.WriteLine("Ban khong the chuyen tien cho chinh minh");
                break;
            case KetQuaGiaoDich.SoTaiKhoanKhongTonTai:
                Console.WriteLine("So tai khoan khong ton tai");
                break;
            case KetQuaGiaoDich.SoTienKhongHopLe:
                Console.WriteLine("So tien khong hop le");
                break;
            case KetQuaGiaoDich.SoDuKhongDu:
                Console.WriteLine("So du cua ban khong du");
                break;
        }
        gdXuly(stk);
    }

    //Giao diện rút tiền
    static void gdRuttien(string stk)
    {
        Console.WriteLine("        ===Rut Tien===      ");
        Console.Write("Nhap so tien can rut: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal a))
        {
            Console.WriteLine("Vui long nhap so tien hop le");
            gdXuly(stk);
            return;
        }

        var kq = qlnh.RutTien(stk, a);
        switch (kq)
        {
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Rut tien thanh cong");
                break;
            case KetQuaGiaoDich.SoTienKhongHopLe:
                Console.WriteLine("So tien khong hop le");
                break;
            case KetQuaGiaoDich.SoDuKhongDu:
                Console.WriteLine("So du khong du");
                break;
        }
        gdXuly(stk);
    }

    //Giao diện nạp tiền khi ko đăng nhập
    static void gdNapTien()
    {
        Console.WriteLine("        ===Nap Tien===      ");
        Console.Write("Nhap so tai khoan can nap: ");
        string stk = Console.ReadLine()!;
        Console.Write("Nhap so tien can nap: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal a))
        {
            Console.WriteLine("Vui long nhap so tien hop le");
            return;
        }

        var kq = qlnh.Naptien(stk, a);
        switch (kq)
        {
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Nap tien thanh cong");
                break;
            case KetQuaGiaoDich.SoTaiKhoanKhongTonTai:
                Console.WriteLine("So tai khoan khong ton tai");
                break;
            case KetQuaGiaoDich.SoTienKhongHopLe:
                Console.WriteLine("So tien khong hop le");
                break;
        }
    }

    //Giao diện nạp tiền chính khi đã đăng nhập
    static void gdNapTien2(string stk)
    {
        Console.WriteLine("        ===Nap Tien===      ");
        Console.Write("Nhap so tien can nap: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal a))
        {
            Console.WriteLine("Vui long nhap so tien hop le");
            gdXuly(stk);
            return;
        }

        var kq = qlnh.Naptien(stk, a);
        switch (kq)
        {
            case KetQuaGiaoDich.ThanhCong:
                Console.WriteLine("Nap tien thanh cong");
                break;
            case KetQuaGiaoDich.SoTienKhongHopLe:
                Console.WriteLine("So tien khong hop le");
                break;
        }
        gdXuly(stk);
    }

    //Giao diện xem số dư
    static void gdXemsodu(string stk)
    {
        decimal sd = qlnh.XemSodu(stk);
        Console.WriteLine($"So du cua ban la: {sd}");
        Console.Write("Nhan phim bat ki de thoat");
        Console.ReadKey();
        Console.WriteLine();
        gdXuly(stk);
    }

    public static void Main(string[] args)
    {
        bool ok = true;
        while (ok)
        {
            gdMenu();
            string t = Console.ReadLine()!;
            switch (t)
            {
                case "1":
                    gdTaoTK();
                    break;
                case "2":
                    gdDangNhap();
                    break;
                case "3":
                    gdNapTien();
                    break;
                case "0":
                    ok = false;
                    break;
                default:
                    Console.WriteLine("Lua chon khong hop le!");
                    break;
            }
        }
    }
}
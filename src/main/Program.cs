    //UI - chỉ nhập liệu, gọi backend, và hiển thị kết quả dựa trên KetQuaGiaoDich trả về.
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Microsoft.Extensions.Logging;
    class Program
    {
        
        static BankManager qlnh= null!;
        
        //giao diện ban đầu
        static void ShowMenu()
        {
            Console.WriteLine("        ===NganHangHuanBank===      ");
            Console.WriteLine("1.Tao tai khoan");
            Console.WriteLine("2.Dang nhap tai khoan");
            Console.WriteLine("3.Nap tien");
            Console.WriteLine("0.Thoat");
            Console.Write("Chon chuc nang: ");
        }

        //Giao diện tạo tài khoản
        static void RegisterScreen()
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

            var kq = qlnh.CreateAccount(ten,sdt,email,ngaysinh,diachi,stk, mk);
            switch (kq)
            {
                case TransactionResult.PhoneAlreadyExists:
                    Console.WriteLine("So dien thoai da ton tai");
                    break;
                case TransactionResult.EmailAlreadyExists:
                    Console.WriteLine("Email thoai da ton tai");
                    break;
                case TransactionResult.Success:
                    Console.WriteLine("Tao tai khoan thanh cong");
                    break;
                case TransactionResult.AccountAlreadyExists:
                    Console.WriteLine("So tai khoan da ton tai");
                    break;
                case TransactionResult.InvalidDate:
                    Console.WriteLine("Ngay sinh khong hop le");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
                    break;
                case TransactionResult.IsNull:
                    Console.WriteLine("Thong tin khong duoc de trong");
                    break;    
                default:
                    Console.WriteLine("Loi");
                    break;
            }
        }

        //Giao diện đăng nhập
        static void LoginScreen()
        {
            Console.WriteLine("        ===Dang Nhap===      ");
            Console.Write("Nhap stk cua ban: ");
            string stk = Console.ReadLine()!;
            Console.Write("Nhap mat khau cua ban: ");
            string mk = Console.ReadLine()!;

            var kq = qlnh.Login(stk, mk);
            switch (kq)
            {
                case TransactionResult.Success:
                    Console.WriteLine("Ban da dang nhap thanh cong");
                    gdXuly(stk);
                    break;
                case TransactionResult.AccountNotFound:
                    Console.WriteLine("Khong tim thay tai khoan");
                    break;
                case TransactionResult.IncorrectPassword:
                    Console.WriteLine("Ban da nhap sai mat khau");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
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
                //Gd chuyentien
                    TransferScreen(stk);
                    break;
                case "2":
                    DepositScreen2(stk);
                    break;
                case "3":
                    WithdrawScreen(stk);
                    break;
                case "4":
                    BalanceScreen(stk);
                    break;
                case "5":
                    InforScreen(stk);
                    break;
                case "6":
                    GetHistoryScreen(stk);
                    break;
                case "0":
                    break;
                default:
                    Console.WriteLine("Lua chon khong hop le!");
                    gdXuly(stk);
                    break;
            }
        }
        // Giao diện xem lịch sử giao dịch
        static void GetHistoryScreen(string stk)
        {
            List<TransactionHistory> ds=qlnh.GetTransactionHistory(stk);
            Console.WriteLine("        ===LichSuGiaoDich===      ");
            if (ds.Count == 0)
        {
        Console.WriteLine("Không có lịch sử giao dịch.");
        }
            else {
                foreach(TransactionHistory x in ds){
                Console.WriteLine(x);
            }
            }
            Console.Write("Nhan phim bat ki de thoat");
            Console.ReadKey();
            gdXuly(stk);
        }
        //Giao diện xem thông tin khách hàng
        static void InforScreen(string stk)
        {
            AccountInfo? a=qlnh.GetAccountInfor(stk);
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
        static void TransferScreen(string stk)
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

            var kq = qlnh.TransferMoney(stk, stk2, a);
            switch (kq)
            {
                case TransactionResult.Success:
                    Console.WriteLine("Chuyen tien thanh cong");
                    Console.WriteLine($"So du cua ban la: {qlnh.GetBalance(stk)}");
                    break;
                case TransactionResult.CannotTransferToSelf:
                    Console.WriteLine("Ban khong the chuyen tien cho chinh minh");
                    break;
                case TransactionResult.AccountNotFound:
                    Console.WriteLine("So tai khoan khong ton tai");
                    break;
                case TransactionResult.InvalidAmount:
                    Console.WriteLine("So tien khong hop le");
                    break;
                case TransactionResult.InsufficientBalance:
                    Console.WriteLine("So du cua ban khong du");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
                    break;
            }
            gdXuly(stk);
        }

        //Giao diện rút tiền
        static void WithdrawScreen(string stk)
        {
            Console.WriteLine("        ===Rut Tien===      ");
            Console.Write("Nhap so tien can rut: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal a))
            {
                Console.WriteLine("Vui long nhap so tien hop le");
                gdXuly(stk);
                return;
            }

            var kq = qlnh.Withdraw(stk, a);
            switch (kq)
            {
                case TransactionResult.Success:
                    Console.WriteLine("Rut tien thanh cong");
                    break;
                case TransactionResult.InvalidAmount:
                    Console.WriteLine("So tien khong hop le");
                    break;
                case TransactionResult.InsufficientBalance:
                    Console.WriteLine("So du khong du");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
                    break;
            }
            gdXuly(stk);
        }

        //Giao diện nạp tiền khi ko đăng nhập
        static void DepositScreen1()
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

            var kq = qlnh.Deposit(stk, a);
            switch (kq)
            {
                case TransactionResult.Success:
                    Console.WriteLine("Nap tien thanh cong");
                    break;
                case TransactionResult.AccountNotFound:
                    Console.WriteLine("So tai khoan khong ton tai");
                    break;
                case TransactionResult.InvalidAmount:
                    Console.WriteLine("So tien khong hop le");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
                    break;
            }
        }

        //Giao diện nạp tiền chính khi đã đăng nhập
        static void DepositScreen2(string stk)
        {
            Console.WriteLine("        ===Nap Tien===      ");
            Console.Write("Nhap so tien can nap: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal a))
            {
                Console.WriteLine("Vui long nhap so tien hop le");
                gdXuly(stk);
                return;
            }

            var kq = qlnh.Deposit(stk, a);
            switch (kq)
            {
                case TransactionResult.Success:
                    Console.WriteLine("Nap tien thanh cong");
                    break;
                case TransactionResult.InvalidAmount:
                    Console.WriteLine("So tien khong hop le");
                    break;
                case TransactionResult.SystemError:
                    Console.WriteLine("Loi SQL");
                    break;
            }
            gdXuly(stk);
        }

        //Giao diện xem số dư
        static void BalanceScreen(string stk)
        {
            decimal sd = qlnh.GetBalance(stk);
            Console.WriteLine($"So du cua ban la: {sd}");
            Console.Write("Nhan phim bat ki de thoat");
            Console.ReadKey();
            Console.WriteLine();
            gdXuly(stk);
        }
        //Main
        public static void Main(string[] args)
        {
           Log.Logger = new LoggerConfiguration()
        .WriteTo.File(
            path: "logs/log_.txt",
            rollingInterval: RollingInterval.Day)
        .CreateLogger();

    var services = new ServiceCollection();
    services.AddLogging(builder => builder.AddSerilog(dispose: true));
    services.AddSingleton<IAccountDAO, AccountDAO>();
    services.AddSingleton<BankManager>();
    var provider = services.BuildServiceProvider();

    qlnh = provider.GetRequiredService<BankManager>();

    var logger = provider.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Khoi dong chuong trinh thanh cong");
        bool ok=true;
            while (ok)
            {
                ShowMenu();
                string t = Console.ReadLine()!;
                switch (t)
                {
                    case "1":
                        RegisterScreen();
                        break;
                    case "2":
                        LoginScreen();
                        break;
                    case "3":
                        DepositScreen1();
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
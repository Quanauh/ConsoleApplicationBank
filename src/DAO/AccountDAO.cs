using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Oracle.ManagedDataAccess.Client;

class AccountDAO
{
    //kiem tra ton tai
    public bool Exists(string stk)
    {
        using OracleConnection conn = Database.GetConnection();
        int dem = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM TAI_KHOAN WHERE SoTK=:stk", new { stk });
        return dem > 0;
    }

    public bool checkPassWord(string stk, string mk)
    {
        using OracleConnection conn = Database.GetConnection();
        int dem = conn.ExecuteScalar<int>(
            "SELECT COUNT(*) FROM tai_khoan WHERE sotk=:stk AND matkhau=:mk", new { stk, mk });
        return dem > 0;
    }

    //Lấy giá trị tiếp theo của 1 sequence - dùng code thay cho trigger
    private int GetNextId(OracleConnection conn, OracleTransaction tran, string tenSequence)
    {
        return conn.ExecuteScalar<int>($"SELECT {tenSequence}.NEXTVAL FROM dual", transaction: tran);
    }

    // Tạo tài khoản
    public void CreateAccount(string ten, string sdt, string email, string ngaysinh, string diachi, string stk, string mk)
    {
        using OracleConnection conn = Database.GetConnection();
        using OracleTransaction trann = conn.BeginTransaction();
        try
        {
            int maKh = GetNextId(conn, trann, "seq_khach_hang");

            conn.Execute(
                @"INSERT INTO khach_hang(MAKH,HOTEN,SDT,EMAIL,NGAYSINH,DIACHI)
                  VALUES (:maKh,:ten,:sdt,:email,TO_DATE(:ngaysinh,'dd/MM/YYYY'),:diachi)",
                new { maKh, ten, sdt, email, ngaysinh, diachi }, trann);

            conn.Execute(
                "INSERT INTO tai_khoan (SOTK,MAKH,MATKHAU) VALUES (:stk,:maKh,:mk)",
                new { stk, maKh, mk }, trann);

            trann.Commit();
        }
        catch
        {
            trann.Rollback();
            throw;
        }
    }

    //Ghi 1 dòng số dư sau giao dịch cho 1 tài khoản, gắn với 1 mã giao dịch (maGd)
    private void BalanceAfterTranaction(OracleConnection conn, OracleTransaction tran, int maGd, string stk, decimal soDuMoi)
    {
        int maSoDu = GetNextId(conn, tran, "seq_so_du_sau_gd");
        conn.Execute(
            "INSERT INTO SO_DU_SAU_GD (MASODU,MAGD,SOTK,SODUSAUGD) VALUES (:maSoDu,:maGd,:stk,:soDu)",
            new { maSoDu, maGd, stk, soDu = soDuMoi }, tran);
    }

    //Cộng/trừ số dư 1 tài khoản, trả về số dư mới luôn (dùng RETURNING, tránh phải SELECT lại)
    //Dapper không trực tiếp trả về output parameter qua kết quả Execute, nên dùng DynamicParameters
    //để khai báo :soDuMoi là tham số OUTPUT rồi đọc lại bằng p.Get<decimal>(...)
    private decimal UpdateBalance(OracleConnection conn, OracleTransaction tran, string stk, decimal a)
    {
        var p = new DynamicParameters();
        p.Add(":delta", a);
        p.Add(":stk", stk);
        p.Add(":soDuMoi", dbType: DbType.Decimal, direction: ParameterDirection.Output);

        conn.Execute(
            "UPDATE TAI_KHOAN SET SODU = SODU + :delta WHERE SOTK = :stk RETURNING SODU INTO :soDuMoi",
            p, tran);

        return p.Get<decimal>(":soDuMoi");
    }

    //chuyen tien - 1 dong GIAO_DICH + 2 dong SO_DU_SAU_GD (ben gui, ben nhan), cung 1 transaction
    public void TransferMoney(string stk1, string stk2, decimal a)
    {
        using OracleConnection conn = Database.GetConnection();
        using OracleTransaction trann = conn.BeginTransaction();
        try
        {
            decimal soDuStk1 = UpdateBalance(conn, trann, stk1, -a);
            decimal soDuStk2 = UpdateBalance(conn, trann, stk2, a);

            int maGd = GetNextId(conn, trann, "seq_giao_dich");
            conn.Execute(
                "INSERT INTO GIAO_DICH (MAGD,SOTK,SOTK_DOI,LOAIGD,SOTIEN) VALUES (:maGd,:stk1,:stk2,'CHUYEN',:a)",
                new { maGd, stk1, stk2, a }, trann);

            BalanceAfterTranaction(conn, trann, maGd, stk1, soDuStk1);
            BalanceAfterTranaction(conn, trann, maGd, stk2, soDuStk2);

            trann.Commit();
        }
        catch
        {
            trann.Rollback();
            throw;
        }
    }

    //Nạp tiền - 1 dong GIAO_DICH + 1 dong SO_DU_SAU_GD, cung 1 transaction
    public void Deposit(string stk, decimal a)
    {
        using OracleConnection conn = Database.GetConnection();
        using OracleTransaction trann = conn.BeginTransaction();
        try
        {
            decimal soDuMoi = UpdateBalance(conn, trann, stk, a);

            int maGd = GetNextId(conn, trann, "seq_giao_dich");
            conn.Execute(
                "INSERT INTO GIAO_DICH (MAGD,SOTK,SOTK_DOI,LOAIGD,SOTIEN) VALUES (:maGd,:stk,NULL,'NAP',:a)",
                new { maGd, stk, a }, trann);

            BalanceAfterTranaction(conn, trann, maGd, stk, soDuMoi);
            trann.Commit();
        }
        catch
        {
            trann.Rollback();
            throw;
        }
    }

    public decimal GetBalance(string stk)
    {
        using OracleConnection conn = Database.GetConnection();
        return conn.ExecuteScalar<decimal>("SELECT sodu FROM tai_khoan WHERE SOTK=:stk", new { stk });
    }

    //Rut tien - 1 dong GIAO_DICH + 1 dong SO_DU_SAU_GD, cung 1 transaction
    public void Withdraw(string stk, decimal a)
    {
        using OracleConnection conn = Database.GetConnection();
        using OracleTransaction trann = conn.BeginTransaction();
        try
        {
            decimal soDuMoi = UpdateBalance(conn, trann, stk, -a);

            int maGd = GetNextId(conn, trann, "seq_giao_dich");
            conn.Execute(
                "INSERT INTO GIAO_DICH (MAGD,SOTK,SOTK_DOI,LOAIGD,SOTIEN) VALUES (:maGd,:stk,NULL,'RUT',:a)",
                new { maGd, stk, a }, trann);

            BalanceAfterTranaction(conn, trann, maGd, stk, soDuMoi);
            trann.Commit();
        }
        catch
        {
            trann.Rollback();
            throw;
        }
    }

    public AccountInfo? GetAccountInfor(string stk)
    {
        using OracleConnection conn = Database.GetConnection();

        string sql = @"SELECT tk.SOTK, kh.HOTEN, kh.SDT, kh.EMAIL, kh.NGAYSINH, kh.DIACHI, tk.SODU
                       FROM TAI_KHOAN tk
                       JOIN KHACH_HANG kh ON tk.MAKH = kh.MAKH
                       WHERE tk.SOTK = :stk";

        return conn.QueryFirstOrDefault<AccountInfo>(sql, new { stk });
    }

    //Lấy lịch sử giao dịch của 1 tài khoản, kèm đúng số dư của TÀI KHOẢN ĐÓ sau mỗi giao dịch.
    //Dùng 3 tên tham số khác nhau (stk1/stk2/stk3) dù cùng 1 giá trị stk, để né việc Dapper
    //không tự set BindByName=true cho Oracle (mặc định ODP.NET bind theo VỊ TRÍ, không theo TÊN,
    //nên 1 tham số dùng lại nhiều lần trong SQL sẽ báo lỗi thiếu biến nếu không xử lý).
    public List<TransactionHistory> GetTransactionHistory(string stk)
    {
        using OracleConnection conn = Database.GetConnection();
        string sql = @"SELECT gd.SOTK, gd.SOTK_DOI, gd.LOAIGD, gd.SOTIEN, sd.SODUSAUGD, gd.THOIGIANGD
                        FROM GIAO_DICH gd
                        JOIN SO_DU_SAU_GD sd ON sd.MAGD = gd.MAGD AND sd.SOTK = :stk1
                        WHERE gd.SOTK = :stk2 OR gd.SOTK_DOI = :stk3
                        ORDER BY gd.THOIGIANGD DESC";

        return conn.Query<TransactionHistory>(sql, new { stk1 = stk, stk2 = stk, stk3 = stk }).ToList();
    }
}
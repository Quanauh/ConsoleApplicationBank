using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;

class AccountDAO
{
     public bool TonTai(string stk)
    {
        using OracleConnection conn = Database.GetConnection();

        string sql = "SELECT COUNT(*) FROM TAI_KHOAN WHERE SoTK=:stk";

        OracleCommand cmd = new OracleCommand(sql, conn);
        cmd.Parameters.Add(":stk", stk);

        int dem = Convert.ToInt32(cmd.ExecuteScalar());

        return dem > 0;
    }
    public void TaoTk(string ten, string sdt,string email,string ngaysinh,string diachi,string stk, string mk)
    {
        using OracleConnection conn=Database.GetConnection();
        using OracleTransaction trann=conn.BeginTransaction();
        try{
        string sql="INSERT INTO khach_hang(HOTEN,SDT,EMAIL,NGAYSINH,DIACHI) VALUES (:ten,:sdt,:email,TO_DATE(:ngaysinh,'dd/MM/YYYY'),:diachi)";
        using OracleCommand cmdkh=new OracleCommand(sql, conn);
        cmdkh.Transaction = trann;
            cmdkh.Parameters.Add(":ten",ten);
            cmdkh.Parameters.Add(":sdt",sdt);
            cmdkh.Parameters.Add(":email",email);
            cmdkh.Parameters.Add(":ngaysinh",ngaysinh);
            cmdkh.Parameters.Add(":diachi",diachi);
            cmdkh.ExecuteNonQuery();
        string sqll="INSERT INTO tai_khoan (SOTK,MAKH,MATKHAU) VALUES (:stk,seq_khach_hang.CURRVAL,:mk)";
        using OracleCommand cmdtk=new OracleCommand(sqll,conn);
        cmdtk.Transaction=trann;
        cmdtk.Parameters.Add(":stk",stk);
        cmdtk.Parameters.Add(":mk",mk);
        cmdtk.ExecuteNonQuery();
        trann.Commit();
        }
        catch
        {
            trann.Rollback();
            throw ;
        }
    }
    public bool checkMk(string stk, string mk)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql="SELECT COUNT(*) FROM tai_khoan WHERE sotk=:stk AND matkhau=:mk";
        using OracleCommand cmd =new OracleCommand(sql,conn);
        cmd.Parameters.Add(":stk",stk);
        cmd.Parameters.Add(":mk",mk);
        int dem=Convert.ToInt32(cmd.ExecuteScalar());
        return dem>0;
    }
    public void Naptien(string stk, decimal a)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql="UPDATE tai_khoan SET SODU=SODU+:a WHERE sotk=:stk";
        using OracleCommand cmd=new OracleCommand(sql,conn);
        cmd.Parameters.Add(":a",a);
        cmd.Parameters.Add(":stk",stk);
        cmd.ExecuteNonQuery();
    }
    public decimal checksd(string stk)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql="SELECT sodu FROM tai_khoan WHERE SOTK=:stk";
        using OracleCommand cmd=new OracleCommand(sql,conn);
        cmd.Parameters.Add(":stk",stk);
        decimal sodu=Convert.ToDecimal(cmd.ExecuteScalar());
        return sodu;
    }
    public void RutTien(string stk,decimal a)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql="UPDATE tai_khoan SET SODU=SODU-:a WHERE sotk=:stk";
        using OracleCommand cmd=new OracleCommand(sql,conn);
        cmd.Parameters.Add(":a",a);
        cmd.Parameters.Add(":stk",stk);
        cmd.ExecuteNonQuery();
    }
    public ThongTinTaiKhoan? LayThongTin(string stk)
{
    using OracleConnection conn = Database.GetConnection();

    string sql = @"SELECT tk.SOTK,kh.HOTEN,kh.SDT,kh.EMAIL,kh.NGAYSINH,kh.DIACHI,tk.SODU
                   FROM TAI_KHOAN tk
                   JOIN KHACH_HANG kh
                        ON tk.MAKH = kh.MAKH
                   WHERE tk.SOTK = :stk";

    using OracleCommand cmd = new OracleCommand(sql, conn);
    cmd.Parameters.Add(":stk", stk);

    using OracleDataReader reader = cmd.ExecuteReader();

    if (!reader.Read())
        return null;

    return new ThongTinTaiKhoan(
        // reader["SOTK"].ToString()??"",
        // reader["HOTEN"].ToString()??"",
        // reader["SDT"].ToString()??"",
        // reader["EMAIL"].ToString()??"",
        // Convert.ToDateTime(reader["NGAYSINH"]),
        // reader["DIACHI"].ToString()??"",
        // Convert.ToDecimal(reader["SODU"])
        reader["SOTK"].ToString()??"",
        reader.IsDBNull(reader.GetOrdinal("HoTen")) ? "" : reader.GetString(reader.GetOrdinal("HoTen")),
        reader.IsDBNull(reader.GetOrdinal("SDT")) ? "" : reader.GetString(reader.GetOrdinal("SDT")),
        reader.IsDBNull(reader.GetOrdinal("Email")) ? "" : reader.GetString(reader.GetOrdinal("Email")),
        reader.IsDBNull(reader.GetOrdinal("NgaySinh")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("NgaySinh")),
        reader.IsDBNull(reader.GetOrdinal("DiaChi")) ? "" : reader.GetString(reader.GetOrdinal("DiaChi")),
        Convert.ToDecimal(reader["SODU"])
    );

}   
    public void GhiLichSu(string stk1,string? stk2,string loai,decimal a)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql="";
        if (loai == "CHUYEN" || loai == "RUT"){
        sql=@"INSERT INTO GIAO_DICH(SOTK,SOTK_DOI,LOAIGD,SOTIEN,SODUSAUGD) 
                    SELECT :stk1,
                    :stk2,
                    :loai,
                    :tien,
                    SODU
                    FROM TAI_KHOAN
                    WHERE SOTK=:stk1"
                    ;
        }
        else{
        sql=@"INSERT INTO GIAO_DICH(SOTK,SOTK_DOI,LOAIGD,SOTIEN,SODUSAUGD) 
                    SELECT :stk1,
                    :stk2,
                    :loai,
                    :tien,
                    SODU
                    FROM TAI_KHOAN
                    WHERE SOTK=:stk1"
                    ;
        }
        using OracleCommand cmd=new OracleCommand(sql,conn);
        cmd.Parameters.Add(":stk1",stk1);
        cmd.Parameters.Add("stk2",stk2==null? DBNull.Value:stk2);
        cmd.Parameters.Add("loai",loai);
        cmd.Parameters.Add(":tien",a);
        cmd.ExecuteNonQuery();
    }
    public List<LichSuGiaoDich> LayLichSu(string stk)
    {
        using OracleConnection conn=Database.GetConnection();
        string sql=@"SELECT tk.SOTK,gd.SOTK_DOI,gd.LOAIGD,gd.SOTIEN,gd.SODUSAUGD,gd.THOIGIANGD
                        FROM GIAO_DICH gd
                        JOIN TAI_KHOAN tk
                        ON gd.SOTK=tk.SOTK
                        WHERE gd.SOTK=:stk";
        using OracleCommand cmd=new OracleCommand(sql,conn);
        cmd.Parameters.Add(":stk",stk);
        using OracleDataReader reader=cmd.ExecuteReader();
        List<LichSuGiaoDich> ds=new List<LichSuGiaoDich>();
        while (reader.Read())
        {
            ds.Add(new LichSuGiaoDich(
                reader["SOTK"].ToString()??"",
                reader.IsDBNull(reader.GetOrdinal("SOTK_DOI")) ? "" : reader.GetString(reader.GetOrdinal("SOTK_DOI")),
                reader["LOAIGD"].ToString()??"",
                Convert.ToDecimal(reader["SOTIEN"]),
                Convert.ToDecimal(reader["SODUSAUGD"]),
                reader.GetDateTime(reader.GetOrdinal("THOIGIANGD"))
            ));
        }
        return ds;
    }
}

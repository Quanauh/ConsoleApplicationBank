using System;
using System.IO;
 
//Ghi log ra file - mỗi lần chạy chương trình tạo 1 file log mới,
//đặt tên theo thời điểm khởi động
static class Logger
{
    private static string duongDanFile = "";
 
    //Khởi tạo log
    public static void KhoiTao()
    {
        string thuMuc = "logs";
        if (!Directory.Exists(thuMuc))
            Directory.CreateDirectory(thuMuc);
 
        string tenFile = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        duongDanFile = Path.Combine(thuMuc, tenFile);
    }
 
    //Ghi 1 dòng log. chiTietLoi chỉ cần truyền khi capDo là ERROR (vd: ex.ToString())
    public static void Ghi(string capDo, string hanhDong, string noiDung, string? chiTietLoi = null)
    {
        string dong = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{capDo}] {hanhDong}: {noiDung}";
        if (chiTietLoi != null)
            dong += Environment.NewLine + "    ChiTiet: " + chiTietLoi;
 
        File.AppendAllText(duongDanFile, dong + Environment.NewLine);
    }
}
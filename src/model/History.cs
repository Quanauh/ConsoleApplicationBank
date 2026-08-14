using System;
 
class TransactionHistory
{
    public string SoTK { get; set; } = "";
    public string? SoTK_Doi { get; set; }
    public string LoaiGD { get; set; } = "";
    public decimal SoTien { get; set; }
    public decimal SoDuSauGD { get; set; }
    public DateTime ThoiGianGD { get; set; }
    public string Chieu { get; set; } = "";
 
    public override string ToString()
    {
        return $"[{ThoiGianGD:dd/MM/yyyy HH:mm:ss}] {Chieu} - So tien: {SoTien} - So du sau GD: {SoDuSauGD}"
             + (SoTK_Doi != null ? $" - Doi tac: {SoTK_Doi}" : "");
    }
}
 
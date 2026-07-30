class LichSuGiaoDich
{
    public string SoTK;
    public string? SoTK_Doi;
    public string LoaiGD;
    public decimal SoTien;
    public decimal Sodusaugd;
    public DateTime Thoigiangd;
    public LichSuGiaoDich(string stk,string? stkd,string loaigd,decimal st,decimal sotiensau,DateTime Thoigian)
    {
        SoTK=stk;
        SoTK_Doi=stkd;
        LoaiGD=loaigd;
        SoTien=st;
        Sodusaugd=sotiensau;
        Thoigiangd=Thoigian;
    }
    public override string ToString()
    {
        return SoTK+" "+LoaiGD+" "+ SoTK_Doi+" So tien:"+SoTien+" So du:"+Sodusaugd+" Thoigian"+Thoigiangd;
    }

}
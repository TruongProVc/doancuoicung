//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BanGiay.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ChiTietDonHang
    {
        public int maChiTietDonHang { get; set; }
        public int maDonHang { get; set; }
        public int maSanPham { get; set; }
        public int soLuong { get; set; }
        public double giaTien { get; set; }
        public int Size { get; set; }
    
        public virtual DonDatHang DonDatHang { get; set; }
        public virtual SanPham SanPham { get; set; }
    }
}

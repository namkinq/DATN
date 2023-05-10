﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace WebBanHang.Models
{
    public partial class LoaiSanPham
    {
        public LoaiSanPham()
        {
            SanPhams = new HashSet<SanPham>();
        }

        public int MaLoai { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string TenLoai { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả")]
        public string MoTa { get; set; }

        public virtual ICollection<SanPham> SanPhams { get; set; }
    }
}

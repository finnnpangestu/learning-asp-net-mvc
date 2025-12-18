using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebCoba.Models {
    public class Product {
        [Key]
        public int Id {  get; set; }

        [Required(ErrorMessage = "Nama Barang tidak boleh kosong!")]
        [StringLength(100, ErrorMessage = "Nama maksimal 100 karakter!")]
        [Display(Name = "Nama Produk")]
        public string Name { get; set; }

        [Column(TypeName = "decimal")]
        [Required(ErrorMessage = "Harga wajib diisi!")]
        [Range(0.01, 999999.99, ErrorMessage = "Harga harus antara 0.01 sampai 999,999.99!")]
        [Display(Name = "Harga ($)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stok wajib diisi!")]
        [Range(0, 99999, ErrorMessage = "Stok minimal 0")]
        public int Stock { get; set; }
    }
}
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace WebBanHang.Models
{
    public partial class dbBanHangContext : DbContext
    {
        public dbBanHangContext()
        {
        }

        public dbBanHangContext(DbContextOptions<dbBanHangContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ChiTietDonHang> ChiTietDonHangs { get; set; }
        public virtual DbSet<DonHang> DonHangs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<KhuyenMai> KhuyenMais { get; set; }
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public virtual DbSet<QuanTriVien> QuanTriViens { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<ThuongHieu> ThuongHieus { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=LAPTOP-73R0Q1AI\\SQLEXPRESS01;Initial Catalog=dbBanHang;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<ChiTietDonHang>(entity =>
            {
                entity.HasKey(e => new { e.MaDh, e.MaSp });

                entity.ToTable("ChiTietDonHang");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.MaSp).HasColumnName("MaSP");

                entity.HasOne(d => d.MaDhNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaDh)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietDonHang_DonHang");

                entity.HasOne(d => d.MaSpNavigation)
                    .WithMany(p => p.ChiTietDonHangs)
                    .HasForeignKey(d => d.MaSp)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChiTietDonHang_SanPham");
            });

            modelBuilder.Entity<DonHang>(entity =>
            {
                entity.HasKey(e => e.MaDh);

                entity.ToTable("DonHang");

                entity.Property(e => e.MaDh).HasColumnName("MaDH");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.HoTen).HasMaxLength(250);

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.NgayDat).HasColumnType("datetime");

                entity.Property(e => e.NgayShip).HasColumnType("datetime");

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TrangThai).HasMaxLength(100);

                entity.HasOne(d => d.MaKhNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaKh)
                    .HasConstraintName("FK_DonHang_KhachHang");

                entity.HasOne(d => d.MaShipperNavigation)
                    .WithMany(p => p.DonHangs)
                    .HasForeignKey(d => d.MaShipper)
                    .HasConstraintName("FK_DonHang_Shipper");
            });

            modelBuilder.Entity<KhachHang>(entity =>
            {
                entity.HasKey(e => e.MaKh);

                entity.ToTable("KhachHang");

                entity.Property(e => e.MaKh).HasColumnName("MaKH");

                entity.Property(e => e.DiaChi).HasMaxLength(500);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenKh)
                    .HasMaxLength(250)
                    .HasColumnName("TenKH");
            });

            modelBuilder.Entity<KhuyenMai>(entity =>
            {
                entity.HasKey(e => e.MaCtkm);

                entity.ToTable("KhuyenMai");

                entity.Property(e => e.MaCtkm).HasColumnName("MaCTKM");

                entity.Property(e => e.Gtgiam).HasColumnName("GTGiam");

                entity.Property(e => e.GttoiThieu).HasColumnName("GTToiThieu");

                entity.Property(e => e.LoaiKm)
                    .HasMaxLength(50)
                    .HasColumnName("LoaiKM");

                entity.Property(e => e.Ma)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.NgayBd)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayBD");

                entity.Property(e => e.NgayKt)
                    .HasColumnType("datetime")
                    .HasColumnName("NgayKT");
            });

            modelBuilder.Entity<LoaiSanPham>(entity =>
            {
                entity.HasKey(e => e.MaLoai)
                    .HasName("PK_LOAISANPHAM");

                entity.ToTable("LoaiSanPham");

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.TenLoai).HasMaxLength(250);
            });

            modelBuilder.Entity<QuanTriVien>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("QuanTriVien");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.MaQtv)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("MaQTV");

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenQtv)
                    .HasMaxLength(250)
                    .HasColumnName("TenQTV");
            });

            modelBuilder.Entity<SanPham>(entity =>
            {
                entity.HasKey(e => e.MaSp);

                entity.ToTable("SanPham");

                entity.Property(e => e.MaSp).HasColumnName("MaSP");

                entity.Property(e => e.Anh).HasMaxLength(250);

                entity.Property(e => e.BaoHanh).HasMaxLength(50);

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.MoTa).HasMaxLength(500);

                entity.Property(e => e.TenSp)
                    .HasMaxLength(250)
                    .HasColumnName("TenSP");

                entity.HasOne(d => d.MaLoaiNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaLoai)
                    .HasConstraintName("FK_SanPham_LoaiSanPham");

                entity.HasOne(d => d.MaThNavigation)
                    .WithMany(p => p.SanPhams)
                    .HasForeignKey(d => d.MaTh)
                    .HasConstraintName("FK_SanPham_ThuongHieu");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.HasKey(e => e.MaShipper);

                entity.ToTable("Shipper");

                entity.Property(e => e.BienSo).HasMaxLength(12);

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.LoaiXe).HasMaxLength(50);

                entity.Property(e => e.MatKhau).HasMaxLength(50);

                entity.Property(e => e.Salt).HasMaxLength(50);

                entity.Property(e => e.Sdt)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("SDT")
                    .IsFixedLength(true);

                entity.Property(e => e.TenShipper).HasMaxLength(250);
            });

            modelBuilder.Entity<ThuongHieu>(entity =>
            {
                entity.HasKey(e => e.MaTh);

                entity.ToTable("ThuongHieu");

                entity.Property(e => e.MaTh).HasColumnName("MaTH");

                entity.Property(e => e.MoTa).HasMaxLength(250);

                entity.Property(e => e.TenTh)
                    .HasMaxLength(250)
                    .HasColumnName("TenTH");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyBanDoGomSu.Domain.Entities
{
    [Table("TinNhanChat")]
    public class TinNhanChat
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id", TypeName = "BIGINT")]
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        [Column("SessionId", TypeName = "VARCHAR(50)")]
        public string SessionId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("SenderId", TypeName = "NVARCHAR(50)")]
        public string SenderId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("SenderName", TypeName = "NVARCHAR(100)")]
        public string SenderName { get; set; }

        [Required]
        [Column("Content", TypeName = "NVARCHAR(MAX)")]
        public string Content { get; set; }

        [Column("IsNhanVien", TypeName = "BIT")]
        public bool IsNhanVien { get; set; }

        [Column("ThoiGian", TypeName = "DATETIME2")]
        public DateTime ThoiGian { get; set; } = DateTime.Now;
    }
}

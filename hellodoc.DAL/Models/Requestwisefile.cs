using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("requestwisefile")]
public partial class Requestwisefile
{
    [Key]
    [Column("requestwisefileid")]
    public int Requestwisefileid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("filename")]
    [StringLength(500)]
    public string? Filename { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }

    [Column("doctype")]
    public short? Doctype { get; set; }

    [Column("isfrontside", TypeName = "bit(1)")]
    public BitArray? Isfrontside { get; set; }

    [Column("iscompensation", TypeName = "bit(1)")]
    public BitArray? Iscompensation { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("isfinalize", TypeName = "bit(1)")]
    public BitArray? Isfinalize { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("ispatientrecords", TypeName = "bit(1)")]
    public BitArray? Ispatientrecords { get; set; }

    [ForeignKey("Adminid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Admin? Admin { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Physician? Physician { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Requestwisefiles")]
    public virtual Request Request { get; set; } = null!;
}

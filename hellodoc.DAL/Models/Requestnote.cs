using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("requestnotes")]
public partial class Requestnote
{
    [Key]
    [Column("requestnotesid")]
    public int Requestnotesid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("strmonth")]
    [StringLength(20)]
    public string? Strmonth { get; set; }

    [Column("intyear")]
    public int? Intyear { get; set; }

    [Column("intdate")]
    public int? Intdate { get; set; }

    [Column("physiciannotes")]
    [StringLength(500)]
    public string? Physiciannotes { get; set; }

    [Column("adminnotes")]
    [StringLength(500)]
    public string? Adminnotes { get; set; }

    [Column("createdby")]
    [StringLength(128)]
    public string Createdby { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("administrativenotes")]
    [StringLength(500)]
    public string? Administrativenotes { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Requestnotes")]
    public virtual Request Request { get; set; } = null!;
}

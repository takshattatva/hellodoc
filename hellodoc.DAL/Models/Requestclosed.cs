using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("requestclosed")]
public partial class Requestclosed
{
    [Key]
    [Column("requestclosedid")]
    public int Requestclosedid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("requeststatuslogid")]
    public int Requeststatuslogid { get; set; }

    [Column("phynotes")]
    [StringLength(500)]
    public string? Phynotes { get; set; }

    [Column("clientnotes")]
    [StringLength(500)]
    public string? Clientnotes { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Requestcloseds")]
    public virtual Request Request { get; set; } = null!;

    [ForeignKey("Requeststatuslogid")]
    [InverseProperty("Requestcloseds")]
    public virtual Requeststatuslog Requeststatuslog { get; set; } = null!;
}

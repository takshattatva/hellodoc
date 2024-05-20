using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("requestconcierge")]
public partial class Requestconcierge
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("conciergeid")]
    public int Conciergeid { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [ForeignKey("Conciergeid")]
    [InverseProperty("Requestconcierges")]
    public virtual Concierge Concierge { get; set; } = null!;

    [ForeignKey("Requestid")]
    [InverseProperty("Requestconcierges")]
    public virtual Request Request { get; set; } = null!;
}

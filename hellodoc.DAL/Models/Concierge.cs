using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("concierge")]
public partial class Concierge
{
    [Key]
    [Column("conciergeid")]
    public int Conciergeid { get; set; }

    [Column("conciergename")]
    [StringLength(100)]
    public string Conciergename { get; set; } = null!;

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("street")]
    [StringLength(50)]
    public string Street { get; set; } = null!;

    [Column("city")]
    [StringLength(50)]
    public string City { get; set; } = null!;

    [Column("state")]
    [StringLength(50)]
    public string State { get; set; } = null!;

    [Column("zipcode")]
    [StringLength(50)]
    public string Zipcode { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("roleid")]
    [StringLength(20)]
    public string? Roleid { get; set; }

    [ForeignKey("Regionid")]
    [InverseProperty("Concierges")]
    public virtual Region? Region { get; set; }

    [InverseProperty("Concierge")]
    public virtual ICollection<Requestconcierge> Requestconcierges { get; set; } = new List<Requestconcierge>();
}

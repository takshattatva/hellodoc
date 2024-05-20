using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("shiftdetail")]
public partial class Shiftdetail
{
    [Key]
    [Column("shiftdetailid")]
    public int Shiftdetailid { get; set; }

    [Column("shiftid")]
    public int Shiftid { get; set; }

    [Column("shiftdate", TypeName = "timestamp without time zone")]
    public DateTime Shiftdate { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("starttime")]
    public TimeOnly Starttime { get; set; }

    [Column("endtime")]
    public TimeOnly Endtime { get; set; }

    [Column("status")]
    public short Status { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray Isdeleted { get; set; } = null!;

    [Column("modifiedby")]
    [StringLength(128)]
    public string? Modifiedby { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("lastrunningdate", TypeName = "timestamp without time zone")]
    public DateTime? Lastrunningdate { get; set; }

    [Column("eventid")]
    [StringLength(100)]
    public string? Eventid { get; set; }

    [Column("issync", TypeName = "bit(1)")]
    public BitArray? Issync { get; set; }

    [ForeignKey("Modifiedby")]
    [InverseProperty("Shiftdetails")]
    public virtual Aspnetuser? ModifiedbyNavigation { get; set; }

    [ForeignKey("Shiftid")]
    [InverseProperty("Shiftdetails")]
    public virtual Shift Shift { get; set; } = null!;

    [InverseProperty("Shiftdetail")]
    public virtual ICollection<Shiftdetailregion> Shiftdetailregions { get; set; } = new List<Shiftdetailregion>();
}

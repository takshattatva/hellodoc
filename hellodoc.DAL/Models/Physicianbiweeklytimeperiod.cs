using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("physicianbiweeklytimeperiod")]
public partial class Physicianbiweeklytimeperiod
{
    [Key]
    [Column("timeperiodid")]
    public int Timeperiodid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("startdate")]
    public DateOnly? Startdate { get; set; }

    [Column("enddate")]
    public DateOnly? Enddate { get; set; }

    [Column("status")]
    public short? Status { get; set; }

    [Column("isfinalize")]
    public bool? Isfinalize { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Physicianbiweeklytimeperiods")]
    public virtual Physician Physician { get; set; } = null!;

    [InverseProperty("Timeperiod")]
    public virtual ICollection<Physiciantimesheet> Physiciantimesheets { get; set; } = new List<Physiciantimesheet>();
}

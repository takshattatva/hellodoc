using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("physiciantimesheet")]
public partial class Physiciantimesheet
{
    [Key]
    [Column("timesheetdateid")]
    public int Timesheetdateid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("timeperiodid")]
    public int Timeperiodid { get; set; }

    [Column("timesheetdate")]
    public DateOnly? Timesheetdate { get; set; }

    [Column("shift")]
    public int? Shift { get; set; }

    [Column("nightshiftweekend")]
    public int? Nightshiftweekend { get; set; }

    [Column("housecalls")]
    public int? Housecalls { get; set; }

    [Column("housecallnightsweekend")]
    public int? Housecallnightsweekend { get; set; }

    [Column("phoneconsults")]
    public int? Phoneconsults { get; set; }

    [Column("phoneconsultsnightsweekend")]
    public int? Phoneconsultsnightsweekend { get; set; }

    [Column("batchtesting")]
    public int? Batchtesting { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Physiciantimesheets")]
    public virtual Physician Physician { get; set; } = null!;

    [ForeignKey("Timeperiodid")]
    [InverseProperty("Physiciantimesheets")]
    public virtual Physicianbiweeklytimeperiod Timeperiod { get; set; } = null!;
}

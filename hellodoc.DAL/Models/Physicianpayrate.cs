using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("physicianpayrate")]
public partial class Physicianpayrate
{
    [Key]
    [Column("payrateid")]
    public int Payrateid { get; set; }

    [Column("physicianid")]
    public int Physicianid { get; set; }

    [Column("nightshiftweekend")]
    public int? Nightshiftweekend { get; set; }

    [Column("shift")]
    public int? Shift { get; set; }

    [Column("housecallnightsweekend")]
    public int? Housecallnightsweekend { get; set; }

    [Column("phoneconsults")]
    public int? Phoneconsults { get; set; }

    [Column("phoneconsultsnightsweekend")]
    public int? Phoneconsultsnightsweekend { get; set; }

    [Column("batchtesting")]
    public int? Batchtesting { get; set; }

    [Column("housecalls")]
    public int? Housecalls { get; set; }

    [ForeignKey("Physicianid")]
    [InverseProperty("Physicianpayrates")]
    public virtual Physician Physician { get; set; } = null!;
}

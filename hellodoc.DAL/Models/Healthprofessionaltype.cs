using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("healthprofessionaltype")]
public partial class Healthprofessionaltype
{
    [Key]
    [Column("healthprofessionalid")]
    public int Healthprofessionalid { get; set; }

    [Column("professionname")]
    [StringLength(50)]
    public string Professionname { get; set; } = null!;

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("isactive", TypeName = "bit(1)")]
    public BitArray? Isactive { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [InverseProperty("ProfessionNavigation")]
    public virtual ICollection<Healthprofessional> Healthprofessionals { get; set; } = new List<Healthprofessional>();
}

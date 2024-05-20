using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("healthprofessionals")]
public partial class Healthprofessional
{
    [Key]
    [Column("vendorid")]
    public int Vendorid { get; set; }

    [Column("vendorname")]
    [StringLength(100)]
    public string Vendorname { get; set; } = null!;

    [Column("profession")]
    public int? Profession { get; set; }

    [Column("faxnumber")]
    [StringLength(50)]
    public string Faxnumber { get; set; } = null!;

    [Column("address")]
    [StringLength(150)]
    public string? Address { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(50)]
    public string? State { get; set; }

    [Column("zip")]
    [StringLength(50)]
    public string? Zip { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("createddate", TypeName = "timestamp without time zone")]
    public DateTime Createddate { get; set; }

    [Column("modifieddate", TypeName = "timestamp without time zone")]
    public DateTime? Modifieddate { get; set; }

    [Column("phonenumber")]
    [StringLength(100)]
    public string? Phonenumber { get; set; }

    [Column("isdeleted", TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("businesscontact")]
    [StringLength(100)]
    public string? Businesscontact { get; set; }

    [ForeignKey("Profession")]
    [InverseProperty("Healthprofessionals")]
    public virtual Healthprofessionaltype? ProfessionNavigation { get; set; }
}

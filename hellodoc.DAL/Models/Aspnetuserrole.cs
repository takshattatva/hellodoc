using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("aspnetuserroles")]
public partial class Aspnetuserrole
{
    [Key]
    [Column("userid")]
    [StringLength(128)]
    public string Userid { get; set; } = null!;

    [Column("roleid")]
    [StringLength(128)]
    public string? Roleid { get; set; }

    [ForeignKey("Roleid")]
    [InverseProperty("Aspnetuserroles")]
    public virtual Aspnetrole? Role { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Aspnetuserrole")]
    public virtual Aspnetuser User { get; set; } = null!;
}

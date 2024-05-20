using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("menu")]
public partial class Menu
{
    [Key]
    [Column("menuid")]
    public int Menuid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("accounttype")]
    public short Accounttype { get; set; }

    [Column("sortorder")]
    public int? Sortorder { get; set; }

    [InverseProperty("Menu")]
    public virtual ICollection<Rolemenu> Rolemenus { get; set; } = new List<Rolemenu>();
}

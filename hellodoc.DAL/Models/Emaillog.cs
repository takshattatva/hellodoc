using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("emaillog")]
public partial class Emaillog
{
    [Key]
    [Column("emaillogid")]
    public int Emaillogid { get; set; }

    [Column("emailtemplate", TypeName = "character varying")]
    public string? Emailtemplate { get; set; }

    [Column("subjectname")]
    [StringLength(200)]
    public string Subjectname { get; set; } = null!;

    [Column("emailid")]
    [StringLength(200)]
    public string Emailid { get; set; } = null!;

    [Column("confirmationnumber")]
    [StringLength(200)]
    public string? Confirmationnumber { get; set; }

    [Column("filepath", TypeName = "character varying")]
    public string? Filepath { get; set; }

    [Column("roleid")]
    public int? Roleid { get; set; }

    [Column("requestid")]
    public int? Requestid { get; set; }

    [Column("physicianid")]
    public int? Physicianid { get; set; }

    [Column("createdate", TypeName = "timestamp without time zone")]
    public DateTime Createdate { get; set; }

    [Column("sentdate", TypeName = "timestamp without time zone")]
    public DateTime? Sentdate { get; set; }

    [Column("isemailsent", TypeName = "bit(1)")]
    public BitArray? Isemailsent { get; set; }

    [Column("senttries")]
    public int? Senttries { get; set; }

    [Column("action")]
    public int? Action { get; set; }

    [Column("adminid")]
    public int? Adminid { get; set; }
}

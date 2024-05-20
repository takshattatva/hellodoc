using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("requestclient")]
public partial class Requestclient
{
    [Key]
    [Column("requestclientid")]
    public int Requestclientid { get; set; }

    [Column("requestid")]
    public int Requestid { get; set; }

    [Column("firstname")]
    [StringLength(100)]
    public string Firstname { get; set; } = null!;

    [Column("lastname")]
    [StringLength(100)]
    public string? Lastname { get; set; }

    [Column("phonenumber")]
    [StringLength(23)]
    public string? Phonenumber { get; set; }

    [Column("location")]
    [StringLength(100)]
    public string? Location { get; set; }

    [Column("address")]
    [StringLength(500)]
    public string? Address { get; set; }

    [Column("regionid")]
    public int? Regionid { get; set; }

    [Column("notimobile")]
    [StringLength(20)]
    public string? Notimobile { get; set; }

    [Column("notiemail")]
    [StringLength(50)]
    public string? Notiemail { get; set; }

    [Column("notes")]
    [StringLength(500)]
    public string? Notes { get; set; }

    [Column("email")]
    [StringLength(50)]
    public string? Email { get; set; }

    [Column("strmonth")]
    [StringLength(20)]
    public string? Strmonth { get; set; }

    [Column("intyear")]
    public int? Intyear { get; set; }

    [Column("intdate")]
    public int? Intdate { get; set; }

    [Column("ismobile", TypeName = "bit(1)")]
    public BitArray? Ismobile { get; set; }

    [Column("street")]
    [StringLength(100)]
    public string? Street { get; set; }

    [Column("city")]
    [StringLength(100)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(100)]
    public string? State { get; set; }

    [Column("zipcode")]
    [StringLength(10)]
    public string? Zipcode { get; set; }

    [Column("communicationtype")]
    public short? Communicationtype { get; set; }

    [Column("remindreservationcount")]
    public short? Remindreservationcount { get; set; }

    [Column("remindhousecallcount")]
    public short? Remindhousecallcount { get; set; }

    [Column("issetfollowupsent")]
    public short? Issetfollowupsent { get; set; }

    [Column("ip")]
    [StringLength(20)]
    public string? Ip { get; set; }

    [Column("isreservationremindersent")]
    public short? Isreservationremindersent { get; set; }

    [Column("latitude")]
    [Precision(9, 6)]
    public decimal? Latitude { get; set; }

    [Column("longitude")]
    [Precision(9, 6)]
    public decimal? Longitude { get; set; }

    [ForeignKey("Regionid")]
    [InverseProperty("Requestclients")]
    public virtual Region? Region { get; set; }

    [ForeignKey("Requestid")]
    [InverseProperty("Requestclients")]
    public virtual Request Request { get; set; } = null!;
}

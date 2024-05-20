using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("MessagesTable")]
public partial class MessagesTable
{
    [Key]
    public int MessageId { get; set; }

    [Column(TypeName = "character varying")]
    public string Message { get; set; } = null!;

    [StringLength(128)]
    public string Sender { get; set; } = null!;

    [StringLength(128)]
    public string Reciever { get; set; } = null!;

    [Column("SentTIme", TypeName = "timestamp without time zone")]
    public DateTime SentTime { get; set; }
}

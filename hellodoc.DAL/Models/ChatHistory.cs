using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("ChatHistory")]
public partial class ChatHistory
{
    [Key]
    public int Id { get; set; }

    public int? RequestId { get; set; }

    [StringLength(255)]
    public string? SenderAspId { get; set; }

    [StringLength(255)]
    public string? ReceiverAspId { get; set; }

    public TimeOnly? Time { get; set; }

    [StringLength(255)]
    public string? Message { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? CreatedDate { get; set; }

    [ForeignKey("RequestId")]
    [InverseProperty("ChatHistories")]
    public virtual Request? Request { get; set; }
}

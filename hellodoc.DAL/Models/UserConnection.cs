using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hellodoc.DAL.Models;

[Table("UserConnection")]
public partial class UserConnection
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string? ConnectionId { get; set; }

    [StringLength(255)]
    public string? UserId { get; set; }

    [StringLength(255)]
    public string? RequestId { get; set; }
}

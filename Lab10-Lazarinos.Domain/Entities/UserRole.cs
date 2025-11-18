using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lab10_Lazarinos.Domain.Entities;

public partial class UserRole
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? AssignedAt { get; set; }

    [JsonIgnore]
    public virtual Role Role { get; set; } = null!;

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}

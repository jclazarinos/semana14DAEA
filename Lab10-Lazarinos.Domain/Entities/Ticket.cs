using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lab10_Lazarinos.Domain.Entities;

public partial class Ticket
{
    public Guid TicketId { get; set; }

    public Guid UserId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string Status { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    [JsonIgnore]
    public virtual ICollection<Response> Responses { get; set; } = new List<Response>();

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}

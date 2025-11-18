using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Lab10_Lazarinos.Domain.Entities;

public partial class Response
{
    public Guid ResponseId { get; set; }

    public Guid TicketId { get; set; }

    public Guid ResponderId { get; set; }

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual User Responder { get; set; } = null!;

    [JsonIgnore]
    public virtual Ticket Ticket { get; set; } = null!;
}

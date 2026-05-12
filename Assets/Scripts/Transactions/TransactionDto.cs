using System;

public class TransactionDto
{
    public decimal? Amount { get; set; }

    public string? Description { get; set; }

    public DateTimeOffset? Timestamp { get; set; }

    public TransactionType Type { get; set; }
}

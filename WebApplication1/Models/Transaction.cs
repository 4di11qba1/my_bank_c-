using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public decimal? AccountId { get; set; }

    public decimal? Amount { get; set; }

    public string? Type { get; set; }

    public DateTime? TimeStamp { get; set; }

    public decimal? SenderId { get; set; }

    public decimal? RecieverId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Account? Reciever { get; set; }

    public virtual Account? Sender { get; set; }
}

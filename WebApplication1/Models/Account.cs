using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Account
{
    public decimal Id { get; set; }

    public decimal? Balance { get; set; }

    public string? Type { get; set; }

    public int? UserId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

    public virtual ICollection<Transaction> TransactionAccounts { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionRecievers { get; set; } = new List<Transaction>();

    public virtual ICollection<Transaction> TransactionSenders { get; set; } = new List<Transaction>();

    public virtual User? User { get; set; }
}

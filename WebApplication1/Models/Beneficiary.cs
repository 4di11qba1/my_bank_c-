using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Beneficiary
{
    public int Id { get; set; }

    public decimal? AccountId { get; set; }

    public int? UserId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual User? User { get; set; }
}

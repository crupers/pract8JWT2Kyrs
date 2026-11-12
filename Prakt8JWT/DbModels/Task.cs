using System;
using System.Collections.Generic;

namespace Prakt8JWT.DbModels;

public partial class Task
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string OwnerLogin { get; set; } = null!;

    public bool? IsCompleted { get; set; }

    public virtual User OwnerLoginNavigation { get; set; } = null!;
}

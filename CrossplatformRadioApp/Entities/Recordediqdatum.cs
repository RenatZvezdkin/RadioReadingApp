using System;
using System.Collections.Generic;

namespace CrossplatformRadioApp.MainDatabase;

public partial class Recordediqdatum
{
    public int Id { get; set; }

    public int RecordId { get; set; }

    public int I { get; set; }

    public int Q { get; set; }

    public DateTime DatetimeOfRecord { get; set; }

    public virtual Record Record { get; set; } = null!;
}

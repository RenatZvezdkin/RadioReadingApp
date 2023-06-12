using System;
using System.Collections.Generic;

namespace CrossplatformRadioApp.MainDatabase;

public partial class Record
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public DateTime DateOfRecord { get; set; }

    public virtual ICollection<Recordediqdatum> Recordediqdata { get; set; } = new List<Recordediqdatum>();
}

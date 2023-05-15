using System;
using System.Collections.Generic;

namespace CrossplatformRadioApp.Entities;

public partial class SavedFile
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string Format { get; set; } = null!;

    public byte[] ByteCode { get; set; } = null!;

    public DateTime DateOfSaving { get; set; }
}

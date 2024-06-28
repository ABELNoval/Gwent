using System;
using System.Reflection.Emit;

public class DropDownItem
{
    public Guid id { get; private set; }
    public string label { get; private set; }
    public DropDownItem(Guid id, string label)
    {
        this.id = id;
        this.label = label;
    }
}

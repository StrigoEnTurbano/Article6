using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class EntryPoint : Godot.Node
{
    private bool Initialized = false;

    private static void Initialize()
    {
        Minor.Initialize();
    }

    public override void _Ready()
    {
        if (!Initialized)
        {
            Initialized = true;
            Initialize();
        }
    }
}
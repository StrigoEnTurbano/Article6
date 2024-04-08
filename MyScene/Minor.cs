using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class Minor : GodotFSharp.Core.MyScene.Minor
{
    public static void Initialize()
    {
        GodotFSharp.Core.MyScene.Minor.Factory = () => new Minor();
    }

    public override void _Draw() => base._Draw();

    public override void _Process(double delta) => base._Process(delta);
}
using Godot;
using Godot.NativeInterop;
using System;

public partial class MyScene : GodotFSharp.Core.MyScene.Main
{
	public override void _Ready() => base._Ready();

	public override void _Process(double delta) => base._Process(delta);

	[Export]
	public new int Radius
	{
		get => base.Radius;
		set => base.Radius = value;
	}

	public new void _on_click_canvas_gui_input (InputEvent ev) => base._on_click_canvas_gui_input(ev);
}
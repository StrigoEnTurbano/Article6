module GodotFSharp.Core.MyScene

open Godot

type [<AbstractClass>] Minor () =
    inherit Node2D()

    let color = Color.FromHsv(GD.Randf(), 0.9f, 0.95f)

    do  base.Name <- $"Minor #%s{color.ToHtml false}"

    let mutable health = 1f

    member val Radius = 32f with get, set

    override this._Process delta =
        health <- health - float32 delta * 0.3f
        if health <= 0f then
            GD.print $"{string this.Name} -> Free"
            this.QueueFree()
        else
            this.QueueRedraw()

    override this._Draw () =
        this.DrawCircle(Vector2.Zero, this.Radius * health, color)


    static member val Factory =
        // Func только из-за того, что C# слишком вербозно описывает FSharpFunc и Unit.
        System.Func<Minor>(fun () -> failwith "Factory is empty!")
        with get, set
        
    static member create () = Minor.Factory.Invoke ()
    
let (|LeftMouseClick|_|) (ev : InputEvent) =
    match ev with
    | :? InputEventMouseButton as ev ->
        if ev.Pressed && ev.ButtonIndex = MouseButton.Left 
        then Some ev.Position
        else None
    | _ ->
        None

type [<AbstractClass>] Main () =
    inherit Node2D()

    let mutable canvas : Control = null

    member val Radius = 42 with get, set

    override this._Ready () =
        canvas <- this.getNode "UI/ClickCanvas"

    override this._Process delta =
        ()

    member this._on_click_canvas_gui_input (ev : InputEvent) =
        match ev with
        | LeftMouseClick position ->
            canvas.AddChild ^ Minor.create(
                Position = position
                , Radius = float32 this.Radius
            )
        | _ ->
            ()
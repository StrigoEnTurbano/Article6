[<AutoOpen>]
module Utils

open Godot

let inline (^) f x = f x

module GD =
    let print str = 
        GD.Print (str : string)

type Node with
    member this.getNode (name : string) : 'a =
        this.GetNode<'a>(name)
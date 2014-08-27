module Actors

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type ActorType =
    | Predator
    | Animal

type Actor = 
    {
        ActorType: ActorType
        Position: Vector2
        Size: Vector2
        Texture: Texture2D option
    }
    member x.CurrentBounds
        with get() = Rectangle((int x.Position.X), (int x.Position.Y), (int x.Size.X), (int x.Size.Y))

    member x.DesiredBounds
        with get() = x.Position

let createActor (tex, actorType, position, size) =
    { ActorType = actorType; Position = position; Size = size; Texture = tex }


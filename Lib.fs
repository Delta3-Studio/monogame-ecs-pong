[<AutoOpen>]
module Lib

open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

[<Struct>]
type PlayerName =
    | Player1
    | Player2

[<Measure>]
type rad

type Angle = float32<rad>

let vec x y = Vector2(x, y)

let rect (location: Vector2) (size: Vector2) =
    Rectangle(location.ToPoint(), size.ToPoint())

[<Struct>]
type CollisionInfo = { Eid: Eid; Velocity: Vector2 }

module Vector2 =
    let up = Vector2(0.f, -1.f)
    let down = Vector2(0.f, 1.f)
    let left = Vector2(-1.f, 0.f)
    let right = Vector2(1.f, 0.f)
    let toTuple (vector: Vector2) = vector.X, vector.Y

type Vector2 with

    member this.WithX x = vec x this.Y
    member this.WithY y = vec this.X y

let (|Vec|_|) v = v |> Vector2.toTuple |> Some

let (|KeyDown|_|) k (state: KeyboardState) =
    if state.IsKeyDown k then Some() else None

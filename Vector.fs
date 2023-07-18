[<AutoOpen>]
module Game.VectorModule

open Microsoft.Xna.Framework
let vec x y = Vector2(x, y)

let rect (location: Vector2) (size: Vector2) =
    Rectangle(location.ToPoint(), size.ToPoint())

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

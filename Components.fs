namespace Components

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

[<Struct>]
type Translate = { Position: Vector2 }

[<Struct>]
type Rotate = Rotate of Angle

[<Struct>]
type Scale = Scale of single

[<Struct>]
type Velocity = Velocity of Vector2

module Velocity =
    let create x y = Vector2(x, y) |> Velocity

[<Struct>]
type Score = { Player: PlayerName; Value: byte }

[<Struct>]
type GameText =
    { SpriteFont: SpriteFont
      Position: Vector2 }

[<Struct>]
type Logo = { Texture: Texture2D; Speed: single }

[<Struct>]
type Player =
    { Texture: Texture2D
      Size: Vector2
      Index: PlayerName }

[<Struct>]
type Ball = { Size: single; Texture: Texture2D }

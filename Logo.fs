module Logo

open Events
open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Components

let private createLogo (game: Game) =
    { Texture = game.Content.Load("logo")
      Speed = 100f }

let private startPosition (game: Game) =
    { Position = Vector2(single game.Window.ClientBounds.Width / 2f, single game.Window.ClientBounds.Height / 2f) }

let private updateLogoRot (Rotate rot) deltaTime = Rotate(rot + 0.1f<rad> * deltaTime)

let private updateLogoScale (Scale scale) deltaTime =
    if scale < 2f then scale + 0.4f * deltaTime else scale
    |> Scale


let private drawLogo (spriteBatch: SpriteBatch) (logo: Logo) (pos: Translate) (Rotate rot) (Scale scale) =
    let logoCenter =
        Vector2(single logo.Texture.Bounds.Width, single logo.Texture.Bounds.Height)
        / 2f

    spriteBatch.Draw(
        logo.Texture,
        pos.Position,
        logo.Texture.Bounds,
        Color(255, 255, 255, 80),
        single rot,
        logoCenter,
        scale,
        SpriteEffects.None,
        0f
    )

let systems (world: Container) =
    [ world.On(fun (LoadContent game) -> world.Create().With(createLogo game) |> ignore)

      world.On
      <| fun (Start game) ->
          for r in world.Query<Eid, Logo>() do
              let entity = world.Get r.Value1
              entity.Add(startPosition game)
              entity.Add(Rotate 0f<rad>)
              entity.Add(Scale 0f)

      world.On<Update>
      <| fun e ->
          for r in world.Query<Scale, Logo>() do
              let scale = &r.Value1
              scale <- updateLogoScale scale e.TotalSeconds

      world.On<Update>
      <| fun e ->
          for r in world.Query<Rotate, Logo>() do
              let rot = &r.Value1
              rot <- updateLogoRot rot e.TotalSeconds

      world.On<Draw>
      <| fun e ->
          for r in world.Query<Logo, Translate, Rotate, Scale>() do
              let struct (logo, translate, rot, scale) = r.Values
              drawLogo e.SpriteBatch logo translate rot scale ]

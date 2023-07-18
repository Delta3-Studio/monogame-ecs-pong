module Logo

open Events
open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Components
open Types

let private createLogo (game: Game) =
    { Texture = game.Content.Load("logo")
      Speed = 100f }

let private startPosition (game: Game) =
    { Position = Vector2(single game.Window.ClientBounds.Width / 2f, single game.Window.ClientBounds.Height / 2f) }

let private updateLogoRot rot deltaTime = Rotate(rot + 0.1f<rad> * deltaTime)

let private updateLogoScale scale deltaTime =
    if (scale < 2f) then scale + 0.4f * deltaTime else scale
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

let configureLogo (world: Container) =
    [ world.On(fun (LoadContent game) -> world.Create().With(createLogo game) |> ignore)

      world.On(
          fun (Start game) struct (eid: Eid, _: Logo) ->
              let entity = world.Get eid
              entity.Add(startPosition game)
              entity.Add(Rotate 0f<rad>)
              entity.Add(Scale 0f)
              eid
          |> Join.update2
          |> Join.over world
      )

      world.On(
          fun (e: Update) struct (Scale scale, _: Logo) -> updateLogoScale scale e.TotalSeconds
          |> Join.update2
          |> Join.over world
      )

      world.On(
          fun (e: Update) struct (Rotate rot, _: Logo) -> updateLogoRot rot e.TotalSeconds
          |> Join.update2
          |> Join.over world
      )

      world.On<Draw>(
          fun e struct (rot: Rotate, scale: Scale, pos: Translate, logo: Logo) ->
              drawLogo e.SpriteBatch logo pos rot scale
          |> Join.iter4
          |> Join.over world
      ) ]

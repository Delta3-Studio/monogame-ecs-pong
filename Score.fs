module Score

open Components
open Events
open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

let private fontSize = 64f

let private createScore index = { Player = index; Value = 0uy }

let private createText (game: Game) index =
    let spriteFont = game.Content.Load<SpriteFont>("sourcecodepro64")
    let width = single game.GraphicsDevice.Viewport.Width

    let y = 50f

    let x =
        match index with
        | Player1 -> width / 4f - fontSize / 2f
        | Player2 -> width * 3f / 4f - fontSize / 2f

    { SpriteFont = spriteFont
      Position = vec x y }

let configureScore (world: Container) =
    [ world.On
      <| fun (LoadContent game) ->
          world.Create().With(createScore Player1).With(createText game Player1) |> ignore
          world.Create().With(createScore Player2).With(createText game Player2) |> ignore

      world.On<ScoreIncrease>
      <| fun e ->
          for r in world.Query<Score>() do
              let score = r.Value

              if e.PlayerIndex = score.Player then
                  r.Value <- { score with Value = score.Value + 1uy }

      world.On<Draw>
      <| fun e ->
          for r in world.Query<Score, GameText>() do
              let struct (score, text) = r.Values
              e.SpriteBatch.DrawString(text.SpriteFont, string score.Value, text.Position, Color.Black) ]

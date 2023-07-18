module Ball

open Events
open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Components

let private initialSpeed = 3f
let private initialVelocity = Vector2 initialSpeed

let private createBall (game: Game) =
    let texture = new Texture2D(game.GraphicsDevice, 1, 1)
    texture.SetData([| Color.Black |])
    { Texture = texture; Size = 40f }

let configureBall (world: Container) =
    [ world.On(fun (LoadContent game) -> world.Create().With(createBall game) |> ignore)

      world.On
      <| fun (Start game) ->
          for r in world.Query<Eid, Ball>() do
              let entity = world.Get r.Value1
              let ball = r.Value2

              let position =
                  Vector2(
                      single game.Window.ClientBounds.Width / 2f - ball.Size / 2f,
                      single game.Window.ClientBounds.Height / 2f - ball.Size / 2f
                  )

              entity.Add(Rotate 0f<rad>)
              entity.Add(Scale 1f)
              entity.Add { Position = position }
              entity.Add(Velocity initialVelocity)

      world.On<Update>
      <| fun e ->
          for r in world.Query<Velocity, Translate, Ball>() do
              let struct (Velocity velocity, { Translate.Position = pos }, ball) = r.Values
              let screenHeight = single e.Game.GraphicsDevice.Viewport.Height

              if pos.Y + ball.Size > screenHeight || pos.Y < 0f then
                  r.Value1 <- velocity.WithY(-velocity.Y) |> Velocity

      world.On<Update>
      <| fun _ ->
          for r in world.Query<Translate, Velocity, Ball>() do
              let struct (translate, Velocity velocity, _) = r.Values

              r.Value1 <-
                  { translate with
                      Position = translate.Position + velocity }

      world.On<Update>
      <| fun e ->
          for r in world.Query<Translate, Ball>() do
              let struct (translate, ball) = r.Values

              let player1Point =
                  translate.Position.X + ball.Size > single e.Game.GraphicsDevice.Viewport.Width

              let player2Point = translate.Position.X < 0f

              if player1Point then
                  world.Send { PlayerIndex = Player1; Game = e.Game }
              elif player2Point then
                  world.Send { PlayerIndex = Player2; Game = e.Game }

      world.On<ScoreIncrease>
      <| fun s ->
          for r in world.Query<Translate, Ball>() do
              let struct (translate, ball) = r.Values
              let width = single s.Game.GraphicsDevice.Viewport.Width
              let height = single s.Game.GraphicsDevice.Viewport.Height
              let center = vec (width / 2f - ball.Size / 2f) (height / 2f - ball.Size / 2f)
              r.Value1 <- { translate with Position = center }

      world.On<ScoreIncrease>
      <| fun _ ->
          for r in world.Query<Velocity, Ball>() do
              let struct (Velocity velocity, _) = r.Values
              let x = if velocity.X > 0f then -initialSpeed else initialSpeed
              let y = if velocity.Y > 0f then -initialSpeed else initialSpeed
              r.Value1 <- Velocity.create x y

      world.On<CollisionInfo>
      <| fun e ->
          let entity = world.Get e.Eid
          let vel = if e.Velocity.X > 0f then 0.1f else -0.1f
          let x = (-e.Velocity.X) + vel
          e.Velocity.WithX(x) |> Velocity |> entity.Add

      world.On<Draw>
      <| fun draw ->
          for r in world.Query<Translate, Ball>() do
              let struct (translate, b) = r.Values
              draw.SpriteBatch.Draw(b.Texture, (rect translate.Position (Vector2 b.Size)), Color.White) ]

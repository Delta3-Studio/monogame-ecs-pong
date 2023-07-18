module Player

open Events
open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input
open Components

let private createPlayer (game: Game) player =
    let texture = new Texture2D(game.GraphicsDevice, 1, 1)
    texture.SetData([| Color.Black |])

    { Texture = texture
      Size = Vector2(40f, 200f)
      Index = player }

let private clampPosition (size: Vector2) (game: Game) (translate: Translate) =
    let x = translate.Position.X
    let minY = 0f

    let maxY = game.GraphicsDevice.Viewport.Height |> single

    let curY = translate.Position.Y

    match curY with
    | y when y < minY ->
        { translate with
            Position = Vector2(x, 0f) }
    | y when y + size.Y > maxY ->
        { translate with
            Position = Vector2(x, maxY - size.Y) }
    | _ -> translate

let configurePlayer (world: Container) =
    [ world.On(fun (LoadContent game) ->
          world.Create().With(createPlayer game Player1) |> ignore
          world.Create().With(createPlayer game Player2) |> ignore)

      world.On
      <| fun (Start game) ->
          for r in world.Query<Eid, Player>() do
              let entity = world.Get r.Value1
              let player = r.Value2

              let startY = single game.Window.ClientBounds.Height / 2f - player.Size.Y / 2f

              let startX =
                  match player.Index with
                  | Player1 -> player.Size.X
                  | Player2 -> single game.Window.ClientBounds.Width - player.Size.X * 2f

              let position = vec startX startY

              entity.Add(Rotate 0f<rad>)
              entity.Add(Scale 1f)
              entity.Add { Position = position }
              entity.Add(Velocity.create 0f 10f)


      world.On<Update>
      <| fun e ->
          for r in world.Query<Translate, Velocity, Player>() do
              let state = Keyboard.GetState()
              let struct (translate, Velocity velocity, player) = r.Values

              let newVelocity =
                  match player.Index, state with
                  | Player1, KeyDown Keys.W
                  | Player2, KeyDown Keys.Up -> -velocity

                  | Player1, KeyDown Keys.S
                  | Player2, KeyDown Keys.Down -> velocity
                  | _ -> Vector2.Zero

              r.Value1 <-
                  { translate with
                      Position = translate.Position + newVelocity }
                  |> clampPosition player.Size e.Game



      world.On<Update>
      <| fun _ ->
          for r in world.Query<Player, Translate>() do
              let struct (player, translate) = r.Values
              let playerRect = rect translate.Position player.Size

              for ballComps in world.Query<Eid, Ball, Velocity, Translate>() do
                  let struct (eid, ball, Velocity ballVelocity, ballTranslate) = ballComps.Values
                  let ballRect = rect ballTranslate.Position (Vector2 ball.Size)

                  if ballRect.Intersects playerRect then
                      world.Send { Eid = eid; Velocity = ballVelocity }

      world.On<Draw>
      <| fun e ->
          for r in world.Query<Translate, Player>() do
              let struct (tr, p) = r.Values
              let playerRect = rect tr.Position p.Size
              e.SpriteBatch.Draw(p.Texture, playerRect, Color.White) ]

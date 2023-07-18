module Systems

open Garnet.Composition
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input
open Events
open Logo
open Player
open Ball
open Score

let configureWorld (world: Container) =
    [ yield! systems world
      yield! configureBall world
      yield! configurePlayer world
      yield! configureScore world

      // quit game system
      world.On<Update>(fun e ->
          if
              GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed
              || Keyboard.GetState().IsKeyDown(Keys.Escape)
          then
              e.Game.Exit()) ]

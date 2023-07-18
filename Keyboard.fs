module Keyboard

open Microsoft.Xna.Framework.Input

let (|KeyDown|_|) k (state: KeyboardState) =
    if state.IsKeyDown k then Some() else None

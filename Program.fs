open Raylib_cs
open State
open getFile
open gameLogic
open gamemenu
open gameplay
open gameover


let mutable scene = MainMenu


[<EntryPoint>]
let main _ =
    Raylib.InitWindow(width, height, "Suika Game - Grid Ver.")
    initImages ()
    while not (Raylib.WindowShouldClose()) do
        Raylib.BeginDrawing()
        Raylib.ClearBackground(bg)
        scene <-
            match scene with
            | MainMenu -> gameMenu ()
            | Playing gameData -> gamePlay gameData
            | Gameover gameData -> gameOver gameData
        
        Raylib.EndDrawing()
    unloadImages ()
    Raylib.CloseWindow()
    0
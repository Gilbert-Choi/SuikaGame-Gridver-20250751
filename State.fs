module State
open Raylib_cs

type Fruit =
| Cherry | Strawberry | Grape | Orange | Persimmon | Apple
| Pear | Peach | Pineapple | Melon | Watermelon


type Cell = 
| EmptyCell
| Placed of Fruit


type Board (r:int, c:int) = 
    let board = Array2D.create r c EmptyCell
    member this.rows = r
    member this.cols = c
    member this.gameBoard = board
    member this.getCell row col = board.[row, col]
    member this.grid = board

module BoardFunc = 
    let toBoard grid =
        let r = grid |> Array2D.length1
        let c = grid |> Array2D.length2
        let newBoard = Board (r, c)
        Array2D.blit grid 0 0 newBoard.grid 0 0 r c
        newBoard

type GameState = {
    board: Board
    currentFruit: Fruit
    nextFruit: Fruit
    score: uint
    seed: int
}

type Scene = 
| MainMenu
| Playing of GameState
| Gameover of GameState

let width = 480
let height = 800
let bg = Color(255uy, 238uy, 204uy)
let lineColor = Color(0uy, 0uy, 51uy)
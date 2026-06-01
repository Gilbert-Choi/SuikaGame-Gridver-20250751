module gameplay
open Raylib_cs
open State
open getFile
open gameLogic


let getImage size (path:string) =
    let mutable temp = Raylib.LoadImage(path)
    Raylib.ImageResize(&temp, size, size)
    let outputImage = Raylib.LoadTextureFromImage(temp)
    Raylib.UnloadImage(temp)
    outputImage

let mutable fruitMap = Map.empty
let initImages () =
    let img_Cherry = getImage 62 "Fruit01-Cherry.png"
    let img_Strawberry = getImage 64 "Fruit02-Strawberry.png"
    let img_Grape = getImage 68 "Fruit03-Grape.png"
    let img_Orange = getImage 72 "Fruit04-Orange.png"
    let img_Persimmon = getImage 76 "Fruit05-Persimmon.png"
    let img_Apple = getImage 78 "Fruit06-Apple.png"
    let img_Pear = getImage 96 "Fruit07-Pear.png"
    let img_Peach = getImage 96 "Fruit08-Peach.png"
    let img_Pineapple = getImage 96 "Fruit09-Pineapple.png"
    let img_Melon = getImage 84 "Fruit10-Melon.png"
    let img_Watermelon = getImage 96 "Fruit11-Watermelon.png"

    fruitMap <- Map [
        Cherry, img_Cherry
        Strawberry, img_Strawberry
        Grape, img_Grape
        Orange, img_Orange
        Persimmon, img_Persimmon
        Apple, img_Apple
        Pear, img_Pear
        Peach, img_Peach
        Pineapple, img_Pineapple
        Melon, img_Melon
        Watermelon, img_Watermelon
    ]



let unloadImages () = 
    fruitMap |> Map.iter (fun _ img -> Raylib.UnloadTexture(img))

let gamePlay gameData = 
    let mutable gameBoard = gameData.board
    let rec drawBoard r c = 
        if c=gameBoard.cols then ()
        elif r=gameBoard.rows then drawBoard 0 (c+1)
        else
            match gameBoard.getCell r c with
            | EmptyCell -> drawBoard (r+1) c
            | Placed fruit ->
                let correspondFruit = fruitMap |> Map.find fruit
                Raylib.DrawTexture(
                    correspondFruit,
                    40 + c*80 + int (0.5*float (80-correspondFruit.Width)),
                    120 + r*80 + int (0.5*float (80-correspondFruit.Height)),
                    bg)
                drawBoard (r+1) c
                
    //그래픽 작업
    let drawAllBoard gamestate =
        [40; 120; 200; 280; 360; 440]
        |> List.iter (fun x -> Raylib.DrawLine(x, 200, x, 760, lineColor)) //Vertical
        [200; 280; 360; 440; 520; 600; 680; 760]
        |> List.iter (fun y -> Raylib.DrawLine(40, y, 440, y, lineColor)) //Horizontal
        
        drawBoard 0 0
        match gamestate.currentFruit, gamestate.nextFruit with
        | cfruit, nfruit ->
            let cfruit_img = fruitMap |> Map.find cfruit
            let nfruit_img = fruitMap |> Map.find nfruit
            Raylib.DrawTexture(cfruit_img, Raylib.GetMouseX(), 100, bg)
            Raylib.DrawTexture(nfruit_img, 400, 20, bg)
        let scoreText = gamestate.score |> sprintf "SCORE : %d"
        Raylib.DrawText(scoreText, 20, 10, 30, lineColor)
        let nextText = sprintf "NEXT"
        Raylib.DrawText(nextText, 350, 20, 30, lineColor)
    //그래픽 작업 끝

    let isFileFilled =
        [0 .. gameBoard.cols - 1]
        |> Seq.exists (fun c -> gameBoard.getCell 0 c <> EmptyCell)

    let LeftClicked:bool = Raylib.IsMouseButtonPressed(MouseButton.Left)
    let nextData:Scene =
        if isFileFilled then
            Gameover gameData
        elif LeftClicked then
            let mouseX = Raylib.GetMouseX()
            Playing (gameUpdate (getFile mouseX) gameData)
        else
            Playing gameData
    
    match nextData with
    | Gameover state -> Gameover state
    | Playing state ->
        drawAllBoard state
        Playing state
    | MainMenu -> MainMenu
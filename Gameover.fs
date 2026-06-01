module gameover
open Raylib_cs
open State
open gameLogic
open gameplay

let color9CF = Color(153uy, 204uy, 255uy)
let colorCFF = Color(204uy, 255uy, 255uy)

let gameOver gameData = 
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
    //그래픽 작업 끝


    let scoreText = int gameData.score |> sprintf "SCORE : %d"
    Raylib.DrawText(scoreText, 20, 10, 30, lineColor)

    let drawButton (x:float32) (y:float32) (width:float32) (height:float32)
        (text:string) (size: float32) color1 color2=
        let btnRect = Rectangle(x, y, width, height)
        let mousePos = Raylib.GetMousePosition()
        let isHovered:bool = Raylib.CheckCollisionPointRec(mousePos, btnRect)
        let color = if isHovered then color1 else color2
        let label = text |> sprintf "%s"
        let textWidth = float32 (Raylib.MeasureText(text, int size))
        Raylib.DrawRectangleRec(btnRect, color)
        Raylib.DrawText(
            label,
            int (x + 0.5f*width - 0.5f*textWidth),
            int (y + 0.5f*height - 0.5f*size),
            int size,
            lineColor)
        if (Raylib.IsMouseButtonPressed (MouseButton.Left)) && isHovered then
            let initSeed =
                (Raylib.GetMouseX()-int x
                + (Raylib.GetMouseY()-int y)*int width) % 100000
            //마우스의 x좌표 + y좌표*너비 값을 최초의 시드로 정한다.
            //시작 버튼을 정확히 같은 위치에 누르지 않은 이상 과일 순서는 매번 달라진다.
            Playing (gameInit initSeed)
        else Gameover gameData
    
    drawAllBoard gameData
    let message = sprintf "Game Over!"
    Raylib.DrawText(message, 20, 80, 30, lineColor)
    drawButton 280f 20f 120f 60f "Restart" 20f color9CF colorCFF
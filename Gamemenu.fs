module gamemenu
open Raylib_cs
open State
open gameLogic

let color9CF = Color(153uy, 204uy, 255uy)
let colorCFF = Color(204uy, 255uy, 255uy)
let rec gameMenu () =
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
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) && isHovered then
            let initSeed =
                (Raylib.GetMouseX()-int x
                + (Raylib.GetMouseY()-int y)*int width) % 100000
            //마우스의 x좌표 + y좌표*너비 값을 최초의 시드로 정한다.
            //시작 버튼을 정확히 같은 위치에 누르지 않은 이상 과일 순서는 매번 달라진다.
            Playing (gameInit initSeed)
        else MainMenu
    
    let title = sprintf "Suika Game - Grid Ver."
    Raylib.DrawText(title, 20, 10, 30, lineColor)
    drawButton 180f 200f 120f 60f "Start" 20f color9CF colorCFF
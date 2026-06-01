module gameLogic
open State
open BoardFunc


let random seed = (12341*seed + 12347)%100000 //PseudoRandom : mod 100000
let getRandomFruit seed =
    let divTrans = seed/20000
    if divTrans=0 then Cherry
    elif divTrans=1 then Strawberry
    elif divTrans=2 then Grape
    elif divTrans=3 then Orange
    else Persimmon

let gameInit seed =
    let gameBoard = Board (8,5)
    let initState = {
        board = gameBoard
        currentFruit = getRandomFruit (random seed)
        nextFruit = getRandomFruit (random (random seed))
        score = 0u
        seed = random (random seed)
    }
    initState
let fruit_order =
    [ Cherry; Strawberry; Grape; Orange; Persimmon; Apple;
    Pear; Peach; Pineapple; Melon; Watermelon;]
   
let adv fruit =
    let rec loop order =
        match order with
            | x :: y :: tl when x=fruit -> Placed y
            | _ :: tl -> loop tl
            | [] -> EmptyCell //수박이 합쳐지면 빈칸이 된다.
    loop fruit_order

let getScore fruit =
    match fruit with
    | Cherry -> 1u | Strawberry -> 2u | Grape -> 4u
    | Orange -> 7u | Persimmon -> 11u | Apple -> 16u
    | Pear -> 25u | Peach -> 40u | Pineapple -> 80u
    | Melon -> 500u | Watermelon -> 2000u




//게임의 상태를 변화시키는 함수. 게임이 돌아가게 하는 역할을 한다.
//file, board, current, next, score, seed를 입력받아 new-board, new-current, new-next, new-score, new-seed를 출력한다
let gameUpdate file gameRecord =
    let mutable newScore = gameRecord.score
    let addScore pts = newScore <- newScore + pts
    
    if file=0 then gameRecord
    else
        let putFruit (grid:Cell[,]) =
            grid |> Array2D.mapi (fun r c cell ->
                if r=0 && c=file-1 then Placed gameRecord.currentFruit else cell
            )
            

        
        //isStable의 조건: unstable이 아닐때
        //아래 중 하나라도 만족하면 unstable이다.
        //상하로 인접한 두 칸 중 위 칸이 어떤 과일인데, 아래칸이 빈칸이거나 같은과일일때 (Vertical),
        //좌우로 인접한 두 칸이 같은 과일일때 (Horizontal).
        let isStable grid =
            let unstableV =
                [0 .. (grid |> Array2D.length2) - 1]
                |> Seq.exists (fun col ->
                    grid.[*, col]
                    |> Array.pairwise
                    |> Array.exists (fun (a,b) -> 
                        match a,b with
                        | EmptyCell, _ -> false
                        | Placed _, EmptyCell -> true
                        | Placed fa, Placed fb -> fa=fb
                    )
                )
            let unstableH =
                [0 .. (grid |> Array2D.length1) - 1]
                |> Seq.exists (fun row ->
                    grid.[row, *]
                    |> Array.pairwise
                    |> Array.exists (fun (a,b) ->
                        match a,b with
                        | Placed fa, Placed fb -> fa=fb
                        | _, _ -> false
                    )
                )
            not (unstableV || unstableH)


        //drop은 과일 아래칸이 비어있을 때 그 칸으로 이동하는 역할을 한다.
        //과일 아래칸이 같은 과일일 때는 두 과일을 합치고 점수를 올리는 mergeBelow도 함께 수행한다.
        let drop grid =
            let rec dropCol col =
                match col with
                | [] -> []
                | x :: tl when x=EmptyCell -> dropCol tl
                | x :: tl -> x :: dropCol tl
            let rec mergeBelow col: Cell list =
                match col with
                | [] -> []
                | [x] -> [x]
                | x :: y :: tl ->
                    match x, y with
                    | Placed fa, Placed fb when fa=fb ->
                        addScore (getScore fa) //점수 추가
                        mergeBelow (adv fa :: tl)
                    | _ -> x :: mergeBelow (y :: tl)
            let fillEmptyCell col =
                let rec loop n lst =
                    if n=0 then lst else loop (n-1) (EmptyCell :: lst)
                let remain = (grid |> Array2D.length1) - (col |> List.length)
                loop remain col

            let transpose grid =
                Array2D.init (grid |> Array2D.length2) (grid |> Array2D.length1) (fun r c -> grid.[c,r])

            let resultGrid =
                [0 .. (grid |> Array2D.length2) - 1]
                |> Seq.map (fun col ->
                    grid.[*, col] |> Array.toList
                    |> dropCol
                    |> mergeBelow 
                    |> fillEmptyCell
                    |> List.toArray
                )
                |> Seq.toArray
                |> array2D
                |> transpose
            resultGrid


        //mergeLeftRight는 같은 과일이 양옆으로 붙어있을 때 합성하는 과정이다.
        let mergeLeftRight grid =
            let rec mergeRow row = 
                match row with
                | [] -> []
                | [x] -> [x]
                | x :: y :: tl ->
                    match x, y with
                    | Placed fa, Placed fb when fa=fb -> 
                        addScore (getScore fa) //점수 추가
                        adv fa :: EmptyCell :: mergeRow tl
                    | _ -> x :: mergeRow (y :: tl)
            
            let resultGrid = 
                [0 .. (grid |> Array2D.length1) - 1]
                |> Seq.map (fun row ->
                    grid.[row, *]
                    |> Array.toList
                    |> mergeRow
                    |> List.toArray
                )
                |> Seq.toArray
                |> array2D
            resultGrid


        let translate grid =
            toBoard grid
        
        let takeCurrentFruit = gameRecord.nextFruit
        let takeNextFruit = getRandomFruit (random gameRecord.seed)

        let rec changeBoard grid =
            if isStable grid then grid
            else
                grid
                |> drop
                |> mergeLeftRight
                |> changeBoard

        let newBoard =
            gameRecord.board.grid
            |> putFruit
            |> changeBoard
            |> translate
        {gameRecord with
            board = newBoard
            currentFruit = takeCurrentFruit
            nextFruit = takeNextFruit
            seed = random gameRecord.seed
            score = newScore
        }
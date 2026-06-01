module getFile

let getFile x =
    let cellInterval = 80
    let beginBoard = 40
    let endBoard = 440
    if x<beginBoard then
        0
    elif x>endBoard then
        0
    else
        1 + (x-beginBoard) / cellInterval
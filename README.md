# Suika game - Grid Ver.

# SuikaGame-Gridver-20250751

Name : Minjun Choi (최민준)
Email : mircmjkr@kaist.ac.kr

# How to Play?
First, you should download .NET SDK environment.
Here is the link of .NET SDK download: https://dotnet.microsoft.com/download

Second, open the terminal and write these commands in order.
1. git clone https://github.com/Gilbert-Choi/SuikaGame-Gridver-20250751.git
2. dotnet run


# Logics of the Game
You can choose one of the 5 files(columns) by mouse click.
When you click the mouse, fruit is automatically dropped and merged.
When two same fruits are adjacent one another, they merge to advanced fruit and get score.

Fruits are randomly selected between Cherry and Persimmon according to the seed. The seed number is determined only by the position of mouse cursor when it clicked 'start' button. The next fruit is determined by evaluating the pseudorandom code.

There are two important rule to understand before the play:
1. When merging, the fruit at the bottom takes precedence over the fruits on the left and right.
2. the fruit at the left takes precedence over the fruits on the right.

When you click the file full of fruits, the fruit is placed out of grid and game over.



# Changes and Why
I included all logics such as drop, mergeBelow, mergeLeft, and mergeRight. But I tied up some functions (drop and mergeBelow), (mergeLeft and mergeRight).
Fruit dropping can be explained by filtering EmptyCell in list and adding EmptyCell to complement the column. In this step, vertical merging can be efficient by using list.
Two functions mergeLeft and mergeRight virtually conducts same operations, so I tied up two functions using List.pairwise.


# AI Assistant
I used AI to understand and apply the Raylib graphics such as how to print out textures and texts.
# ITCS 4236 Game Project / Chess With Guns

Final project for "Artificial Intelligence for Computer Games" course.
You've heard of chess, you've heard of guns, but have you ever heard of chess WITH guns?!?!
Welcome to a 2d board game, you can play with a friend or against our AI. The goal is the same as chess where you must destroy your enemy's king.

**Important:** Make sure that you set your display to 16:9, and ensure that the panels in Unity are adjusted so that the user interfaces in the game are properly aligned with the board.

## How to Play

### Board Controls:

- Left click to select your piece.
- Left click a green tile to move to that tile.
  - A piece must be selected to move.
- Left click a non-green tile to deselect a piece.
- Left clicking a piece that is already selected will move the piece to the same tile.
  - This lets you attack with a piece without moving it.
- Left click an enemy piece with an orange dot to attack that piece.
  - Happens automatically if you move a piece within attack range of an enemy piece.

- Right click to destroy your piece.
  - If you destroy your King, you lose.

### Shop Controls:

The shop is the GUI on the right side of the screen.

- Left click a piece to select it.
  - You can only select one piece at a time.
- Left click the "X" button to deselect a piece.
- Spawn Zones:
  - The top 2 rows are player 1's spawn zone.
  - The bottom 2 rows are player 2's spawn zone.
- Left click an empty space 2 rows within your spawn zone to spawn a piece.
  - You must have a piece selected to spawn.
  - You must have enough money to spawn a piece.

## Pieces

### King

<img src="Assets/Piece Images/movedKing.png" width="50"/>

- Movement: 1 tile
- Attack Range: 1 tile
- Attack Damage: 1
- Health: 15
- **Note:** If your King is destroyed, you lose.

### Soldier

<img src="Assets/Piece Images/SelectedSoldier_image.png" width="40"/>

- Movement: 1 tiles
- Attack Range: 1 tile
- Attack Damage: 1
- Health: 1
- Cost: 10

### Sniper

<img src="Assets/Piece Images/SelectedSniper_image.png" width="40"/>

- Movement: 1 tile
- Attack Range: 4 tiles
- Attack Damage: 1 to 4
- Health: 1
- Cost: 40

### Tank

<img src="Assets/Piece Images/SelectedTank_image.png" width="40"/>

- Movement: 1 tile
- Attack Range: 2 tiles
- Attack Damage: 3 or 4
- Health: 5
- Cost: 70

### Helicopter

<img src="Assets/Piece Images/movedHeli.png" width="40"/>

- Movement: 2 tiles
- Attack Range: 2 tiles + carrying piece's attack range / 2 (floored)
- Attack Damage: 2 tiles + carrying piece's attack damage
  - If the carrying piece has a variable attack damage, a random number in the range of the attack damage will be chosen.
- Health: 3 tiles + carrying piece's attack range / 2 (floored)
- Cost: 100

### Tnt

<img src="Assets/Piece Images/tnt.png" width="50"/>

- Cost: 50
- **Note:** Tnt is a special piece that can be used to destroy a wall within 1 tile of a piece you own. It doesn't actually get placed on the board.

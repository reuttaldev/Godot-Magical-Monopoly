# Introduction 
This project’s concept is a magical version of Monopoly. You and a friend play as witches, competing to become the most powerful by obtaining the most “Magic Points.”  
It was developed using the Godot 3 game engine and .NET.

# Game Rules
- There are 4 card categories: property, reward, fine, and jail. 
- You can choose whether to buy the property you land on. 
- If you land on your opponent's property, you must pay them a fine - 50% of the original price of the property.
- The first to reach zero ‘Magic Points’ loses the game.
- If you land on a jail card, you shall wait for 2 turns to break free.

## Download Link
To access the Windows build, please follow this [link](https://drive.google.com/file/d/1aKQG4GHajXYaUmMVjQUkoRyVrzXZn5oh/view?usp=sharing). Download, unzip, and run the executable file. Please contact reutgaming@gmail.com for further assistance or a platform-specific build.

## Game snippet
[![Watch the demo on Google Drive](https://drive.google.com/thumbnail?id=1pY6jzij4AF1mp9eMgLbIvKN9DCNiOFCD&sz=w1200-h675)](https://drive.google.com/file/d/1pY6jzij4AF1mp9eMgLbIvKN9DCNiOFCD/view?usp=sharing)

# Implementation

The game was deve
1. Calling DropDice() from GameController
2. After there is a result from the dice, the first move of the player is triggered, moving one tile at a time- until we reach our target destination.
4. Once the player is at the target position moving, LandOnCard() in GameController handles the buying process, rewards, fines, and jail.
5. Once all actions are done, the turn moves to the next player.
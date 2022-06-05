Dear professor Dingle, 
I have created a magical board game. In the game, you and a friend play as witches, competing to be more powerful by receiving the most 'Magic Points'.

Game Rules:
- There are 4 card categories: property, reward, fine, and jail. 
- You can choose whether to buy the property you land on. 
- If you land on your opponent's property, you must pay them a fine - 50% of the original price of the property.
- The first to reach zero ‘Magic Points’ loses the game.
- If you land on a jail card, you shall wait for 2 turns to break free.

Workflow:
1. Calling DropDice() from GameController
2. After there is a result from the dice, the first move of the player is triggered, moving one tile at a time- until we reach our target destination.
4. Once the player is at the target position moving, LandOnCard() in GameController handles the buying process, rewards, fines, and jail.
5. Once all actions are done, the turn moves to the next player.

Thank you and enjoy,
Reut
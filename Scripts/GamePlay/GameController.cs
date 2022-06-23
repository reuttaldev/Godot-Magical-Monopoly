    using Godot;
using System;

public class GameController : Node
{
    internal const int NUM_OF_CARDS = 24;
    readonly string[] playersNames = {"Player 1","Player 2"};
    [Export]
    private float playerMovingSpeed=10;
    [Export]
    internal  Vector3 playersStartPos;
    [Export]
    private int jailWaitTurns = 2; // the number of turns a player has to wait in order to get out of jail
    private int currentPlayer = 0, roundCounter =1;
    private bool gameOver = false;
    private Card [] cardArry = new Card [NUM_OF_CARDS]; // an arry that contains all card instances from scene
    private Transform[] cardsTransforms = new Transform [NUM_OF_CARDS]; // transform of all cards in scene so we know where to move players to
    private Player[] playersArry = new Player[2];
    private UIGameController uiController;
    private DiceController diceController;
    public float PlayerMovingSpeed { get { return this.playerMovingSpeed; }set { this.playerMovingSpeed = value; } }
    public bool GameOver { get { return this.gameOver; } set { this.gameOver = value; } }
     public override void _Ready()
    {
        //  export is not like serializable field, you cannot assign a node in the editor :(
        uiController = (UIGameController)GetNode("Control");
        diceController = (DiceController)GetNode("Control/Dice");
        for (int i = 0; i < playersArry.Length; i++)
        {
            playersArry[i] = (Player)GetNode("Player "+(i+1).ToString());
            // making game controller listen to the players arrived signal, and tell it what method to execute
            playersArry[i].Connect("arrived",this,"HandlePlayerArriving");

        }
        var cardTemp = GetTree().GetNodesInGroup("Card Properties");
        var transformTemp = GetTree().GetNodesInGroup("Card Positions");
        for (int i = 0; i < cardTemp.Count; i++)
        {
            cardArry[i] = (Card)cardTemp[i];
            Spatial s = (Spatial)transformTemp[i];
            cardsTransforms[i] =s.Transform;
        }
        StartTurn();
    }

    private void StartTurn()
    {
        if(gameOver)
            return;
        int result = diceController.RollDice();
    }
    private void SwitchTurns()
    {
        int lastPlayer = currentPlayer;
        currentPlayer = (currentPlayer+1)%2;
        if(playersArry[lastPlayer].JailTime != 0)
            playersArry[lastPlayer].JailTime-=1;
    }
        internal void MoveCurrentPlayer(int howMuch)
    {
        Player player = playersArry[currentPlayer];
        int currentPosition = player.IndexOnBoard;
        int targetPosIndex = currentPosition + howMuch;
        // trigger the move to the next card
        HandlePlayerArriving(currentPosition,targetPosIndex);
    }
    // when the player sends the signal that it arrived where it needed and stopped moving
    internal void HandlePlayerArriving(int currentIndex , int targetIndex )
    {
        // figure out if it needs to move any more, or it is at the target position
        if(currentIndex == targetIndex)
        {
            LandOnCard(currentIndex);
            return;
        }
        // next tile position on the board
        int nextIndex = currentIndex+1;
        // if we are on the last tile
        if (nextIndex >= cardArry.Length - 1)
        {
            targetIndex = targetIndex - currentIndex;
            nextIndex = 0;
            // we need to do this only once, otherwise the round will change whenever ONE of the player passes start (it will change twice accidentaly)
            if(currentPlayer == 0) // if the other player before it didnt cross yet
            {
                roundCounter++;
                uiController.ChangeRoundCountText(roundCounter);
            }
        }
         //adding these values so the player wont bump into each other while being on the same tile
        playersArry[currentPlayer].MoveTo(targetIndex,nextIndex, cardsTransforms[nextIndex].origin);
    }
    void LandOnCard(int positionOnBoard)
    {
        Player player = playersArry[currentPlayer];
        // the card the player is currently on
        Card card = cardArry[player.IndexOnBoard];
        CardCategory catagory = card.Catagory;
        int cost = card.Cost;
        GD.Print(" index "+player.IndexOnBoard);

        switch (catagory)
        {
            case CardCategory.property:
                uiController.DisplayPopUp(catagory,card.message,cost);
                break;
            case CardCategory.takenProperty:
                // if it is owned by the opponent        
                if (card.OwnedBy != currentPlayer)
                {
                    //The fine is equal to 50% from the amount your opponent has paid in order to buy the property. 
                    cost =cost/2;
                    GD.Print("original cost = "+card.Cost+" new cost "+cost);
                    player.SubtractMagicPoints(cost);
                    // add those point to the opponent
                    playersArry[(currentPlayer+1)%2].AddMagicPoints(cost);
                    SwitchTurns();
                }
                break;
            case CardCategory.reward:
                player.AddMagicPoints(cost);
                SwitchTurns();
                break;
            case CardCategory.fine:
                player.SubtractMagicPoints(cost);
                SwitchTurns();
                break;
            case CardCategory.jail:
                player.JailTime = jailWaitTurns;
                SwitchTurns();
                break;
        }
        if(!gameOver)
        {
            uiController.DisplayPopUp(catagory,card.message,cost);
            uiController.UpdateAmountDisplay(currentPlayer,player.MpAmount);
        }
    }
    

    // this method will be conected to the continue button in the pop up panel
    public void ContinueButton()
    {
        uiController.ClosePopUpPanel();
        uiController.ChangeNowPlayingText(playersNames[currentPlayer]); 
        StartTurn();
    }
    public void YesButton()
    {
        // if the player can buy it
        Player player = playersArry[currentPlayer];
        Card card = cardArry[player.IndexOnBoard];
        GD.Print("card index "+player.IndexOnBoard);
        if(player.MpAmount - card.Cost>=0)
        {
            // make the player buy it
            player.SubtractMagicPoints( card.Cost);
            card.ChangeCardOwnership(currentPlayer);
            uiController.UpdateAmountDisplay(currentPlayer,player.MpAmount);
            uiController.ChangeCardColor(player.IndexOnBoard, currentPlayer);
            SwitchTurns();
            ContinueButton();

        }
        else
        {
            // display a text saying you dont have enough points
            uiController.CantBuyText();
        }
    }
    public void NoButton()
    {
        SwitchTurns();
        ContinueButton();
    }
    internal void RestartGame(int firstPlayPos,int secondPlayerPos)
    {
        uiController.ClosePopUpPanel();
        for (int i = 0; i < playersArry.Length; i++)
        {
            playersArry[i].SetUpPlayer();
        }
        // changing the playing now text to the correct one
        currentPlayer = 0;
        uiController.ChangeNowPlayingText(playersNames[currentPlayer]);
    }
    internal void GameIsOver(int losingIndex)
    {
        GD.Print("Game is over");
        gameOver = true;
        int winningIndex  = (losingIndex+1)%2;
        string winnerName = playersNames[winningIndex];
        uiController.GameOver(winnerName);
        uiController.UpdateAmountDisplay(losingIndex,0);
    }
}

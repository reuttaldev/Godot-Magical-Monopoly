    using Godot;
using System;

public class GameController : Node
{
    const int numOfCards = 24;
    readonly string[] playersNames = {"Player 1","Player 2"};
    [Export]
    private float playerMovingSpeed=10;
    [Export]
    private int jailWaitTurns = 2; // the number of turns a player has to wait in order to get out of jail
    private int currentPlayer = 0, roundCounter =1;
    private bool gameOver = false;
    private Card [] cardArry = new Card [numOfCards]; // an arry that contains all card instances from scene
    private Transform[] cardsTransforms = new Transform [numOfCards]; // transform of all cards in scene so we know where to move players to
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
        int result = diceController.RollDice();
        GD.Print("cube rolled "+result);
    }
    private void SwitchTurns()
    {
        int next = (currentPlayer+1)%2;
        //if(playersArry[currentPlayer].JailTime != 0)
        //{
         //   playersArry[currentPlayer].JailTime-=1;
        //}
        //else
            currentPlayer = next;
    }
        internal void MoveCurrentPlayer(int howMuch)
    {
        GD.Print("Moving player "+(currentPlayer+1)+ " " +howMuch);
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
            GD.Print("got to final destenation");
            LandOnCard(currentIndex);
            return;
        }
        // next tile position on the board
        int nextIndex = currentIndex+1;
        GD.Print("GameController: called to move player to index"+nextIndex);
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
        CardCatagory catagory = card.Catagory;
        int cost = card.Cost;
        GD.Print((currentPlayer +1)+" landed on card with cat "+catagory+" and cost of "+card.Cost);
        if ( catagory == CardCatagory.takenProperty)
        {
            // if it is owned by the opponent        
            if (card.OwnedBy != currentPlayer)
            {
                //The fine is equal to 50% from the amount your opponent has paid in order to buy the property. 
                cost =cost/2;
                player.SubtractMagicPoints(cost);
                // add those point to the opponent
                playersArry[(currentPlayer+1)%2].AddMagicPoints(cost);
            }
            // if we land on a card we already own
            else
            {

            }
        }
        if (catagory == CardCatagory.reward)
            player.AddMagicPoints(cost);
        if(catagory == CardCatagory.fine)
            player.SubtractMagicPoints(cost);
        if(catagory == CardCatagory.jail)
            player.JailTime = jailWaitTurns;

        uiController.UpdateAmountDisplay(currentPlayer,player.MpAmount);
        uiController.DisplayPopUp(catagory,card.message,cost);
        SwitchTurns();
    }

    // this method will be conected to the continue button in the pop up panel
    public void ContinueButton()
    {
        uiController.ClosePopUpPanal();
        uiController.ChangeNowPlayingText(playersNames[currentPlayer]); 
        StartTurn();
    }
    public void YesButton()
    {
        // if the player can buy it
        Player player = playersArry[currentPlayer];
        Card card = cardArry[player.IndexOnBoard];
        if(player.MpAmount - card.Cost>=0)
        {
            // make the player buy it
            player.SubtractMagicPoints( card.Cost);
            card.ChangeCardOwnership(currentPlayer);
            uiController.UpdateAmountDisplay(currentPlayer,player.MpAmount);
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
        ContinueButton();
    }
    internal void RestartGame(int firstPlayPos,int secondPlayerPos)
    {
        GD.Print("Game Reopened");
        uiController.ClosePopUpPanal();
        for (int i = 0; i < playersArry.Length; i++)
        {
            playersArry[i].SetUpPlayer();
        }
        // changing the playing now text to the correct one
        currentPlayer = 0;
        uiController.ChangeNowPlayingText(playersNames[currentPlayer]);
    }
    internal void GameIsOver()
    {
        GD.Print("Game is over");
        gameOver = true;
        string winnerName = "";
        if (playersArry[0].MpAmount >= playersArry[1].MpAmount)
            winnerName = playersNames[0];
        else
            winnerName = playersNames[1];
        uiController.GameOver(winnerName);
    }
}

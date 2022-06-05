    using Godot;
using System;

public class GameController : Node
{
    const int numOfCards = 24;
    readonly string[] playersNames = {"Player 1","Player 2"};
    [Export]
    private float jailTimerLength = 300 ,playerMovingSpeed=15;
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
        currentPlayer = (currentPlayer+1)%2;
    }
        internal void MoveCurrentPlayer(int howMuch)
    {
        GD.Print("Moving player "+(currentPlayer+1)+ " " +howMuch);
        Player player = playersArry[currentPlayer];
        int currentPosition = player.IndexOnBard;
        int targetPosIndex = currentPosition + howMuch+1;
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
        if (nextIndex == cardArry.Length - 1)
        {
            targetIndex = targetIndex - currentIndex;
            nextIndex = 0;
            // meaning we have started a new circle
            roundCounter++;
            uiController.ChangeRoundCountText(roundCounter);
        }
        Vector3 cardTransform = cardsTransforms[nextIndex].origin;
         //adding these values so the player wont bump into each other while being on the same tile
        Vector3 nextTransform = new Vector3(cardTransform[0] + playersArry[currentPlayer].PaddingValue[0], 0, cardTransform[2] - playersArry[currentPlayer].PaddingValue[2]);
        playersArry[currentPlayer].MoveTo(targetIndex, nextTransform);
    }

    void LandOnCard(int positionOnBoard)
    {
        Player player = playersArry[currentPlayer];
        // the card the player is currently on
        Card card = cardArry[player.IndexOnBard];
        CardCatagory catagory = card.Catagory;
        if ( catagory == CardCatagory.property )
        {
            // make the player buy it
            player.SubtractMagicPoints(card.Cost);
            card.ChangeCardOwnership(currentPlayer);
        }
        else if ( catagory == CardCatagory.takenProperty)
        {
            // if it is owned by the opponent        
            if (card.OwnedBy != currentPlayer)
            {
                // pay the fine
                player.SubtractMagicPoints(card.Cost);
                // add those point to the opponent
                playersArry[(currentPlayer+1)%2].AddMagicPoints(card.Cost);
            }
        }
        else if (catagory == CardCatagory.reward)
            player.AddMagicPoints(card.Cost);
        else if(catagory == CardCatagory.fine)
            player.SubtractMagicPoints(card.Cost);
        else if(catagory == CardCatagory.jail)
            uiController.ShowJailPopUp( jailTimerLength);

        uiController.DisplayPopUp(catagory,card.message,card.Cost);
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
        
    }
    public void NoButton()
    {

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
        gameOver = true;
        string winnerName = "";
        if (playersArry[0].MpAmount >= playersArry[1].MpAmount)
            winnerName = playersNames[0];
        else
            winnerName = playersNames[1];
        uiController.GameOver(winnerName);
    }
}

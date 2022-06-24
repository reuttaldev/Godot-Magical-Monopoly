using Godot;
using System;
public class Player : KinematicBody
{
    
    private const int MPStartAmount = 1500 ;
    [Export]
    private Vector3 paddingValue; // padding value is that the players wont be on top of each other when on the same tile
    [Export]
    public int playerIndex;
    // current index is the index we are currently standing on
    // next transfom in the next position in our path
    // target index is our final destanation
    private int indexOnBoard,targetIdex, nextIndex; 
    // MP is magic points
    private int mpAmount = 0; // magic points amount
    private Vector3 direction = Vector3.Zero, nextTransform;
    public int MpAmount { get { return this.mpAmount; } }
    private int jailTime = 0; // the number of turns the player has left in jail. 0 means its not in jail
    public int PlayerIndex { get { return this.playerIndex; } }
    public int JailTime  {  get { return this.jailTime; } set { this.jailTime = value; }  }
    public int IndexOnBoard  {  get { return this.indexOnBoard; } set { this.indexOnBoard = value; }  }
    public int TargetIndex  {  get { return this.targetIdex; } set { this.targetIdex = value; }  }
    private UIGameController uiController;	
    private GameController gameController;
    [Signal]
    delegate void arrived(int c , int t );// our current and target indexes
    private AnimationPlayer animationPlayer;
    private int movementCounter = 0;

    public override void _Ready()
    {
        uiController = (UIGameController)GetNode("../Control");
        gameController = (GameController)GetNode("../");
        animationPlayer = (AnimationPlayer)GetNode("mannequiny-030/AnimationPlayer");
        SetUpPlayer();
        SetPhysicsProcess(false);
    }
    internal void SetUpPlayer()
    {
        jailTime = 0;
        mpAmount = MPStartAmount;
        uiController.UpdateAmountDisplay(this.playerIndex, MPStartAmount);
        indexOnBoard =0;
        Translation =new Vector3(gameController.playersStartPos[0]+paddingValue[0],paddingValue[1],gameController.playersStartPos[2]+paddingValue[2]);
        jailTime = 0;
    }
    #region MOVMENT
    internal void MoveTo(int tindex,int nIndex, Vector3 nTransform)
    {
        targetIdex = tindex;
        nextIndex = nIndex;
        nextTransform =   new Vector3(nTransform[0]+paddingValue[0],paddingValue[1],nTransform[2]+paddingValue[2]);
        SetPhysicsProcess(true);
    }

    public override void _Process(float delta)
    {
        base._Process(delta);
            if(animationPlayer.CurrentAnimation !="run" &&animationPlayer.CurrentAnimation !="idle")
            {
                animationPlayer.Play("idle");
            }
    }
    //happens at a fixed rate, 60 times per second by default
    // we shut this off and on depending if we need to move or not.

    public override void _PhysicsProcess(float delta)
    {
        // when arrived at next position
        if(nextTransform.DistanceTo(Transform.origin)<0.5)
        {
            // set a signal to the game controller we have stopped moving
            this.indexOnBoard = nextIndex;
            SetPhysicsProcess(false);
            EmitSignal(nameof(arrived),indexOnBoard,targetIdex);
            //rotate when needed
            movementCounter++;
            if(movementCounter== GameController.NUM_OF_CARDS/4)
            {
                movementCounter = 0;
                RotationDegrees = new Vector3(0,RotationDegrees[1]-90,0);
            }

        }
        else
        {
            direction = (nextTransform-Transform.origin);
            MoveAndSlide(direction.Normalized()*gameController.PlayerMovingSpeed);
            if(animationPlayer.CurrentAnimation !="run")
                animationPlayer.Play("run");

        }
    }

    #endregion
    #region CURRENCY CONTROLLERS
    internal void AddMagicPoints(int amount)
    {
        this.mpAmount += amount;
        uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
        GD.Print("adding  "+amount+" from player "+(playerIndex+1));
    }
    internal void SubtractMagicPoints(int amount)
    {
        if (this.mpAmount - amount >= 0)
        {
            this.mpAmount -= amount;
            uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
        }
        else
            gameController.GameIsOver(playerIndex);
        GD.Print("subtracting  "+amount+" from player "+(playerIndex+1));
        

    }

    #endregion
}

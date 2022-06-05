using Godot;
using System;
public class Player : KinematicBody
{
    
    private const int MPStartAmount = 300;
    [Export]
    private float constY; // the players y value that it looks like hes on top of the table
    [Export]
    private Vector3 startingPos, paddingVale; // padding value is that the players wont be on top of each other when on the same tile
    [Export]
    private int playerIndex, indexOnBard,targetIdex; //posOnBoard will mark the index of the tile the player is currently standing on
    // MP is magic points
    private int mpAmount = 0; // magic points amount
    private Vector3 direction = Vector3.Zero, nextTransform;
    public int MpAmount { get; }
    private bool inJail = false;
    public int PlayerIndex { get; }
    public bool InJail  { get; set;  }
    public int IndexOnBard  { get; set;  }
    public int TargetIndex  { get; set;  }
    public Vector3 PaddingValue {get;}
    private UIGameController uiController;	
    private GameController gameController;
    [Signal]
    delegate void arrived(int c , int t );// our current and target indexes
    public override void _Ready()
    {
        uiController = (UIGameController)GetNode("../Control");
        gameController = (GameController)GetNode("../");
        SetUpPlayer();
        SetPhysicsProcess(false);
    }
    internal void SetUpPlayer()
    {
        uiController.UpdateAmountDisplay(this.playerIndex, MPStartAmount);
        mpAmount = MPStartAmount;
        indexOnBard =0;
        Translation = startingPos;
    }
    #region MOVMENT
    internal void MoveTo(int tPos,Vector3 nTransform)
    {
        // current index is the index we are currently standing on
        // next transfom in the next position in our path
        // target index is our final destanation
        targetIdex = tPos;
        nextTransform =   new Vector3(nTransform[0],constY,nTransform[2]);
        SetPhysicsProcess(true);
        GD.Print("Player: called to move to "+nextTransform);
    }

    //happens at a fixed rate, 60 times per second by default
    // we shut this off and on depending if we need to move or not.
    public override void _PhysicsProcess(float delta)
    {
        GD.Print("process");
        // when arrived at next position
        if(nextTransform.DistanceTo(Transform.origin)<0.5)
        {
            // set a signal to the game controller we have stopped moving
            indexOnBard++;
            SetPhysicsProcess(false);
            EmitSignal(nameof(arrived),indexOnBard,targetIdex);
        }
        else
        {
            direction = (nextTransform-Transform.origin).Normalized()*gameController.PlayerMovingSpeed;// the difference between the position we are currently in and where we want to go
            MoveAndSlide(direction);
        }
    }
    #endregion
    #region CURRENCY CONTROLLERS
    internal void AddMagicPoints(int amount)
    {
        this.mpAmount += amount;
        uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
    }
    internal void SubtractMagicPoints(int amount)
    {
        if (this.mpAmount - amount >= 0)
        {
            this.mpAmount -= amount;
            uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
        }
        else
            gameController.GameIsOver();
        

    }
    #endregion
}

using Godot;
using System;

public class Player : KinematicBody
{
    // MP is magic points
    [Export]
    private int Speed = 14,MPStartAmount = 300;
    [Export]
    private UIGameController uiController;
    [Export]
    private int playerIndex;
    private int mpAmount = 0; // magic points amount
    private int jail = 0;
    public int MpAmount { get { return this.mpAmount; }set { this.mpAmount = value; } }
    public int PlayerIndex { get { return this.playerIndex; } }
    public int Jail { get { return this.jail; } set { this.Jail = value; } }
    public override void _Ready()
    {
        uiController.UpdateAmountDisplay(this.playerIndex, MPStartAmount);

    }

    #region CURRENCY CONTROLLERS
    internal void PlayerAddCurrency(string amount)
    {
        int amountInt = int.Parse(amount);
        this.mpAmount += amountInt;
        uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
    }
    internal void PlayerSubtractCurrency(string amount)
    {
        int amountInt = int.Parse(amount);
        if (this.mpAmount - amountInt >= 0)
        {
            this.mpAmount -= amountInt;
            uiController.UpdateAmountDisplay(playerIndex, this.mpAmount);
        }
        else
        // game over 
    }
    #endregion
}

using Godot;
using System;

// this class will represent each of the locations in the board (each tile).
public class Card : Spatial
{
    public enum CardCatagory
    {
        property,reward, jail,fine
    }
    [Export]
    private CardCatagory catagory; // Each tile can be either a property, a reward, or a starting point.
    [Export]
    private int cost=100;
    [Export]
    private int ownedBy; // =0 if own by player 1 and =2 if owned by player 2, -1 if owned by no one
    [Export]
    internal string message;
    public CardCatagory Catagory { get { return this.catagory; } }
    public int Cost { get { return this.cost; } }
    public int OwnedBy { get { return this.ownedBy; } set { this.ownedBy = value; } }
    public  override void _Ready()
    {
        ownedBy = -1;
    }
}

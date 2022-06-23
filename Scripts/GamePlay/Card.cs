using Godot;
using System;

// this class will represent each of the locations in the board (each tile).
    public enum CardCategory
    {
        property,takenProperty,reward, jail,fine
    }
public class Card : Spatial
{
    [Export]
    private CardCategory category; // Each tile can be either a property, a reward, or a starting point.
    [Export]
    private int cost=100;
    [Export]
    private int ownedBy = -1; // =0 if own by player 1 and =2 if owned by player 2, -1 if owned by no one
    public int OwnedBy {get{return this.ownedBy;}}
    [Export]
    internal string message;
    public CardCategory Catagory { get { return this.category; } }
    public int Cost { get { return this.cost; } }
    public  override void _Ready()
    {
        ownedBy = -1;
    }

    internal void ChangeCardOwnership(int owner)
    {
        ownedBy = owner;
        category = CardCategory.takenProperty;
    }
}

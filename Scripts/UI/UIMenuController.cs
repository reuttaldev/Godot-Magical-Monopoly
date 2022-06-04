using Godot;
using System;

public class UIMenuController : Node
{
     [Signal]
    // this is connected to the script of the main game 
    public delegate void StartGame();
    public void OnClick()
    {
        // change to the board scene
        GD.Print("start button is being pressed ");
        GetTree().ChangeScene("res://Scenes/BoardScene.tscn");
        //EmitSignal("StartGame");
    }
}

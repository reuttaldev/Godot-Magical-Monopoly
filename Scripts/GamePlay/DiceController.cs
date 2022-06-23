using Godot;
using System;

public class DiceController : Control
{
    private TextureRect [] faces = new TextureRect[6];
    [Export]
    private float animationDuration = 2;// the number of seconds to show each face of the cube
    [Export]
    private float faceDuration = 0.3f;     // the number of seconds to show each face of the cube
    [Export]
    private float showResultDuration = 20;//the number of secs to display the result at the end
    private float time =0, lastFaceTime = 0;
    private int result = 0;
    private bool roll =false,showResult = false;// when this is true, the cube will be triggered to roll
    private GameController gameController;
    public override void _Ready()
    {
        // getting the faces textures
        for (int i = 0 ; i < 6 ; ++i)
            faces[i] = GetNode<TextureRect>((i + 1).ToString());
        gameController = (GameController)GetNode("../../");
    }
    
    // the result of the cube roll (1-6) is returned here
    internal int RollDice()
    {
        Visible = true;
        time = 0;
        lastFaceTime = faceDuration;
        roll = true;
        result = GetNewResult();
        return result;
    }
    private int GetNewResult()
    {
        return new Random().Next(1,6);
    }

    // will make visible only the face with the given index
    private void ShowFace(int onIndex)
    {
        for(int i=0;i<faces.Length;i++)
        {
            faces[i].Visible = (i == onIndex);

        }
    }
            
    // rolling dice animation
    // can be done using UI tweening as well
    public override void _Process(float delta)
    {
        if(roll)
        {
            // if we are whithin time limits of the animation
            if(animationDuration +delta>time)
            {
                // if its time to switch faces
                if(time + faceDuration>=lastFaceTime)
                {
                    lastFaceTime = time;
                    ShowFace(GetNewResult()-1);
                }
            }
            else
            {
                roll = false;
                // show the actual result
                ShowFace(result-1);
                showResult = true; 

                gameController.MoveCurrentPlayer(result);
     
            }
        // take some time to display the player the correct result
        }
        if(showResult)
        {
            // show the result for the remaining set time
            if(delta+showResultDuration<time)
            {
                Visible = false;
                showResult = false;      
            }
        }

        time+=delta;
    }


}

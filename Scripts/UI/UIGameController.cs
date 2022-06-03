using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGameController : Control
{
    private Label [] amountText=new Label[2];
    private Label nowPlayingTxt, bigPopUpTxt, smallPopUpText,roundCountText, jailTimerTxt;
    private Control popUpPanel;
    private Button continueButton;
    private string[] playersNames = {"Player 1","Player 2"};

    public override void _Ready()
    {
        // loading nodes from scene, unfortunately you cannot set them in the editor using serializable field (or export here) like in unity
        nowPlayingTxt = (Label)GetNode("UI/Top Display/Now playing text");
        bigPopUpTxt = (Label)GetNode("UI/Pop Up Panel/Purple BackGround/White Background/Big Text");
        smallPopUpText = (Label)GetNode("UI/Pop Up Panel/Purple BackGround/White Background/Description Text");
        roundCountText = (Label)GetNode("UI/Rounds Display/Label");
        jailTimerTxt = (Label)GetNode("UI/Jail Timer Text");
        continueButton = (Button)GetNode("UI/Pop Up Panel/Purple BackGround/White Background/Continue Button");
        popUpPanel = (Control)GetNode("UI/Pop Up Panel");
        amountText[0] = (Label)GetNode("UI/Top Display/Points Diplay/1/numbr of points");
        amountText[1] = (Label)GetNode("UI/Top Display/Points Diplay/2/numbr of points");


        nowPlayingTxt.Text = playersNames[0];
    }
    private void UpdatePopUpText(string smallTxt, string bigTxt)
    {
        OpenPopUpPanal();
        smallPopUpText.Text = smallTxt;
        bigPopUpTxt.Text = bigTxt;
    }
    internal void ShowRewardCardPopUp(int amount, string description)
    {
        UpdatePopUpText(description, "Earn " + amount.ToString() + " Magic Points!");
    }
    internal void ShowLosesCardPopUp(int amount, string description)
    {
        UpdatePopUpText(description,"You Lost " + amount.ToString() + " Magic Points!");
    }
    internal void ShowTakenProportyCardPopUp(int amount)
    {
        smallPopUpText.Text = "You landed on your opponent's property!";
        bigPopUpTxt.Text = "Lose " + amount.ToString() + " Magic Points!";
    }
    internal void ShowStartCardPopUp(int baseReward)
    {
        smallPopUpText.Text = "You landed on START and got a reward!";
        bigPopUpTxt.Text = "A new beginning is afoot. How exciting exciting! Earn " + baseReward.ToString() + "  Magic Points";
    }
    public void ShowJailPopUp(float timerLength)
    {
        GD.Print("Starting show jail pop up");
        if (timerLength != 0)
        {
            UpdatePopUpText( "You hear whispers in town that the witch hunters are getting closer and closer. You must go into hiding until it is safe. They are still burning witches alive like it's the sixties!", "");
            jailTimerTxt.Visible = true;
            bigPopUpTxt.Visible = false;
        }
    }
    internal void CloseJailPopUp()
    {
        jailTimerTxt.Visible = false;
        ClosePopUpPanal();
    }

    // this method activates UI pop up and changes the amount display
    internal void UpdateAmountDisplay(int playerIndex, int amount)
    {
        GD.Print("Updating amount display");
        amountText[playerIndex].Text = amount.ToString();
    }
    internal void ClosePopUpPanal()
    {
        popUpPanel.Visible = false;
    }
    internal void OpenPopUpPanal()
    {
        popUpPanel.Visible = true;
    }
    internal void ChangeRoundCountText(int roundCounter)
    {
        roundCountText.Text = "Round " + roundCounter.ToString();
    }
    internal void ChangeNowPlayingText(string name)
    {
        nowPlayingTxt.Text = name;
    }
    internal void GameOver(string winnerName)
    {
        UpdatePopUpText("You ran out of money",winnerName + " Won!" );
        continueButton.Visible = false;
    }
    
}

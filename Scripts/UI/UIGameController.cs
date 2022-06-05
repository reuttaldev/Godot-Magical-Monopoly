using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGameController : Control
{
    private Label [] amountText=new Label[2];
    private Label nowPlayingTxt, bigPopUpTxt, smallPopUpText,roundCountText, jailTimerTxt;
    private Control popUpPanel;
    private Button continueButton, yesButton, noButton;
    private GameController gameController;

    public override void _Ready()
    {
        //loading nodes from scene, unfortunately you cannot set them in the editor using serializable field (or export here) like in unity
        // if a node is a scene and you mark it as edible children, it doesnt load it correctly
        bigPopUpTxt = GetNode<Label>("Popup/Purple BackGround/White Background/Big Txt");
        smallPopUpText = GetNode<Label>("Popup/Purple BackGround/White Background/Description Text");
        roundCountText = (Godot.Label)GetNode("Rounds Display/Label");
        jailTimerTxt = GetNode<Label>("Popup/Purple BackGround/White Background/Jail Timer Text");
        noButton = GetNode<Button>("Popup/Purple BackGround/White Background/No Button");
        yesButton  = GetNode<Button>("Popup/Purple BackGround/White Background/Yes Button");
        continueButton = GetNode<Button>("Popup/Purple BackGround/White Background/Continue Button");
        popUpPanel = GetNode<Control>("Popup");
        amountText[0] = GetNode<Label>("Top Display/Points Diplay/1/numbr of points");
        amountText[1] = GetNode<Label>("Top Display/Points Diplay/2/numbr of points");
        nowPlayingTxt = GetNode<Label>("Top Display/Playing Now Text/Label");
        nowPlayingTxt.Text = "Player 1";
        gameController = (GameController)GetNode("../");
        yesButton.Connect("pressed",gameController, "YesButton");
        noButton.Connect("pressed",gameController, "NoButton");
        continueButton.Connect("pressed",gameController, "ContinueButton");
    }
    internal void DisplayPopUp(CardCatagory cat,string description,int amount)
    {
        string s="", b="";
        switch (cat)
        {
            case CardCatagory.property:
                s = "a";
                b ="a" ;
                break;
            case CardCatagory.takenProperty:
                s = "You landed on your opponent's property!";
                b = "Lost " + amount.ToString() + " Magic Points!";
                break;
            case CardCatagory.reward:
                s = description;
                b = "Earned " + amount.ToString() + " Magic Points!";
                break;
            case CardCatagory.fine:
                s = description;
                b = "Lost " + amount.ToString() + " Magic Points!";
                break;
                
        }
        ChangePopUpText(s,b);

    }
    private void ChangePopUpText(string smallTxt, string bigTxt)
    {
        OpenPopUpPanal();
        smallPopUpText.Text = smallTxt;
        bigPopUpTxt.Text = bigTxt;
    }
    public void ShowJailPopUp(float timerLength)
    {
        GD.Print("Starting show jail pop up");
        if (timerLength != 0)
        {
            ChangePopUpText( "You hear whispers in town that the witch hunters are getting closer and closer. You must go into hiding until it is safe. They are still burning witches alive like it's the sixties!", "");
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
        ChangePopUpText("You ran out of money",winnerName + " Won!" );
        continueButton.Visible = false;
    }
    
}

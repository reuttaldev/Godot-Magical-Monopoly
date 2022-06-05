using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGameController : Control
{
    private Label [] amountText=new Label[2];
    private Label nowPlayingTxt, bigPopUpTxt, smallPopUpText,roundCountText,cantBuyText;
    private Popup popUpPanel;
    private Button continueButton, yesButton, noButton;
    private GameController gameController;

    public override void _Ready()
    {
        //loading nodes from scene, unfortunately you cannot set them in the editor using serializable field (or export here) like in unity
        // if a node is a scene and you mark it as edible children, it doesnt load it correctly
        bigPopUpTxt = GetNode<Label>("Popup/Purple BackGround/White Background/Big Txt");
        smallPopUpText = GetNode<Label>("Popup/Purple BackGround/White Background/Description Text");
        roundCountText = (Godot.Label)GetNode("Rounds Display/Label");
        noButton = GetNode<Button>("Popup/Purple BackGround/White Background/No Button");
        yesButton  = GetNode<Button>("Popup/Purple BackGround/White Background/Yes Button");
        continueButton = GetNode<Button>("Popup/Purple BackGround/White Background/Continue Button");
        popUpPanel = GetNode<Popup>("Popup");
        amountText[0] = GetNode<Label>("Top Display/Points Diplay/1/numbr of points");
        amountText[1] = GetNode<Label>("Top Display/Points Diplay/2/numbr of points");
        nowPlayingTxt = GetNode<Label>("Top Display/Playing Now Text/Label");
        cantBuyText  = GetNode<Label>("Popup/Purple BackGround/White Background/Cant Buy Text");
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
                s = description;
                b ="Do you want to buy this special item?" ;
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
            case CardCatagory.jail:
                s = description; //"You hear whispers in town that the witch hunters are getting closer and closer. You must go into hiding until it is safe. They are still burning witches alive like it's the sixties!"
                b = "Wait for 2 turns!";
                break;
                
        }
        ChangePopUpText(s,b);
        OpenPopUpPanal(cat == CardCatagory.property);
    }
    private void ChangePopUpText(string smallTxt, string bigTxt)
    {
        smallPopUpText.Text = smallTxt;
        bigPopUpTxt.Text = bigTxt;
    }

    // this method activates UI pop up and changes the amount display
    internal void UpdateAmountDisplay(int playerIndex, int amount)
    {
        GD.Print("Updating amount display for player "+playerIndex);
        amountText[playerIndex].Text = amount.ToString();
    }
    internal void ClosePopUpPanal()
    {
        cantBuyText.Visible = false;
        popUpPanel.Hide();
    }
    internal void OpenPopUpPanal(bool freeProperty)
    {
        if(freeProperty)
        {
            yesButton.Visible = true;
            noButton.Visible = true;
            continueButton.Visible = false;
        }
        else
        {
            continueButton.Visible = true;
            yesButton.Visible = false;
            noButton.Visible = false;
        }
        popUpPanel.Show();
    }
    internal void CantBuyText()
    {
        cantBuyText.Visible = true;
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
        GD.Print("ui game over over");
        ChangePopUpText("You ran out of money",winnerName + " Won!" );
        OpenPopUpPanal(false);
        
        continueButton.Visible = false;
        yesButton.Visible = false;
        noButton.Visible = false;
    }
    
}

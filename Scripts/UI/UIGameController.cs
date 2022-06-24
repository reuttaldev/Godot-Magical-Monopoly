using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public class UIGameController : Control
{
    private Label [] amountText=new Label[2];
    private Label nowPlayingTxt, bigPopUpTxt, smallPopUpText,roundCountText,cantBuyText;
    private Popup popUpPanel;
    private Button continueButton, yesButton, noButton,closeButton;
    private GameController gameController;
    private  Sprite3D[] cardSprites;
    private Color [] cardTint = new Color[]{ new Color( 0.51f, 0.3f, 0.84f, 1f ),new Color( 0.4f, 0.64f, 1, 1f )};

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
        closeButton = GetNode<Button>("Popup/Purple BackGround/White Background/Close Game Button");
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
        closeButton.Connect("pressed",this, "CloseGameButton");

        cardSprites = new Sprite3D[GameController.NUM_OF_CARDS];
        var cardTemp = GetTree().GetNodesInGroup("Card Sprite");
        for (int i = 0; i < cardTemp.Count; i++)
        {
            cardSprites[i] = (Sprite3D)cardTemp[i];
        }
    }
    internal void DisplayPopUp(CardCategory cat,string description,int amount)
    {
        string s="", b="";
        switch (cat)
        {
            case CardCategory.property:
                s = description;
                b ="Do you want to buy this special item for \n"+amount +" Magic Points?" ;
                break;
            case CardCategory.takenProperty:
                s = "You landed on your opponent's property!";
                b = "Lost " + amount.ToString() + " Magic Points!";
                break;
            case CardCategory.reward:
                s = description;
                b = "Earned " + amount.ToString() + " Magic Points!";
                break;
            case CardCategory.fine:
                s = description;
                b = "Lost " + amount.ToString() + " Magic Points!";
                break;
            case CardCategory.jail:
                s = description; //"You hear whispers in town that the witch hunters are getting closer and closer. You must go into hiding until it is safe. They are still burning witches alive like it's the sixties!"
                b = "Wait for 2 turns!";
                break;
                
        }
        ChangePopUpText(s,b);
        OpenPopUpPanal(cat == CardCategory.property);
    }
    private void ChangePopUpText(string smallTxt, string bigTxt)
    {
        smallPopUpText.Text = smallTxt;
        bigPopUpTxt.Text = bigTxt;
    }
    internal void ChangeCardColor(int cardIndex, int playerIndex)
    {
        //Texture t = (Texture)GD.Load(""+(player+1).ToString());
        cardSprites[cardIndex].Modulate = cardTint[playerIndex];
    }
    // this method activates UI pop up and changes the amount display
    internal void UpdateAmountDisplay(int playerIndex, int amount)
    {
        amountText[playerIndex].Text = amount.ToString();
    }
    internal void ClosePopUpPanel()
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
        closeButton.Visible = true;
    }

    void CloseGameButton()
    {
        GetTree().Quit();
    }
    
}

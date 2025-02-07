using Godot;
using System;
using System.Collections.Generic;

public partial class TutorialScreen : Control
{
    private static Dictionary<int, string> textContent = new Dictionary<int, string>
    {
        {1, "This is the map where you have to travel from node to node"},
        {2, "You start from the leftmost node and slowly move to final 'Boss' node where you have to defeat boss"},
        {3, "You move on the map by clicking on the node connected to your currently standing node"},
        {4, "There is also elite enemies with stronger monsters. So choose path wisely"},
        {5, "Once you click on enemy or elite enemy node, it is going to start battle"},
        {6, "Below you will see your hand. Drag the card on characters to apply effect"},
        {7, "Hover over card to see effect description"},
        {8, "On left draw pile. At the beginning of turn you will get new set of card to your hand"},
        {9, "On the right discard pile. Consumed cards will move there and get shuffled back to draw pile"},
        {10, "On upper-left side is your mana pool. Each card has mana cost so be careful"},
        {11, "Your goal is to defeat them. Good luck"}
    };
    
    private static Dictionary<int, Vector2> textPosition = new Dictionary<int, Vector2>
    {
        {1, new Vector2(20, 260)},
        {2, new Vector2(20, 260)},
        {3, new Vector2(20, 0)},
        {4, new Vector2(20, 260)},
        {5, new Vector2(20, 130)},
        {6, new Vector2(20, 0)},
        {7, new Vector2(20, 0)},
        {8, new Vector2(20, 0)},
        {9, new Vector2(20, 0)},
        {10, new Vector2(20, 260)},
        {11, new Vector2(20, 260)}
    };
    
    [Export] private Button skipButton;
    [Export] private Label text;
    [Export] private Sprite2D sprite;

    private int curPage = 0;

    public override void _Ready()
    {
        skipButton.Pressed += ClosePage;
    }

    public void ChangePage()
    {
        if (curPage == 11)
        {
            ClosePage();
            return;
        }
        
        if (curPage == 0)
            skipButton.Hide();
        
        curPage++;
        sprite.Texture = GD.Load<Texture2D>("res://Sprites/Tutorial/" + curPage + ".png");
        text.Text = textContent[curPage];
        text.Position = textPosition[curPage];
    }

    public void ClosePage()
    {
        GameManager gameManager = (GameManager) GetParent();
        gameManager.CloseTutorial();
    }
}

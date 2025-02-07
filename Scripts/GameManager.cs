using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using Godot.Collections;

public partial class GameManager : Node2D
{
	[Export] private PackedScene mainMenuScene;
	[Export] private PackedScene pauseMenuScene;
	[Export] private PackedScene mapScene;
	[Export] private PackedScene battleScene;
	[Export] private PackedScene tutorialScreenScene;

	private MainMenu mainMenu;
	private PauseMenu pauseMenu;
	private Map map;
	private BattleManager battleManager;
	private TutorialScreen tutorialScreen;
	
	public Array<CardData> PlayerCards = new();
	
	public override void _Ready()
	{
		// Load master card
		CardDatabase cardDatabase = ResourceLoader.Load<CardDatabase>("res://Data/card_database.tres");
		if (cardDatabase != null)
			PlayerCards = cardDatabase.allCards;
		else
			Debug.Print("Couldn't load card database");
		
		Debug.Print("Player Cards Loaded: " + PlayerCards.Count);
		
		// Instance, Add and Hide the pause menu
		pauseMenu = (PauseMenu) pauseMenuScene.Instantiate();
		AddChild(pauseMenu);
		pauseMenu.Visible = false;
		pauseMenu.ProcessMode = ProcessModeEnum.WhenPaused;
		pauseMenu.ResumeGame += UnpauseGame;
		pauseMenu.ResolutionSelected += ChangeResolution;
		
		// Instantiate and Add MainMenu
		mainMenu = (MainMenu) mainMenuScene.Instantiate();
		AddChild(mainMenu);
		if (GameSettings.watchedTutorial)
			mainMenu.startButton.Pressed += StartGame;
		else
			mainMenu.startButton.Pressed += OpenTutorial;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))  // e.g. ESC
		{
			if (GetTree().Paused)
				UnpauseGame();
			else
				PauseGame();
		}
		
		if (@event is InputEventKey eventKey && eventKey.Pressed && eventKey.Keycode == Key.Space)
		{
			tutorialScreen.ChangePage();
		}
	}

	private void OpenTutorial()
	{
		mainMenu.Hide();
		tutorialScreen = (TutorialScreen)tutorialScreenScene.Instantiate();
		AddChild(tutorialScreen);
	}

	public void CloseTutorial()
	{
		tutorialScreen.QueueFree();
		RemoveChild(tutorialScreen);
		StartGame();
	}

	private void PauseGame()
	{
		pauseMenu.Visible = true;
		GetTree().Paused = true;
	}

	private void UnpauseGame()
	{
		GetTree().Paused = false;
		pauseMenu.Visible = false;
	}

	private void ChangeResolution(long index)
	{
		switch (index)
		{
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetSize(new Vector2I(640, 360));
				break;
			case 1:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				DisplayServer.WindowSetSize(new Vector2I(1280, 720));
				break;
			case 2:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				DisplayServer.WindowSetSize(new Vector2I(1920, 1080));
				break;
		}
	}

	private List<EntityData> loadPlayerTeam()
	{
		List<EntityData> playerTeam = new List<EntityData>();
		
		EntityData player = new EntityData("Player", 20);
		playerTeam.Add(player);
		return playerTeam;
	}

	private void StartGame()
	{
		if (GetChildren().Contains(mainMenu))
			RemoveChild(mainMenu);
		
		map = (Map) mapScene.Instantiate();
		AddChild(map);
		map.NodeClicked += StartBattle;
	}

	private void StartBattle(MapNode node)
	{
		RemoveChild(map);
		
		battleManager = (BattleManager) battleScene.Instantiate();
		battleManager.Setup(PlayerCards, loadPlayerTeam(), node.enemies);
		AddChild(battleManager);
		battleManager.BattleWon += FinishBattle;
	}

	private void FinishBattle()
	{
		RemoveChild(battleManager);
		battleManager.QueueFree();
		battleManager = null;
		
		AddChild(map);
	}
}



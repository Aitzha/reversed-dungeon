using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using FileAccess = Godot.FileAccess;

public partial class GameManager : Node2D
{
	[Export] private PackedScene mainMenuScene;
	[Export] private PackedScene pauseMenuScene;
	[Export] private PackedScene mapScene;
	[Export] private PackedScene battleScene;

	private MainMenu mainMenu;
	private PauseMenuScript pauseMenu;
	private Map map;
	private BattleManager battleManager;
	
	public List<CardData> PlayerCards = new();
	
	
	public override void _Ready()
	{
		// Load master card
		PlayerCards = CardLoader.LoadCards();
		Debug.Print("Player Cards Loaded: " + PlayerCards.Count);
		
		// Instance the pause menu, script and add to Main
		pauseMenu = (PauseMenuScript)pauseMenuScene.Instantiate();
		AddChild(pauseMenu);
		pauseMenu.Visible = false;
		pauseMenu.ProcessMode = ProcessModeEnum.WhenPaused;
		pauseMenu.ResumeGame += UnpauseGame;
		pauseMenu.ResolutionSelected += ChangeResolution;
		
		// Instantiate MainMenu
		mainMenu = (MainMenu) mainMenuScene.Instantiate();
		AddChild(mainMenu);
		mainMenu.startButton.Pressed += StartGame;
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

	private List<EntityData> loadEnemyTeam()
	{
		List<EntityData> enemyTeam = new List<EntityData>();

		EntityData enemy1 = new EntityData("Enemy#1", 20);
		EntityData enemy2 = new EntityData("Enemy#2", 20);
		EntityData enemy3 = new EntityData("Enemy#3", 20);
		enemyTeam.Add(enemy1);
		enemyTeam.Add(enemy2);
		enemyTeam.Add(enemy3);
		return enemyTeam;
	}

	private void StartGame()
	{
		RemoveChild(mainMenu);

		map = (Map)mapScene.Instantiate();
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

public static class CardLoader
{
	public static List<CardData> LoadCards()
	{
		string json = FileAccess.GetFileAsString("res://Data/master_cards.json");

		List<CardData> cards = JsonSerializer.Deserialize<List<CardData>>(json);
		return cards;
	}
}



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public partial class GameManager : Node2D
{
	[Export] public PackedScene PauseMenuScene;
	[Export] public PackedScene BattleScene;
	
	private Control _pauseMenu;
	private PauseMenuScript _pauseMenuScript;
	
	public List<CardData> PlayerCards = new();
	
	
	public override void _Ready()
	{
		// Load master cards
		String filePath = ProjectSettings.GlobalizePath("res://Data/master_cards.json");
		
		PlayerCards = CardLoader.LoadCards(filePath);
		Debug.Print("Player Cards Loaded: " + PlayerCards.Count);

		
		BattleManager battleScene = (BattleManager) BattleScene.Instantiate();
		battleScene.Setup(PlayerCards, loadPlayerTeam(), loadEnemyTeam());
		AddChild(battleScene);
		
		
		// Instance the pause menu, script and add to Main
		_pauseMenu = (Control)PauseMenuScene.Instantiate();
		_pauseMenuScript = _pauseMenu as PauseMenuScript;
		AddChild(_pauseMenu);
		
		// Hide it initially
		_pauseMenu.Visible = false;
		
		// Change process mode to "When Paused"
		_pauseMenu.ProcessMode = ProcessModeEnum.WhenPaused;

		// Add function to the signals
		if (_pauseMenuScript != null)
		{
			_pauseMenuScript.ResumeGame += UnpauseGame;
			_pauseMenuScript.ResolutionSelected += ChangeResolution;
		}
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
		_pauseMenu.Visible = true;
		GetTree().Paused = true;
	}

	private void UnpauseGame()
	{
		GetTree().Paused = false;
		_pauseMenu.Visible = false;
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
}

public static class CardLoader
{
	public static List<CardData> LoadCards(string filePath)
	{
		string json = File.ReadAllText(filePath);

		List<CardData> cards = JsonSerializer.Deserialize<List<CardData>>(json);
		return cards;
	}
}



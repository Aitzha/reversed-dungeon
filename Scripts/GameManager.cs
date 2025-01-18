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
	
	public Dictionary<String, CardData> MasterCards = new Dictionary<String, CardData>();
	public List<CardData> PlayerCards = new List<CardData>();
	
	
	public override void _Ready()
	{
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
		
		// Load master cards
		String filePath = ProjectSettings.GlobalizePath("res://Data/master_cards.json");
		Debug.Print(filePath);
		MasterCards = JsonConverter.DeserializeCardData(filePath);
		Debug.Print("Master Cards Loaded: " + MasterCards.Count);
		
		// TODO: Load Player cards (Replace with actual player cards load later)
		foreach (CardData card in MasterCards.Values)
		{
			PlayerCards.Add(card);
		}
		
		Debug.Print("Player Cards Loaded: " + PlayerCards.Count);

		var battleScene = BattleScene.Instantiate();
		Control interfaceControl = battleScene.GetNode<Control>("Interface");
		BattleInterface battleInterface = interfaceControl as BattleInterface;
		battleInterface.playerCards = PlayerCards;
		AddChild(battleScene);
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
}

public class JsonConverter
{
	public static Dictionary<String, CardData> DeserializeCardData(String filePath)
	{
		if (File.Exists(filePath))
		{
			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				Converters = { new JsonStringEnumConverter() }
			};
			
			string text = File.ReadAllText(filePath);
			var dict = JsonSerializer.Deserialize<Dictionary<string, CardData>>(text, options);
			if (dict == null)
				return null;
			
			foreach (var kvp in dict)
			{
				kvp.Value.Id = kvp.Key;
			}

			return dict;
		}

		Debug.Print("No File Found");
		return null;
	}
}



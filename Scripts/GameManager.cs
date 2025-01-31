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
		// Load master cards
		String filePath = ProjectSettings.GlobalizePath("res://Data/master_cards.json");
		Debug.Print(filePath);
		// MasterCards = JsonConverter.DeserializeCardData(filePath);
		Debug.Print("Master Cards Loaded: " + MasterCards.Count);
		
		// TODO: Load Player cards (Replace with actual player cards load later)
		PlayerCards = CardLoader.LoadCards(filePath);
		// foreach (CardData card in MasterCards.Values)
		// {
		// 	PlayerCards.Add(card);
		// }
		
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

public class BaseEffectListConverter : JsonConverter<List<BaseEffect>>
{
    public override List<BaseEffect> Read(ref Utf8JsonReader reader, 
                                          Type typeToConvert,
                                          JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Expected start of array for effects.");

        var list = new List<BaseEffect>();

        // Advance into the array
        reader.Read();

        // Loop until we hit the end of the array
        while (reader.TokenType != JsonTokenType.EndArray)
        {
            // Parse each object in the array
            using var doc = JsonDocument.ParseValue(ref reader);
            var element = doc.RootElement;

            // We look for "effectType" to decide which subclass to instantiate
            if (!element.TryGetProperty("effectType", out var effectTypeProp))
                throw new JsonException("Missing 'effectType' property on effect object.");

            string effectType = effectTypeProp.GetString();
            BaseEffect effect;
            switch (effectType)
            {
                case "InstantEffect":
                    effect = JsonSerializer.Deserialize<InstantEffect>(element.GetRawText(), options);
                    break;
                case "ContinuousEffect":
                    effect = JsonSerializer.Deserialize<ContinuousEffect>(element.GetRawText(), options);
                    break;
                case "BuffDebuffEffect":
                    effect = JsonSerializer.Deserialize<BuffDebuffEffect>(element.GetRawText(), options);
                    break;
                default:
                    throw new JsonException($"Unknown effectType '{effectType}'.");
            }

            list.Add(effect);

            // Move reader to next array element
            reader.Read();
        }

        return list;
    }

    public override void Write(Utf8JsonWriter writer, 
                               List<BaseEffect> value, 
                               JsonSerializerOptions options)
    {
        // If you need to serialize back to JSON, implement similarly:
        // 1) Write StartArray
        // 2) For each BaseEffect in 'value', check its runtime type and call
        //    JsonSerializer.Serialize(...).
        // 3) Write EndArray
        throw new NotImplementedException();
    }
}


public static class CardLoader
{
	public static List<CardData> LoadCards(string filePath)
	{
		string json = File.ReadAllText(filePath);

		var options = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			// to allow comments or trailing commas if needed:
			ReadCommentHandling = JsonCommentHandling.Skip,
			AllowTrailingCommas = true
		};
		options.Converters.Add(new BaseEffectListConverter());
		options.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));

		// If your JSON top-level is { "cards": [...] }
		var cards = JsonSerializer.Deserialize<List<CardData>>(json, options);
		return cards;
	}
}



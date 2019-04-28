using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DeathMessageManager : Node2D {
    private GameMaster gameMaster;
    private VBoxContainer messageContainer;
    private PackedScene deathLabel;

    private Node2D fxContainer;
    private PackedScene fx;

    private Node2D soundContainer;
    private PackedScene soundEffect;

    private List<string> names = new List<string>();
    private List<string> messages = new List<string>();
    private List<string> customMessages = new List<string>();

    private Godot.Collections.Dictionary<Label, long> deathMessages = new Godot.Collections.Dictionary<Label, long>();
    
    private string[] effectNames = { "death_1.png", "death_2.png" };
    private List<Texture> effects = new List<Texture>();

    private string[] soundEffectsNames = {"dying_1.wav", "dying_2.wav", "dying_3.wav", "dying_4.wav"};
    private List<AudioStream> soundEffects = new List<AudioStream>();

    public override void _Ready() {
        gameMaster = GetNode<GameMaster>("..");
        messageContainer = GetNode<VBoxContainer>("/root/MainScene/CanvasLayer/MessageContainer");
        deathLabel = GD.Load<PackedScene>("DeathMessage.tscn");

        fxContainer = GetNode<Node2D>("/root/MainScene/FxContainer");
        fx = GD.Load<PackedScene>("Fx.tscn");

        soundContainer = GetNode<Node2D>("/root/MainScene/SoundEffects");
        soundEffect = GD.Load<PackedScene>("SoundEffect.tscn");

        names = ReadFile("res://Data/Names.txt");
        messages = ReadFile("res://Data/Messages.txt");
        customMessages = ReadFile("res://Data/Custom.txt");
        
        foreach (var effectName in effectNames) {
            var texture = GD.Load<Texture>($"res://Fx/{effectName}");
            effects.Add(texture);
        }

        foreach (var soundName in soundEffectsNames) {
            var soundEffect = GD.Load<AudioStream>($"res://Sounds/{soundName}");
            soundEffects.Add(soundEffect);
        }
    }

    private static List<string> ReadFile(string path) {
        var result = new List<string>();
        var file = new File();
        file.Open(path, (int) File.ModeFlags.Read);
        var index = 1;
        while (!file.EofReached()) {
            var line = file.GetLine();
            result.Add(line.Trim());
            index++;
        }

        file.Close();
        return result;
    }

    public override void _Process(float delta) {
        foreach (var pair in deathMessages.ToList()) {
            var label = pair.Key;
            var time = pair.Value;
            if (OS.GetTicksMsec() - time > 5000) {
                label.QueueFree();
                deathMessages.Remove(label);
            }
        }
    }

    public void AddDeaths(int amount) {
        if (amount <= 0) return;

        SpawnParticles(amount > 3 ? 3 : (int) GD.RandRange(1, amount));

        if (GD.Randf() > .9f) {
            var effect = (SoundEffect) soundEffect.Instance();
            effect.Stream = soundEffects[(int) GD.RandRange(0, soundEffects.Count)];
            soundContainer.AddChild(effect);
            effect.Play();
        }

        if (deathMessages.Count > 5) {
            return;
        }

        var death = (Label) deathLabel.Instance();
        GD.Randomize();
        death.Text = $"#{gameMaster.TotalDeaths} {GetRandomName()} died by '{GetRandomMessage()}'";
        messageContainer.AddChild(death);
        deathMessages.Add(death, OS.GetTicksMsec());
    }

    private void SpawnParticles(int amount) {
        var buildings = gameMaster.placedBuildings.ToList();
        for (var i = 0; i < amount; i++) {
            if (buildings.Count == 0) break;

            var randomBuildingIndex = (int) GD.RandRange(0, buildings.Count);
            var randomBuilding = buildings[randomBuildingIndex];
            buildings.RemoveAt(randomBuildingIndex);

            var effect = (Fx) fx.Instance();
            effect.Position = randomBuilding * 16;
            effect.Texture = effects[(int) GD.RandRange(0, effects.Count)];
            fxContainer.AddChild(effect);
        }
    }

    private string GetRandomName() {
        return names[(int) GD.RandRange(0, names.Count)];
    }

    private string GetRandomMessage() {
        return messages[(int) GD.RandRange(0, messages.Count)];
    }

    private string GetRandomCustom() {
        return customMessages[(int) GD.RandRange(0, customMessages.Count)];
    }
}
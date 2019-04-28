using Godot;
using System;

public class MusicManager : Node2D {

    [Export(PropertyHint.Flags)] public bool isEnabled = true;
    
    private AudioStreamPlayer music;

    private string[] songs = {"song_1.ogg", "song_2.ogg"};
    private int current = -1;

    public override void _Ready() {
        music = GetNode<AudioStreamPlayer>("Music");
        if (!isEnabled) return;
        
        OnCurrentFinished();
    }

    // Event
    public void OnCurrentFinished() {
        current++;
        if (current > songs.Length - 1) {
            current = 0;
        }

        var next = GD.Load<AudioStream>($"res://Music/{songs[current]}");
        music.Stop();
        music.Stream = next;
        music.Play();
    }
    
}
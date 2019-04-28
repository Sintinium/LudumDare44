using Godot;
using System;

public class SoundEffect : AudioStreamPlayer {

    // Event
    public void OnFinished() {
        QueueFree();
    }
    
}
using Godot;
using System;

public class Fx : Sprite {
    private long spawnTime;
    
    public override void _Ready() {
        spawnTime = OS.GetTicksMsec();
    }

    public override void _Process(float delta) {
        Position = new Vector2(Position.x, Position.y - 10 * delta);

        if (OS.GetTicksMsec() - spawnTime > 1000) {
            QueueFree();
        }
    }
}
using Godot;
using System;

public class Camera : Camera2D {
    public override void _Ready() {
        SetProcess(true);
    }

    public override void _Process(float delta) {
        Align();
    }
}
using Godot;
using System;

public class GameOver : Node2D {
    public void Again() {
        GetTree().ChangeScene("res://MainScene.tscn");
    }

    public void MainMenu() {
        GetTree().ChangeScene("res://MainMenu.tscn");
    }
}
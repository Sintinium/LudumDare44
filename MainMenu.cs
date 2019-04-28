using Godot;
using System;

public class MainMenu : Node2D {

    private Control buttons;
    private Control help;

    public override void _Ready() {
        buttons = GetNode<Control>("Buttons");
        help = GetNode<Control>("Help");
    }

    public void OnPlay() {
        GetTree().ChangeScene("res://MainScene.tscn");
    }

    public void OnHelp() {
        buttons.Visible = false;
        help.Visible = true;
    }

    public void OnExit() {
        GetTree().Quit();
    }

    public void ExitHelp() {
        buttons.Visible = true;
        help.Visible = false;
    }
    
}
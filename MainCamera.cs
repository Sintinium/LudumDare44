using Godot;
using System;

public class MainCamera : Node2D {

    [Export(PropertyHint.Length)] private float SPEED = 100f;
    
    private Vector2 targetPosition;

    public override void _Ready() {
        targetPosition = Position;
    }

    public override void _Process(float delta) {
        var x = 0f;
        var y = 0f;
        var setSpeed = SPEED;
        if (Input.IsKeyPressed(16777237)) { // Shift keycode
            setSpeed = SPEED * 3;
        }
        if (Input.IsActionPressed("ui_left")) {
            x = -setSpeed;
        } else if (Input.IsActionPressed("ui_right")) {
            x = setSpeed;
        }

        if (Input.IsActionPressed("ui_up")) {
            y = -setSpeed;
        } else if (Input.IsActionPressed("ui_down")) {
            y = setSpeed;
        }

        if (x != 0f || y != 0f) {
            targetPosition.x += x * delta;
            targetPosition.y += y * delta;
        }
//        Position = new Vector2(Position.x + x * delta, Position.y + y * delta);
        var newX = Mathf.Lerp(Position.x, targetPosition.x, 10f * delta);
        var newY = Mathf.Lerp(Position.y, targetPosition.y, 10f * delta);
        
        Position = new Vector2(newX, newY);
        var vwdith = GetViewport().GetVisibleRect().Size.x;
        var vheight = GetViewport().GetVisibleRect().Size.y;
        if (Position.x + vwdith > 16 * 48) {
            Position = new Vector2(16 * 48 - vwdith, Position.y);
            targetPosition.x = Position.x;
        } else if (Position.x < 0) {
            Position = new Vector2(0f, Position.y);
            targetPosition.x = Position.x;
        }
        if (Position.y + vheight > 16 * 48) {
            Position = new Vector2(Position.x, 16 * 48 - vheight);
            targetPosition.y = Position.y;
        } else if (Position.y < 0) {
            Position = new Vector2(Position.x, 0f);
            targetPosition.y = Position.y;
        }
    }
    
}
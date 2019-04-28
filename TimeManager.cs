using Godot;
using System;

public class TimeManager : Node2D {
    private GameMaster gameMaster;
    private DemandManager demandManager;

    public int lastState = 1;
    public int state { get; private set; } = -1;

    public const int PAUSE = 0;
    public const int PLAY = 1;
    public const int FAST = 2;
    public const int DOUBLE = 3;

    private TextureButton buttonPause;
    private TextureButton buttonPlay;
    private TextureButton buttonFast;
    private TextureButton buttonDouble;

    private Color colorOff = Colors.White;
    private Color colorOn = Colors.Yellow;
    
    private long lastTick = 0L;

    public override void _Ready() {
        gameMaster = GetNode<GameMaster>("..");
        demandManager = GetNode<DemandManager>("../DemandManager");

        var container = "/root/MainScene/CanvasLayer/HBoxContainer/";
        buttonPause = GetNode<TextureButton>(container + "Pause");
        buttonPlay = GetNode<TextureButton>(container + "Play");
        buttonFast = GetNode<TextureButton>(container + "Fast");
        buttonDouble = GetNode<TextureButton>(container + "Double");
        OnPlayPressed();
    }

    public override void _Process(float delta) {
        var currentTime = OS.GetTicksMsec();
        var tickLength = getTickLength();
        if (tickLength == -1) {
            lastTick = OS.GetTicksMsec();
            return;
        }
        
        if (currentTime - lastTick > tickLength) {
            demandManager.RecalculateDemand();
            gameMaster.TickTime();
            lastTick = currentTime;
        }
    }

    private long getTickLength() {
        var time = 0;
        switch (state) {
            case PAUSE:
                time = -1;
                break;
            case PLAY:
                time = 1000;
                break;
            case FAST:
                time = 500;
                break;
            case DOUBLE:
                time = 100;
                break;
        }

        return time;
    }

    // Event
    public void OnPausePressed() {
        state = PAUSE;
        ResetButtons();
        buttonPause.Modulate = colorOn;
    }

    // Event
    public void OnPlayPressed() {
        state = PLAY;
        ResetButtons();
        buttonPlay.Modulate = colorOn;
    }

    // Event
    public void OnFastPressed() {
        state = FAST;
        ResetButtons();
        buttonFast.Modulate = colorOn;
    }

    // Event
    public void OnDoublePressed() {
        state = DOUBLE;
        ResetButtons();
        buttonDouble.Modulate = colorOn;
    }

    private void ResetButtons() {
        buttonPause.Modulate = colorOff;
        buttonPlay.Modulate = colorOff;
        buttonFast.Modulate = colorOff;
        buttonDouble.Modulate = colorOff;
    }

    private void UpdateCurrentState() {
        switch (state) {
            case PAUSE:
                OnPausePressed();
                break;
            case PLAY:
                OnPlayPressed();
                break;
            case FAST:
                OnFastPressed();
                break;
            case DOUBLE:
                OnDoublePressed();
                break;
        }
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (!(@event is InputEventKey)) {
            return;
        }

        var inputEvent = (InputEventKey) @event;
        if (!inputEvent.Pressed) return;
        switch (inputEvent.Scancode) {
            case (int) KeyList.Minus:
                state--;
                break;
            case (int) KeyList.Comma:
                state--;
                break;
            case (int) KeyList.Equal:
                state++;
                break;
            case (int) KeyList.Period:
                state++;
                break;
            case (int) KeyList.Space:
                if (state == PAUSE) {
                    state = lastState;
                }
                else {
                    lastState = state;
                    state = PAUSE;
                }
                break;
        }

        if (state > DOUBLE) {
            state = DOUBLE;
        } else if (state < PAUSE) {
            state = PAUSE;
        }
        
        UpdateCurrentState();
    }
}
using Godot;
using System;
using System.Collections.Generic;
using System.Text;

public class GameMaster : Node2D {
    public int TotalDeaths { get; private set; }
    public static GameMaster Instance { get; private set; }
    public BuildingTypes.Building selectedBuilding = null;

    private Label time;
    private Label populationLabel;
    public TileMap tileMap;
    private HBoxContainer buildingButtons;
    public int Population { get; private set; } = 100000;
    public int RoadCount { get; private set; }

    public readonly Dictionary<BuildingTypes.Building, int> buildings = new Dictionary<BuildingTypes.Building, int>();
    private DemandManager demandManager;
    private DeathMessageManager deathMessageManager;

    // Only buildings not roads for FX to make life easier
    public List<Vector2> placedBuildings = new List<Vector2>();

    private int currentTime;
    private long dieTime;

    public override void _Ready() {
        Instance = this;

        demandManager = GetNode<DemandManager>("DemandManager");
        deathMessageManager = GetNode<DeathMessageManager>("DeathMessageManager");
        time = GetNode<Label>("/root/MainScene/CanvasLayer/Info/InfoBox/TimeContainer/Time");
        populationLabel = GetNode<Label>("/root/MainScene/CanvasLayer/Info/InfoBox/PopContainer/Population");
        tileMap = GetNode<TileMap>("/root/MainScene/TileMap");
        buildingButtons = GetNode<HBoxContainer>("/root/MainScene/CanvasLayer/Buttons/Panel/Buttons");

        var buildingButton = GD.Load<PackedScene>("res://BuildingButton.tscn");
        var init = false;
        var bulldozerButton = (BuildingButton) buildingButton.Instance();
        bulldozerButton.Name = "Bulldozer";
        bulldozerButton.isBulldozer = true;
        bulldozerButton.GetNode<CenterContainer>("LabelContainer").Free();
        bulldozerButton.TextureNormal = GD.Load<Texture>("res://bulldozer.png");
        bulldozerButton.Connect("BuildingSelected", this, "BuildingSelected");
        buildingButtons.AddChild(bulldozerButton);

        foreach (var buildingType in BuildingTypes.values) {
            var button = (BuildingButton) buildingButton.Instance();
            button.Name = buildingType.tileName;
            button.buildingType = buildingType;
            if (!(buildingType is BuildingTypes.Road)) {
                button.TextureNormal = GD.Load<Texture>($"res://Buildings/{buildingType.tileName}.png");
            }
            else {
                button.TextureNormal = GD.Load<Texture>("res://road_icon.png");
            }

            button.Connect("BuildingSelected", this, "BuildingSelected");

            var priceLabel = button.GetNode<Label>("LabelContainer/Label");
            priceLabel.SetText(buildingType.price.ToString());
            buildingButtons.AddChild(button);
            if (!init) {
                selectedBuilding = buildingType;
                init = true;
                var underline = button.GetNode<ColorRect>("Underline");
                underline.Visible = true;

            }

            if (!(buildingType is BuildingTypes.Road)) {
                buildings.Add(buildingType, 0);
            }
        }

        UpdatePopulationLabel();
        demandManager.RecalculateDemand();
    }

    public override void _Process(float delta) {
        if (Population <= 0 && OS.GetTicksMsec() - dieTime > 3000) {
            GetTree().ChangeScene("res://GameOver.tscn");
        }

        if (Input.IsActionJustPressed("screenshot")) {
            var image = GetViewport().GetTexture().GetData();
            image.FlipY();
            image.SavePng("res://Screenshots/screenshot.png");
        }
    }

    public void OnNextDay() {
        demandManager.OnNextDay();
    }

    // Signal
    public void BuildingSelected(BuildingButton button) {
        foreach (var child in buildingButtons.GetChildren()) {
            if (child is BuildingButton) {
                ((BuildingButton) child).Pressed = false;
                ((BuildingButton) child).GetNode<ColorRect>("Underline").Visible = false;
            }
        }

        button.Pressed = true;
        selectedBuilding = button.buildingType;
        button.GetNode<ColorRect>("Underline").Visible = true;
    }

    public override void _UnhandledInput(InputEvent inputEvent) {
        
        if (!(inputEvent is InputEventMouseButton)) {
            return;
        }

        var mouseEvent = (InputEventMouseButton) inputEvent;
        if (!mouseEvent.Pressed) {
            return;
        }

        if (mouseEvent.ButtonIndex != 1) {
            return;
        }
        
        if (Population < selectedBuilding.price) {
            return;
        }

        var mousePosition = tileMap.WorldToMap(GetGlobalMousePosition());
        var selectedTile = tileMap.GetCellv(mousePosition);
        if (selectedTile != -1) {
            if (selectedBuilding == null) {
                var tileName = tileMap.TileSet.TileGetName(selectedTile);
                var building = TilemapHelper.getBuildingFromTilename(tileName);
                if (building == null) {
                    GD.PrintErr("Tried to remove tile that's not a building!");
                    return;
                }

                placedBuildings.Remove(mousePosition);
                tileMap.SetCellv(mousePosition, -1);
                tileMap.UpdateBitmaskRegion(new Vector2(mousePosition.x - 1, mousePosition.y - 1), new Vector2(mousePosition.x + 1, mousePosition.y + 1));

                if (building is BuildingTypes.Road) {
                    RoadCount--;
                }
                else {
                    buildings[building]--;
                }
            }

            return;
        }

        if (selectedBuilding == null) {
            return;
        }

        if (!TilemapHelper.isTileRoad(selectedBuilding.tileName) && !TilemapHelper.IsByRoad(tileMap, (int) mousePosition.x, (int) mousePosition.y)) {
            return;
        }

        tileMap.SetCellv(mousePosition, tileMap.TileSet.FindTileByName(selectedBuilding.tileName));
        tileMap.UpdateBitmaskRegion(new Vector2(mousePosition.x - 1, mousePosition.y - 1), new Vector2(mousePosition.x + 1, mousePosition.y + 1));
//        tileMap.UpdateBitmaskArea(mousePosition);

        if (selectedBuilding is BuildingTypes.Road) {
            RoadCount++;
        }
        else {
            buildings[selectedBuilding]++;
            placedBuildings.Add(mousePosition);
        }

        RemovePopulation(selectedBuilding.price);
    }

    public void AddPopulation(int amount) {
        if (amount < 0) {
            return;
        }

        Population += amount;
        UpdatePopulationLabel();
    }

    public void RemovePopulation(int amount) {
        if (amount < 0) {
            return;
        }

        Population -= amount;
        if (Population < 0) {
            Population = 0;
        }

        if (Population == 0) {
            dieTime = OS.GetTicksMsec();
            UpdatePopulationLabel();
            return;
        }

        deathMessageManager.AddDeaths(amount);
        TotalDeaths += amount;
        UpdatePopulationLabel();
    }

    public void TickTime() {
        currentTime++;
        if (currentTime > 24) {
            currentTime = 0;
            OnNextDay();
        }

        var hour = currentTime.ToString();
        if (currentTime < 10) {
            hour = "0" + hour;
        }

        time.SetText($"{hour}:00");
    }

    private void UpdatePopulationLabel() {
        populationLabel.SetText(Population.ToString());
    }
}
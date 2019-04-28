using Godot;
using System;

public class BuildingButton : TextureButton {

    [Signal]
    public delegate void BuildingSelected(BuildingButton buildingButton);
    
    public BuildingTypes.Building buildingType;
    public bool isBulldozer = false;

    private ColorRect underline;

    public override void _Ready() {
        underline = GetNode<ColorRect>("Underline");
    }

    public void OnPressed() {
        switch (buildingType) {
            case BuildingTypes.House house:
                underline.Color = new Color(36f / 255f, 240f / 255f, 68f / 255f);
                break;
            case BuildingTypes.ShoppingCenter shoppingCenter:
                underline.Color = new Color(48f / 255f, 193f / 255f, 237f / 255f);
                break;
            case BuildingTypes.Workplace workplace:
                underline.Color = new Color(250f / 255f, 47f / 255f, 56f / 255f);
                break;
        }
        EmitSignal("BuildingSelected", this);
    }
    
}
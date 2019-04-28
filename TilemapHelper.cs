using System.Linq;
using Godot;

public class TilemapHelper {

    public static bool IsByRoad(TileMap tileMap, int x, int y) {
        var up = tileMap.GetCell(x, y - 1);
        var down = tileMap.GetCell(x, y + 1);
        var left = tileMap.GetCell(x - 1, y);
        var right = tileMap.GetCell(x + 1, y);
        
        var tileSet = tileMap.TileSet;
        foreach (var building in BuildingTypes.values) {
            if (!(building is BuildingTypes.Road)) continue;
            if (isAny(tileSet, building, up, down, left, right)) {
                return true;
            }
        }
        return false;
    }

    private static bool isAny(TileSet tileSet, BuildingTypes.Building building, params int[] tiles) {
        foreach (var tile in tiles) {
            if (tile == -1) continue;
            
            if (building.tileName.Equals(tileSet.TileGetName(tile))) {
                return true;
            }
        }

        return false;
    }

    public static bool isTileRoad(string tileName) {
        foreach (var building in BuildingTypes.values) {
            if (!(building is BuildingTypes.Road)) continue;

            if (building.tileName.Equals(tileName)) return true;
        }

        return false;
    }

    public static BuildingTypes.Building getBuildingFromTilename(string name) {
        return BuildingTypes.values.FirstOrDefault(it => it.tileName.Equals(name));
    }
    
}
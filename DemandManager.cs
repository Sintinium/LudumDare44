using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class DemandManager : Node2D {
    private GameMaster gameMaster;

    private int houseDemand = 0;
    private int shopDemand = 0;
    private int workDemand = 0;

    private int totalHouse = 0;
    private int totalShop = 0;
    private int totalWork = 0;

    private TextureProgress barHouse;
    private TextureProgress barShop;
    private TextureProgress barWork;

    private int Population => gameMaster.Population;

    public override void _Ready() {
        gameMaster = GetNode<GameMaster>("..");

        var demandContainer = "/root/MainScene/CanvasLayer/Demand/DemandContainer/";
        barHouse = GetNode<TextureProgress>(demandContainer + "HouseContainer/House");
        barShop = GetNode<TextureProgress>(demandContainer + "ShopContainer/Shop");
        barWork = GetNode<TextureProgress>(demandContainer + "WorkContainer/Work");
    }

    public void RecalculateDemand() {
        totalHouse = 0;
        totalShop = 0;
        totalWork = 0;

        houseDemand = HouseGoal();
        shopDemand = ShopGoal();
        workDemand = WorkGoal();

        foreach (var it in gameMaster.buildings) {
            switch (it.Key) {
                case BuildingTypes.House house:
                    totalHouse += house.size * it.Value;
                    break;
                case BuildingTypes.ShoppingCenter shop:
                    totalShop += shop.size * it.Value;
                    break;
                case BuildingTypes.Workplace work:
                    totalWork += work.size * it.Value;
                    break;
            }
        }

        UpdateCharts();
    }

    private void UpdateCharts() {
        var houseRatio = (float) totalHouse / houseDemand;
        var shopRatio = (float) totalShop / shopDemand;
        var workRatio = (float) totalWork / workDemand;

        barHouse.Value = Mathf.Max(100 - houseRatio * 100, 1);
        barShop.Value = Mathf.Max(100 - shopRatio * 100, 1);
        barWork.Value = Mathf.Max(100 - workRatio * 100, 1);
    }

    public void OnNextDay() {
        PayoutTaxes();

        var houseDiff = houseDemand - totalHouse;
        var shopDiff = shopDemand - totalShop;
        var workDiff = workDemand - totalWork;

        if (houseDiff > Population * .6f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(houseDiff * .25f));
        }
        else if (houseDiff > Population * .3f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(houseDiff * .1f));
        }

        if (shopDiff > Population * .8f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(shopDiff * .125f));
        }
        else if (shopDiff > Population * .4f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(shopDiff * .05f));
        }

        if (workDiff > Population * .7f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(workDiff * .18f));
        }
        else if (workDiff > Population * .45f) {
            gameMaster.RemovePopulation(Mathf.FloorToInt(workDiff * .9f));
        }
    }

    private void PayoutTaxes() {
        gameMaster.RemovePopulation(Mathf.FloorToInt((float) gameMaster.RoadCount / BuildingTypes.ROAD_SET_SIZE) * BuildingTypes.ROAD_TAX_PER_SET);
        var taken = new Dictionary<Type, int> {{typeof(BuildingTypes.House), 0}, {typeof(BuildingTypes.ShoppingCenter), 0}, {typeof(BuildingTypes.Workplace), 0}};
        foreach (var it in gameMaster.buildings) {
            var building = it.Key;
            var amount = it.Value;

            // Take/give taxes to people and workplaces
            switch (building) {
                case BuildingTypes.TaxedBuilding taxedBuilding:
                    var popRatio = 1f;
                    if (taxedBuilding is BuildingTypes.ShoppingCenter) {
                        popRatio = .4f;
                    }
                    else if (taxedBuilding is BuildingTypes.Workplace) {
                        popRatio = .8f;
                    }

                    var takenSlots = taken[taxedBuilding.GetType()];
                    var buildingDiff = (Population * popRatio) - takenSlots;
                    var buildingSize = taxedBuilding.size * amount;
                    if (buildingDiff > 0) {
                        if (buildingDiff < buildingSize) {
                            buildingSize = (int) buildingDiff - buildingSize;
                        }

                        taken[taxedBuilding.GetType()] += buildingSize;
                        amount -= Mathf.FloorToInt(buildingSize / (float) amount);
                        gameMaster.AddPopulation(taxedBuilding.tax * amount);
                    }

                    break;
                case BuildingTypes.WorkerBuilding workerBuilding:
                    gameMaster.RemovePopulation(workerBuilding.tax * amount);
                    break;
            }
        }
    }

    public int HouseGoal() {
        var ratio = 10 / 10f;
        return Mathf.FloorToInt(gameMaster.Population * ratio);
    }

    public int ShopGoal() {
        var ratio = 4 / 10f;
        return Mathf.FloorToInt(gameMaster.Population * ratio);
    }

    public int WorkGoal() {
        var ratio = 8 / 10f;
        return Mathf.FloorToInt(gameMaster.Population * ratio);
    }
}
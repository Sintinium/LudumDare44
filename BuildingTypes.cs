using System;
using System.Collections.Generic;

public static class BuildingTypes {
    public static readonly Road ROAD1 = new Road("roads", 1);
    
    public static readonly House HOUSE1 = new House("house_1", 10, 3, 1);
    public static readonly House APARTMENT1 = new House("apartment_2", 25, 10, 3);
    public static readonly House APARTMENT2 = new House("apartment_3", 40, 15, 5);
    public static readonly House APARTMENT3 = new House("apartment_1", 60, 20, 6);

    public static readonly ShoppingCenter SHOP1 = new ShoppingCenter("shop_1", 15, 5, 2);
    public static readonly ShoppingCenter SHOP2 = new ShoppingCenter("shop_2", 35, 10, 5);
    public static readonly ShoppingCenter SHOP3 = new ShoppingCenter("shop_4", 50, 15, 7);
    public static readonly ShoppingCenter SHOP4 = new ShoppingCenter("shop_3", 120, 30, 15);
    
    public static readonly Workplace WORK1 = new Workplace("work_1", 40, 15, 15);
    public static readonly Workplace WORK2 = new Workplace("work_3", 55, 20, 20);
    public static readonly Workplace WORK3 = new Workplace("office_1", 120, 40, 40);
    public static readonly Workplace WORK4 = new Workplace("work_2", 160, 50, 50);
    public static readonly Workplace WORK5 = new Workplace("work_4", 400, 100, 100);

    public static readonly int ROAD_TAX_PER_SET = 5;
    public static readonly int ROAD_SET_SIZE = 10;
        
    public static IEnumerable<Building> values {
        get {
            yield return ROAD1;
            
            yield return HOUSE1;
            yield return APARTMENT1;
            yield return APARTMENT2;
            yield return APARTMENT3;
            
            yield return SHOP1;
            yield return SHOP2;
            yield return SHOP3;
            yield return SHOP4;
            
            yield return WORK1;
            yield return WORK2;
            yield return WORK3;
            yield return WORK4;
            yield return WORK5;
        }
    }

    public abstract class Building {
        public readonly string tileName;
        public readonly int price;

        protected Building(string tileName, int price) {
            this.tileName = tileName;
            this.price = price;
        }
    }

    public abstract class TaxedBuilding : Building {
        public readonly int size;
        public readonly int tax;

        protected TaxedBuilding(string tileName, int price, int size, int tax) : base(tileName, price) {
            this.size = size;
            this.tax = tax;
        }
    }
    
    public class Road : Building {
        public Road(string tileName, int price) : base(tileName, price) { }
    }

    public class House : TaxedBuilding {
        public House(string tileName, int price, int size, int tax) : base(tileName, price, size, tax) { }
    }

    public class Workplace : TaxedBuilding {
        public Workplace(string tileName, int price, int size, int tax) : base(tileName, price, size, tax) { }
    }

    public class ShoppingCenter : TaxedBuilding {
        public ShoppingCenter(string tileName, int price, int size, int tax) : base(tileName, price, size, tax) { }
    } 
    
    public class WorkerBuilding : Building {
        public int workers;
        public int tax;
        public WorkerBuilding(string tileName, int price, int workers, int tax) : base(tileName, price) {
            this.workers = workers;
            this.tax = tax;
        }
    }
}
using System;

[Serializable]
public class CellData
{
    public int stage;
    public float timer;
    public int watermelonIndex = -1;
}

[Serializable]
public class SaveData
{
    public string saveName = "Слот";
    public string saveDate = "";
    public int coins;
    public int growSpeedLevel;
    public int harvestValueLevel;
    public int critChanceLevel;
    public int fertilizerLevel;
    public int superSeedLevel;
    public CellData[] cells = new CellData[0];
}

// UpgradeData.cs
using UnityEngine;

public enum UpgradeType
{
    Weapon,
    Stat
}

[System.Serializable]
public class UpgradeData
{
    [Header("Basic Info")]
    public string upgradeName;
    public string description;
    public Sprite icon;
    public int level = 0;
    public int maxLevel = 5;

    [Header("Upgrade Properties")]
    public UpgradeType type;
    public string upgradeID;
    public float[] valuePerLevel; // Different values for each level
    public GameObject weaponPrefab; // For weapon upgrades
    public bool isUnlocked = false;
    public string statToUpgrade; // For stat upgrades: "attack", "defense", etc.

    public float GetCurrentValue()
    {
        return valuePerLevel[Mathf.Min(level, valuePerLevel.Length - 1)];
    }
}
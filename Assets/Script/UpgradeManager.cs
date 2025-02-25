using UnityEngine;
using System.Collections.Generic;


public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    [Header("Weapons")]
    public List<UpgradeData> allWeaponUpgrades = new List<UpgradeData>();
    public List<UpgradeData> unlockedWeapons = new List<UpgradeData>();

    [Header("Weapons Container")]
    public GameObject weaponsContainerPrefab;
    private GameObject weaponsContainer;
    private Transform weaponHolder;

    [Header("Stats")]
    public List<UpgradeData> statUpgrades = new List<UpgradeData>();

    [Header("Upgrade Settings")]
    public int upgradeOptionsCount = 3;
    public bool hasOrbitWeapon = false;

    private PlayerStats playerStats;
    private Transform playerTransform;

    private void Awake()
    {
        instance = this;
        playerStats = FindObjectOfType<PlayerStats>();
        playerTransform = playerStats?.transform;

        // Create weapons container
        InitializeWeaponsContainer();
    }

    private void InitializeWeaponsContainer()
    {
        // Check if weapons container already exists
        weaponsContainer = GameObject.Find("WeaponsContainer");

        if (weaponsContainer == null)
        {
            // Create new container
            if (weaponsContainerPrefab != null)
            {
                weaponsContainer = Instantiate(weaponsContainerPrefab);
            }
            else
            {
                weaponsContainer = new GameObject("WeaponsContainer");
            }

            // Add follow script if needed
            if (!weaponsContainer.GetComponent<WeaponsContainer>())
            {
                weaponsContainer.AddComponent<WeaponsContainer>();
            }
        }

        weaponsContainer.name = "WeaponsContainer";
        weaponHolder = weaponsContainer.transform;
    }

    private void LateUpdate()
    {
        // Ensure the weapons container exists
        if (weaponsContainer == null)
        {
            InitializeWeaponsContainer();
        }
    }


    public List<UpgradeData> GetAvailableUpgrades()
    {
        List<UpgradeData> availableUpgrades = new List<UpgradeData>();

        // Add weapon upgrades
        foreach (var weapon in allWeaponUpgrades)
        {
            // Add unowned weapons or upgradeable weapons
            if ((!weapon.isUnlocked && CanUnlockWeapon(weapon)) ||
                (weapon.isUnlocked && weapon.level < weapon.maxLevel))
            {
                availableUpgrades.Add(weapon);
            }
        }

        // Add stat upgrades
        foreach (var stat in statUpgrades)
        {
            if (stat.level < stat.maxLevel)
            {
                availableUpgrades.Add(stat);
            }
        }

        return availableUpgrades;
    }

    private bool CanUnlockWeapon(UpgradeData weapon)
    {
        // Check if it's an orbit weapon and player doesn't have one yet
        if (weapon.upgradeID.Contains("orbit") && !hasOrbitWeapon)
            return true;

        // Other weapon-specific conditions can be added here
        return true;
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.Weapon:
                ApplyWeaponUpgrade(upgrade);
                break;
            case UpgradeType.Stat:
                ApplyStatUpgrade(upgrade);
                break;
        }

        upgrade.level++;

        // If this is the first level, mark as unlocked
        if (upgrade.level == 1)
        {
            upgrade.isUnlocked = true;
            if (upgrade.type == UpgradeType.Weapon)
            {
                unlockedWeapons.Add(upgrade);
                if (upgrade.upgradeID.Contains("orbit"))
                {
                    hasOrbitWeapon = true;
                }
            }
        }
    }

    private void ApplyWeaponUpgrade(UpgradeData upgrade)
    {
        if (weaponHolder == null)
        {
            InitializeWeaponsContainer();
        }

        if (!upgrade.isUnlocked)
        {
            // Spawn new weapon
            if (upgrade.weaponPrefab != null)
            {
                GameObject newWeapon = Instantiate(upgrade.weaponPrefab, weaponHolder);
                // Configure weapon properties based on level
                ConfigureWeapon(newWeapon, upgrade);
            }
        }
        else
        {
            // Upgrade existing weapon
            GameObject existingWeapon = FindWeaponByID(upgrade.upgradeID);
            if (existingWeapon != null)
            {
                ConfigureWeapon(existingWeapon, upgrade);
            }
        }
    }

    private void ConfigureWeapon(GameObject weapon, UpgradeData upgrade)
    {
        // Configure weapon based on its type
        if (upgrade.upgradeID.Contains("orbit"))
        {
            var flightKnife = weapon.GetComponent<FlightKnife>();
            if (flightKnife != null)
            {
                flightKnife.rotationSpeed = upgrade.GetCurrentValue();
            }
        }
        // Add configuration for other weapon types
    }

    private GameObject FindWeaponByID(string upgradeID)
    {
        if (weaponHolder == null) return null;

        // Simple implementation - iterate through children and look for matching component name
        foreach (Transform child in weaponHolder)
        {
            // Match by name containing the upgrade ID
            if (child.name.ToLower().Contains(upgradeID.ToLower()))
            {
                return child.gameObject;
            }

            // Or check for specific components based on weapon type
            if (upgradeID.Contains("orbit") && child.GetComponent<FlightKnife>() != null)
            {
                return child.gameObject;
            }
            // Add more weapon type checks as needed
        }

        return null;
    }

    private void ApplyStatUpgrade(UpgradeData upgrade)
    {
        if (playerStats != null)
        {
            float upgradeValue = upgrade.GetCurrentValue();

            switch (upgrade.statToUpgrade.ToLower())
            {
                case "attack":
                    playerStats.attackMultiplier += upgradeValue;
                    break;
                case "defense":
                    playerStats.currentDefense += upgradeValue;
                    break;
                case "speed":
                    var movement = playerStats.GetComponent<PlayerMovement>();
                    if (movement != null)
                    {
                        movement.moveSpeed += upgradeValue;
                    }
                    break;
                case "health":
                    playerStats.healthMultiplier += upgradeValue;
                    playerStats.UpdateMaxHealth();
                    break;
                    // Add other stats as needed
            }
        }
    }
}


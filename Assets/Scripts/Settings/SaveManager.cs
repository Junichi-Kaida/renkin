using UnityEngine;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private List<MaterialSO> allMaterials;
    [SerializeField] private List<WeaponSO> allWeapons;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        foreach (var mat in allMaterials)
        {
            int count = InventoryManager.Instance.GetMaterialCount(mat);
            PlayerPrefs.SetInt("Mat_" + mat.materialName, count);
        }

        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        if (combat != null && combat.GetCurrentWeapon() != null)
        {
            PlayerPrefs.SetString("EquippedWeapon", combat.GetCurrentWeapon().weaponName);
        }

        PlayerPrefs.Save();
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        Dictionary<MaterialSO, int> loadedMaterials = new Dictionary<MaterialSO, int>();
        foreach (var mat in allMaterials)
        {
            if (PlayerPrefs.HasKey("Mat_" + mat.materialName))
            {
                int count = PlayerPrefs.GetInt("Mat_" + mat.materialName);
                if (count > 0) loadedMaterials.Add(mat, count);
            }
        }
        InventoryManager.Instance.SetMaterials(loadedMaterials);

        if (PlayerPrefs.HasKey("EquippedWeapon"))
        {
            string weaponName = PlayerPrefs.GetString("EquippedWeapon");
            WeaponSO weapon = allWeapons.Find(w => w.weaponName == weaponName);
            if (weapon != null)
            {
                FindObjectOfType<PlayerCombat>()?.SetWeapon(weapon);
            }
        }

        Debug.Log("Game Loaded!");
    }
}

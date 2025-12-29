using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Debug / Starting Items")]
    [SerializeField] private List<MaterialSO> startingMaterials;
    [SerializeField] private List<int> startingAmounts;

    private Dictionary<MaterialSO, int> materials = new Dictionary<MaterialSO, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Apply starting items
            if (startingMaterials != null)
            {
                for(int i=0; i<startingMaterials.Count; i++)
                {
                    if (startingMaterials[i] != null)
                    {
                        int amt = (i < startingAmounts.Count) ? startingAmounts[i] : 1;
                        AddMaterial(startingMaterials[i], amt);
                    }
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddMaterial(MaterialSO material, int amount)
    {
        if (materials.ContainsKey(material))
        {
            materials[material] += amount;
        }
        else
        {
            materials.Add(material, amount);
        }
        Debug.Log($"Added {amount} {material.materialName}. Total: {materials[material]}");
        SaveManager.Instance?.SaveGame();
    }

    public void SetMaterials(Dictionary<MaterialSO, int> loadedMaterials)
    {
        materials = loadedMaterials;
    }

    public Dictionary<MaterialSO, int> GetMaterials() => materials;

    public bool HasMaterials(MaterialSO material, int amount)
    {
        if (materials.ContainsKey(material))
        {
            return materials[material] >= amount;
        }
        return false;
    }

    public void ConsumeMaterial(MaterialSO material, int amount)
    {
        if (HasMaterials(material, amount))
        {
            materials[material] -= amount;
        }
    }

    public int GetMaterialCount(MaterialSO material)
    {
        if (materials.ContainsKey(material))
        {
            return materials[material];
        }
        return 0;
    }

    public Dictionary<MaterialSO, int> GetAllMaterials() => materials;
}

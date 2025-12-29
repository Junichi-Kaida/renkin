using UnityEngine;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public bool CanCraft(RecipeSO recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (!InventoryManager.Instance.HasMaterials(ingredient.material, ingredient.amount))
            {
                return false;
            }
        }
        return true;
    }

    public void Craft(RecipeSO recipe)
    {
        if (!CanCraft(recipe)) return;

        foreach (var ingredient in recipe.ingredients)
        {
            InventoryManager.Instance.ConsumeMaterial(ingredient.material, ingredient.amount);
        }

        PlayerCombat playerCombat = FindObjectOfType<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.SetWeapon(recipe.resultWeapon);
            Debug.Log($"Alchemized {recipe.resultWeapon.weaponName}!");
        }
    }
}

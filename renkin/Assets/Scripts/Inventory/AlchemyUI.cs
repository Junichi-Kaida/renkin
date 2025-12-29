using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AlchemyUI : MonoBehaviour
{
    [Header("Recipes")]
    [SerializeField] private List<RecipeSO> availableRecipes;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject recipeButtonPrefab;
    [SerializeField] private Transform recipeListParent;
    [SerializeField] private Text materialDisplayText;
    [SerializeField] private Text resultInfoText;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeButton;

    private RecipeSO selectedRecipe;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        craftButton.onClick.AddListener(OnCraftClicked);
        RefreshUI();
    }

    private void RefreshUI()
    {
        UpdateMaterialDisplay();
        PopulateRecipes();
        UpdateSelectedRecipeInfo();
    }

    private void UpdateMaterialDisplay()
    {
        var materials = InventoryManager.Instance.GetAllMaterials();
        string text = "Materials:\n";
        foreach (var mat in materials)
        {
            text += $"{mat.Key.materialName}: {mat.Value}\n";
        }
        materialDisplayText.text = text;
    }

    private void PopulateRecipes()
    {
        foreach (Transform child in recipeListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var recipe in availableRecipes)
        {
            GameObject btnObj = Instantiate(recipeButtonPrefab, recipeListParent);
            btnObj.GetComponentInChildren<Text>().text = recipe.recipeName;
            btnObj.GetComponent<Button>().onClick.AddListener(() => SelectRecipe(recipe));
        }
    }

    private void SelectRecipe(RecipeSO recipe)
    {
        selectedRecipe = recipe;
        UpdateSelectedRecipeInfo();
    }

    private void UpdateSelectedRecipeInfo()
    {
        if (selectedRecipe == null)
        {
            resultInfoText.text = "Select a recipe";
            craftButton.interactable = false;
            return;
        }

        string info = $"Weapon: {selectedRecipe.resultWeapon.weaponName}\n";
        info += $"Damage: {selectedRecipe.resultWeapon.damage}\n";
        info += $"Interval: {selectedRecipe.resultWeapon.attackInterval}s\n";
        info += $"Range: {selectedRecipe.resultWeapon.attackRange}\n\n";
        info += "Requirement:\n";

        bool hasAll = true;
        foreach (var ing in selectedRecipe.ingredients)
        {
            int current = InventoryManager.Instance.GetMaterialCount(ing.material);
            string status = current >= ing.amount ? " (OK)" : " (Missing)";
            info += $"{ing.material.materialName}: {current}/{ing.amount}{status}\n";
            if (current < ing.amount) hasAll = false;
        }

        resultInfoText.text = info;
        craftButton.interactable = hasAll;
    }

    private void OnCraftClicked()
    {
        if (selectedRecipe != null)
        {
            AlchemyManager.Instance.Craft(selectedRecipe);
            RefreshUI();
        }
    }

    private void Close()
    {
        FindObjectOfType<AlchemyTable>()?.CloseUI();
    }
}

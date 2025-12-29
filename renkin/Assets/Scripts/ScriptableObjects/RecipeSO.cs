using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct Ingredient
{
    public MaterialSO material;
    public int amount;
}

[CreateAssetMenu(fileName = "NewRecipe", menuName = "AlchemyGame/Recipe")]
public class RecipeSO : ScriptableObject
{
    public string recipeName;
    public List<Ingredient> ingredients;
    public WeaponSO resultWeapon;
}

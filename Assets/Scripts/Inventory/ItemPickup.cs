using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private MaterialSO material;
    [SerializeField] private int amount = 1;

    public void SetItem(MaterialSO newMaterial, int newAmount)
    {
        material = newMaterial;
        amount = newAmount;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            InventoryManager.Instance.AddMaterial(material, amount);
            Destroy(gameObject);
        }
    }
}

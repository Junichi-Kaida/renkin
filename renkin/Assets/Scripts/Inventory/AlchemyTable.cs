using UnityEngine;

public class AlchemyTable : MonoBehaviour
{
    [SerializeField] private GameObject alchemyUIPrefab;
    private GameObject activeUI;
    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            ToggleAlchemyUI();
        }
    }

    private void ToggleAlchemyUI()
    {
        if (activeUI == null)
        {
            activeUI = Instantiate(alchemyUIPrefab);
            Time.timeScale = 0f;
        }
        else
        {
            CloseUI();
        }
    }

    public void CloseUI()
    {
        if (activeUI != null)
        {
            Destroy(activeUI);
            Time.timeScale = 1f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (activeUI != null) CloseUI();
        }
    }
}

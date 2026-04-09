using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject winUI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           Time.timeScale = 0f; // Stop the game
           winUI.SetActive(true); // Show the win UI

        }
    }
}

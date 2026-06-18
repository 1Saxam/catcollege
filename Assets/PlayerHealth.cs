using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    private bool canTakeDamage = true;
    public float damageCooldown = 1f;

    public int lives = 3;

    public GameObject gameOverPanel;

    private PlayerMovement movement;
    private Rigidbody2D rb;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        gameOverPanel.SetActive(false);

        UpdateHearts();

    }

    public void TakeDamage()
    {
        if (!canTakeDamage)
            return;

        canTakeDamage = false;

        lives--;

        UpdateHearts();

        Debug.Log("Lives Left: " + lives);

        if (lives <= 0)
        {
            movement.canMove = false;
            rb.velocity = Vector2.zero;

            gameOverPanel.SetActive(true);
        }

        Invoke(nameof(ResetDamage), damageCooldown);
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    void ResetDamage()
    {
        canTakeDamage = true;
    }


    void UpdateHearts()
    {
        heart1.SetActive(lives >= 1);
        heart2.SetActive(lives >= 2);
        heart3.SetActive(lives >= 3);
    }



}
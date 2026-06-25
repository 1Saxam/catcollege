using UnityEngine;

public class CrowHealth : MonoBehaviour
{

    public GateController gate;


    [Header("Health")]
    public int maxHealth = 3;
    public float damageCooldown = 0.5f;

    [Header("Heart UI")]
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    private int currentHealth;
    private bool canTakeDamage = true;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
    }

    public void TakeDamage(int damage)
    {
        if (!canTakeDamage)
            return;

        canTakeDamage = false;

        currentHealth -= damage;

        Debug.Log("Crow Health: " + currentHealth);

        UpdateHearts();

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        Invoke(nameof(ResetDamage), damageCooldown);
    }

    void ResetDamage()
    {
        canTakeDamage = true;
    }

    void UpdateHearts()
    {
        if (heart1 != null)
            heart1.SetActive(currentHealth >= 1);

        if (heart2 != null)
            heart2.SetActive(currentHealth >= 2);

        if (heart3 != null)
            heart3.SetActive(currentHealth >= 3);
    }

    void Die()
    {
        Debug.Log("Crow Disabled");

        if (gate != null)
        {
            gate.OpenGate();
        }

        gameObject.SetActive(false);

        Debug.Log(gameObject.activeSelf);
    }
}
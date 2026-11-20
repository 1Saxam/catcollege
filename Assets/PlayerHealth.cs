using System.Collections.Generic;
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
    public float gameOverDelay = 1f;          // lets the death animation play first

    [Header("Bleed Effect")]
    public GameObject bleedEffectPrefab;      // red particle burst prefab
    public float bleedLifetime = 1.5f;

    [Header("Sounds (optional)")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Cat Animator (optional)")]
    public Animator animator;
    public string hurtTrigger = "Hurt";       // -> Stuned state
    public string deathTrigger = "Death";     // -> Death state

    public bool IsDead => lives <= 0;

    private PlayerMovement movement;
    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    
    private readonly HashSet<string> animParams = new HashSet<string>();

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (animator == null) animator = GetComponent<Animator>();
        if (animator != null)
            foreach (var p in animator.parameters) animParams.Add(p.name);

        gameOverPanel.SetActive(false);

        UpdateHearts();
    }

    [Header("Knockback")]
    public float knockbackTime = 0.25f;       // how long controls are paused while pushed

    // Old version still works (crows etc.)
    public void TakeDamage()
    {
        TakeDamage(transform.position, Vector2.zero);
    }

    public bool TakeDamage(Vector2 hitFrom)
    {
        return TakeDamage(hitFrom, Vector2.zero);
    }

    // hitFrom = where the hit came from (bleed spawns on that side of the cat)
    // push = knockback velocity (zero = no push). Returns true if damage was actually taken.
    public bool TakeDamage(Vector2 hitFrom, Vector2 push)
    {
        if (!canTakeDamage || IsDead)
            return false;

        canTakeDamage = false;

        lives--;

        

        SpawnBleed(hitFrom);
        UpdateHearts();

        Debug.Log("Lives Left: " + lives);

        if (lives <= 0)
        {
            movement.canMove = false;
            rb.velocity = Vector2.zero;

            PlaySound(deathSound);
            SetTrigger(deathTrigger);

            Invoke(nameof(ShowGameOver), gameOverDelay);
            return true;
        }

        PlaySound(hurtSound);
        SetTrigger(hurtTrigger);

        if (movement != null && push != Vector2.zero)
            movement.ApplyKnockback(push, knockbackTime);

        Invoke(nameof(ResetDamage), damageCooldown);
        return true;
    }

    void SpawnBleed(Vector2 hitFrom)
    {
        if (bleedEffectPrefab == null) return;

        Vector2 pos = bodyCollider != null ? bodyCollider.ClosestPoint(hitFrom) : (Vector2)transform.position;
        GameObject fx = Instantiate(bleedEffectPrefab, pos, Quaternion.identity, transform);
        Destroy(fx, bleedLifetime);
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }

    void SetTrigger(string name)
    {
        if (animator != null && animParams.Contains(name)) animator.SetTrigger(name);
    }
}
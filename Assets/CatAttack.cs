using System.Collections;
using UnityEngine;

public class CatAttack : MonoBehaviour
{
    public Animator anim;
    public GameObject attackPoint;   // the child with AttackHitbox + trigger collider

    public float attackDuration = 0.2f;

    public bool isAttacking;

    [Header("Attack Sound")]
    public AudioSource audioSource;          // leave empty = uses the AudioSource on the cat
    public AudioClip attackSound;

    [Header("Attack Effect (slash image)")]
    public Sprite attackEffectSprite;        // drag your attack effect image here
    public Vector2 effectOffset = Vector2.zero;   // nudge it from the Attack Point
    public float effectSize = 1f;
    public float effectTime = 0.2f;          // how long it stays on screen
    public bool effectFlipX = false;         // tick if the slash points the wrong way

    private PlayerHealth playerHealth;
    private PlayerMovement movement;
    private SpriteRenderer catRenderer;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();
        catRenderer = GetComponentInChildren<SpriteRenderer>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        if (attackPoint != null) attackPoint.SetActive(false);
    }

    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead) return;
        if (movement != null && !movement.canMove) return;

        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");

        if (attackSound != null && audioSource != null)
            audioSource.PlayOneShot(attackSound);

        if (attackEffectSprite != null)
            StartCoroutine(ShowAttackEffect());

        if (attackPoint != null) attackPoint.SetActive(true);

        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
        if (attackPoint != null) attackPoint.SetActive(false);
    }

    IEnumerator ShowAttackEffect()
    {
        GameObject fx = new GameObject("AttackEffect");

        // Child of the cat -> follows the cat and flips with it automatically
        fx.transform.SetParent(transform, false);
        Vector3 basePos = attackPoint != null ? attackPoint.transform.localPosition : Vector3.zero;
        fx.transform.localPosition = basePos + (Vector3)effectOffset;

        SpriteRenderer sr = fx.AddComponent<SpriteRenderer>();
        sr.sprite = attackEffectSprite;
        sr.flipX = effectFlipX;
        if (catRenderer != null)
        {
            sr.sortingLayerID = catRenderer.sortingLayerID;
            sr.sortingOrder = catRenderer.sortingOrder + 5;   // draw in front of the cat
        }

        // Quick "pop and fade"
        float t = 0f;
        Color c = sr.color;
        while (t < effectTime)
        {
            float k = t / effectTime;
            float scale = effectSize * Mathf.Lerp(0.8f, 1.15f, k);
            fx.transform.localScale = new Vector3(scale, scale, 1f);
            c.a = 1f - k * k;
            sr.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(fx);
    }
}
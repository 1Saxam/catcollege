using UnityEngine;

public class CatAttack : MonoBehaviour
{
    public Animator anim;
    public GameObject attackPoint;   // the child with AttackHitbox + trigger collider

    public float attackDuration = 0.2f;

    public bool isAttacking;

    private PlayerHealth playerHealth;
    private PlayerMovement movement;

    void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();

        // Hitbox is only switched on while attacking
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

        // Turning the hitbox on makes OnTriggerEnter fire even if the bat is already overlapping
        if (attackPoint != null) attackPoint.SetActive(true);

        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
        if (attackPoint != null) attackPoint.SetActive(false);
        Debug.Log("Attack Ended");
    }
}
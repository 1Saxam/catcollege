using UnityEngine;

public class CatAttack : MonoBehaviour
{
    public Animator anim;
    public GameObject attackPoint;

    public float attackDuration = 0.2f;

    private bool isAttacking;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && !isAttacking)
        {
            StartAttack();
        }
    }

    void StartAttack()
    {
        isAttacking = true;

        anim.SetTrigger("Attack");

        attackPoint.SetActive(true);

        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        attackPoint.SetActive(false);

        isAttacking = false;
    }
}
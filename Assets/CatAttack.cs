using UnityEngine;

public class CatAttack : MonoBehaviour
{
    public Animator anim;
    public GameObject attackPoint;

    public float attackDuration = 0.2f;

    public bool isAttacking;

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


        Invoke(nameof(EndAttack), attackDuration);
    }

    void EndAttack()
    {
        isAttacking = false;
        Debug.Log("Attack Ended");
    }
}
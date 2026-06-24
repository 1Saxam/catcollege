using UnityEngine;

public class CrowEnemy : MonoBehaviour
{
    public Transform player;

    public float detectRange = 5f;
    public float followSpeed = 3f;
    public float attackSpeed = 10f;
    public float chargeTime = 0.5f;
    public float recoverTime = 0.5f;

    public float hoverHeight = 2f;
    public float hoverDistance = 1.5f;

    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 targetPos;

    private bool canFollowPlayer = false;

    private enum State
    {
        Idle,
        Charge,
        Dive,
        Recover
    }

    private State currentState;

    private float timer;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        currentState = State.Idle;
    }

    public void StartFollowing()
    {
        canFollowPlayer = true;
    }

    void Update()
    {
        // Face player
        if (currentState != State.Dive)
        {
            if (player.position.x > transform.position.x)
            {
                sr.flipX = true; // swap if needed
            }
            else
            {
                sr.flipX = false;
            }
        }

        switch (currentState)
        {
            case State.Idle:

                anim.SetInteger("State", 0);

                if (!canFollowPlayer)
                    break;

                // Hover beside and above player
                float side = Mathf.Sign(transform.position.x - player.position.x);

                Vector2 hoverPos = new Vector2(
                    player.position.x + side * hoverDistance,
                    player.position.y + hoverHeight);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    hoverPos,
                    followSpeed * Time.deltaTime);

                // Attack when close enough
                if (Vector2.Distance(transform.position, player.position) < detectRange)
                {
                    currentState = State.Charge;
                    timer = 0f;

                    targetPos = player.position;
                }

                break;

            case State.Charge:

                anim.SetInteger("State", 1);

                timer += Time.deltaTime;

                if (timer >= chargeTime)
                {
                    currentState = State.Dive;
                }

                break;

            case State.Dive:

                anim.SetInteger("State", 2);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    targetPos,
                    attackSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, targetPos) < 0.1f)
                {
                    currentState = State.Recover;
                    timer = 0f;
                }

                break;

            case State.Recover:

                anim.SetInteger("State", 3);

                timer += Time.deltaTime;

                if (timer >= recoverTime)
                {
                    currentState = State.Idle;
                }

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (currentState != State.Dive)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage();

            currentState = State.Recover;
            timer = 0f;
        }
    }
}
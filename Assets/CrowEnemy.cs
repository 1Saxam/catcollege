using UnityEngine;

public class CrowEnemy : MonoBehaviour
{
    public Transform player;

    public float followSpeed = 3f;

    public float detectRange = 5f;
    public float attackSpeed = 10f;
    public float recoverSpeed = 5f;
    public float chargeTime = 0.5f;

    private Animator anim;
    private SpriteRenderer sr;

    private Vector2 startPos;
    private Vector2 targetPos;

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

        startPos = transform.position;

        currentState = State.Idle;
    }

    void Update()
    {
        // Face the player only while idle
        if (currentState == State.Idle)
        {
            // Swap true and false if the crow faces the wrong way
            if (player.position.x > transform.position.x)
            {
                sr.flipX = true;
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

                // Chase the player
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.position,
                    followSpeed * Time.deltaTime);

                // Start attack when close enough
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
                }

                break;

            case State.Recover:

                anim.SetInteger("State", 3);

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    startPos,
                    recoverSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, startPos) < 0.1f)
                {
                    currentState = State.Idle;
                }

                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage player only while diving
        if (currentState != State.Dive)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage();

            // Immediately go back after hitting the player
            currentState = State.Recover;
        }
    }
}
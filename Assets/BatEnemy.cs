using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Put this on the BAT (root object).
// Root needs: SpriteRenderer, Animator (your BatAnimation), Rigidbody2D, a Collider2D (body, Is Trigger ON), AudioSource.
// Child "BatAttack" (optional): Collider2D (Is Trigger ON) + BatAttackHitbox, placed at the bat's MOUTH.
// If you don't have the child, the bat uses Bite Point / Bite Radius instead.
[RequireComponent(typeof(Rigidbody2D))]
public class BatEnemy : MonoBehaviour
{
    [Header("References")]
    public Transform player;                 // leave empty = finds the cat automatically
    public BatAttackHitbox attackHitbox;     // optional child "BatAttack"
    public AudioSource audioSource;

    [Header("Debug")]
    public bool showDebugLogs = true;

    [Header("Health")]
    public int maxHealth = 2;
    public float invulnerableTime = 0.1f;
    public float hurtStunTime = 0.35f;
    public float knockbackSpeed = 4f;
    public float corpseStayTime = 0.5f;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float detectRange = 6f;
    public float loseRange = 9f;
    public float hoverAbovePlayer = 0.3f;    // height above the cat's middle
    public float hoverSideDistance = 1.2f;   // how far beside the cat it waits before biting
    public float bobHeight = 0.15f;
    public float bobSpeed = 3f;

    [Header("Facing")]
    [Tooltip("Stop the game, set the bat's Scale X to a POSITIVE number and look at it. " +
             "Tick if the bat's head points RIGHT, untick if it points LEFT. " +
             "You can also toggle this while playing to test.")]
    public bool artLooksRight = false;
    public float turnDeadZone = 0.25f;

    [Header("Attack")]
    public float attackRange = 1.5f;         // distance from bat to the cat's body edge
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.3f;        // pause before lunging
    public float lungeSpeed = 9f;
    public float attackActiveTime = 0.2f;    // lunge + bite time
    public float attackRecovery = 0.35f;
    [Tooltip("Used only if no BatAttack child: bite point in front of the bat (local X forward, Y up).")]
    public Vector2 bitePoint = new Vector2(0.6f, 0f);
    public float biteRadius = 0.5f;

    [Header("Push Cat Back On Bite")]
    public float pushBackForce = 7f;         // sideways push
    public float pushUpForce = 4f;           // small hop up

    [Header("Sounds")]
    public AudioClip attackSound;
    public AudioClip hurtSound;
    public AudioClip deathSound;

    [Header("Animator (matches your BatAnimation)")]
    public string moveTrigger = "MoveTrigger";
    public string attackTrigger = "AttackTrigger";
    public string stunedTrigger = "StunedTrigger";
    public string deathTrigger = "DeathTrigger";
    public string idleStateName = "Idle";
    public string deathStateName = "Death";

    public bool IsDead => isDead;

    Rigidbody2D rb;
    Animator anim;
    PlayerHealth playerHealth;
    Collider2D playerCollider;
    readonly HashSet<string> animParams = new HashSet<string>();

    int health;
    Vector2 homePos;
    float originalScaleX;
    float lookDir = -1f;                      // -1 = looking left, +1 = looking right

    bool isDead, isStunned, isAttacking, isChasing, isLunging, biteLanded;
    bool animMoving;
    float invulnerableUntil, nextAttackTime;

    bool hasMoveTarget;
    Vector2 moveTarget, knockback, lungeTarget;

    Coroutine attackRoutine, stunRoutine;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (attackHitbox == null) attackHitbox = GetComponentInChildren<BatAttackHitbox>(true);

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        if (anim != null)
            foreach (var p in anim.parameters) animParams.Add(p.name);

        health = maxHealth;
        homePos = transform.position;
        originalScaleX = Mathf.Abs(transform.localScale.x);
        if (originalScaleX < 0.0001f) originalScaleX = 1f;
    }

    void Start()
    {
        if (player == null)
        {
            PlayerHealth found = FindObjectOfType<PlayerHealth>();
            if (found != null) player = found.transform;
        }
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerCollider = player.GetComponent<Collider2D>();
        }
        if (player == null) Log("No player found! Drag the cat into the Player slot.");
    }

    void Update()
    {
        hasMoveTarget = false;

        if (isDead || isStunned || isAttacking) return;

        bool playerAlive = player != null && (playerHealth == null || !playerHealth.IsDead);

        if (playerAlive)
        {
            float dist = DistanceToPlayer();
            if (!isChasing && dist <= detectRange) { isChasing = true; Log("Chasing cat"); }
            else if (isChasing && dist > loseRange) { isChasing = false; Log("Lost cat, going home"); }
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            Vector2 catCenter = PlayerCenter();
            FaceTowards(catCenter.x);

            // Wait beside the cat on whichever side the bat already is
            float side = rb.position.x >= catCenter.x ? 1f : -1f;
            Vector2 target = catCenter + new Vector2(side * hoverSideDistance, hoverAbovePlayer);

            bool inRange = DistanceToPlayer() <= attackRange;
            if (inRange && Time.time >= nextAttackTime)
            {
                attackRoutine = StartCoroutine(AttackRoutine());
                return;
            }

            moveTarget = target;
            hasMoveTarget = true;
        }
        else
        {
            moveTarget = homePos + Vector2.up * Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            hasMoveTarget = true;
            if (Vector2.Distance(rb.position, homePos) > 0.3f)
                FaceTowards(homePos.x);
        }

        bool moving = hasMoveTarget && Vector2.Distance(rb.position, moveTarget) > 0.3f;
        SetMovingAnim(moving);
    }

    void FixedUpdate()
    {
        if (isDead) return;

        if (isStunned)
        {
            rb.MovePosition(rb.position + knockback * Time.fixedDeltaTime);
            knockback = Vector2.MoveTowards(knockback, Vector2.zero, knockbackSpeed * 4f * Time.fixedDeltaTime);
            return;
        }

        if (isLunging)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, lungeTarget, lungeSpeed * Time.fixedDeltaTime));
            return;
        }

        if (hasMoveTarget)
            rb.MovePosition(Vector2.MoveTowards(rb.position, moveTarget, moveSpeed * Time.fixedDeltaTime));
    }

    // Facing is applied AFTER the Animator runs, so animation clips can't undo it.
    void LateUpdate()
    {
        float scaleSign = artLooksRight ? lookDir : -lookDir;
        transform.localScale = new Vector3(originalScaleX * scaleSign, transform.localScale.y, transform.localScale.z);
    }

    // ---------------- ATTACK ----------------

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        biteLanded = false;
        nextAttackTime = Time.time + attackCooldown;
        Log("ATTACK!");

        FaceTowards(PlayerCenter().x);
        PlayOneShotAnim(attackTrigger);
        PlaySound(attackSound);

        yield return new WaitForSeconds(attackWindup);

        // Lunge at the cat while biting
        lungeTarget = PlayerCenter();
        isLunging = true;
        if (attackHitbox != null) attackHitbox.Activate();

        float t = 0f;
        while (t < attackActiveTime)
        {
            if (attackHitbox == null && !biteLanded) TryBiteWithCircle();
            t += Time.deltaTime;
            yield return null;
        }

        if (attackHitbox != null) attackHitbox.Deactivate();
        isLunging = false;

        yield return new WaitForSeconds(attackRecovery);

        isAttacking = false;
        attackRoutine = null;
    }

    void TryBiteWithCircle()
    {
        Vector2 point = BiteWorldPoint();
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(point, biteRadius))
        {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                biteLanded = true;
                Log("Bite hit the cat");
                ph.TakeDamage(point, GetPushFor(ph.transform.position));
                return;
            }
        }
    }

    Vector2 BiteWorldPoint()
    {
        return (Vector2)transform.position + new Vector2(bitePoint.x * lookDir, bitePoint.y);
    }

    void CancelAttack()
    {
        if (attackRoutine != null) StopCoroutine(attackRoutine);
        attackRoutine = null;
        if (attackHitbox != null) attackHitbox.Deactivate();
        isAttacking = false;
        isLunging = false;
    }

    // ---------------- DAMAGE ----------------

    public void TakeDamage(int amount, Vector2 hitFrom)
    {
        if (isDead || Time.time < invulnerableUntil) return;

        invulnerableUntil = Time.time + invulnerableTime;
        health -= amount;
        Log("Got hit, health = " + health);
        CancelAttack();
        isChasing = true;

        if (health <= 0)
        {
            Die();
            return;
        }

        PlaySound(hurtSound);
        PlayOneShotAnim(stunedTrigger);

        Vector2 dir = (Vector2)transform.position - hitFrom;
        if (dir.sqrMagnitude < 0.001f) dir = Vector2.right;
        knockback = dir.normalized * knockbackSpeed;

        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(StunRoutine());
    }

    IEnumerator StunRoutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(hurtStunTime);
        isStunned = false;
        knockback = Vector2.zero;
        nextAttackTime = Mathf.Max(nextAttackTime, Time.time + 0.4f);
        stunRoutine = null;
    }

    void Die()
    {
        isDead = true;
        isStunned = false;
        CancelAttack();
        StopAllCoroutines();
        Log("Dead");

        PlayDeathAnim();
        PlaySound(deathSound);

        foreach (Collider2D c in GetComponentsInChildren<Collider2D>())
            c.enabled = false;

        StartCoroutine(DeathRoutine());
    }

    // Jump straight into the Death state (doesn't depend on Any State arrows / triggers)
    void PlayDeathAnim()
    {
        if (anim == null) { Log("No Animator on the bat!"); return; }

        // Clear every trigger so nothing (Stuned, Attack, Move) pulls the bat out of Death
        foreach (var p in anim.parameters)
            if (p.type == AnimatorControllerParameterType.Trigger)
                anim.ResetTrigger(p.name);

        int deathHash = Animator.StringToHash(deathStateName);
        if (anim.HasState(0, deathHash))
        {
            anim.Play(deathHash, 0, 0f);
            Log("Playing Death animation");
        }
        else
        {
            Log("No state called '" + deathStateName + "' in Base Layer! Using trigger instead.");
            SetTrigger(deathTrigger);
        }
    }

    // Your animator has Death -> Idle, so freeze on the last Death frame instead.
    IEnumerator DeathRoutine()
    {
        if (anim != null)
        {
            yield return null; // let Play() take effect

            float t = 0f;
            while (t < 5f)
            {
                AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
                bool inDeath = info.IsName(deathStateName);

                if (inDeath && (anim.IsInTransition(0) || info.normalizedTime >= 0.98f))
                    break;

                if (!inDeath && t > 0.2f)
                {
                    Log("Something pulled the bat OUT of Death (check Any State arrows with no condition)");
                    break;
                }

                t += Time.deltaTime;
                yield return null;
            }

            Log("Death animation finished, freezing last frame");
            anim.enabled = false;
        }

        yield return new WaitForSeconds(corpseStayTime);
        Destroy(gameObject);
    }

    // ---------------- ANIMATION ----------------

    void SetMovingAnim(bool moving)
    {
        if (anim == null || moving == animMoving) return;
        animMoving = moving;

        if (moving)
        {
            SetTrigger(moveTrigger);
        }
        else
        {
            ResetTrigger(moveTrigger);
            anim.CrossFade(idleStateName, 0.1f);
        }
    }

    void PlayOneShotAnim(string trigger)
    {
        ResetTrigger(moveTrigger);
        SetTrigger(trigger);
        animMoving = false;
    }

    void SetTrigger(string name)
    {
        if (anim == null) return;
        if (animParams.Contains(name)) anim.SetTrigger(name);
        else Log("Animator has no parameter called '" + name + "'");
    }

    void ResetTrigger(string name)
    {
        if (anim != null && animParams.Contains(name)) anim.ResetTrigger(name);
    }

    // ---------------- HELPERS ----------------

    // Knockback for the cat: always pushes AWAY from the bat, plus a little hop up
    public Vector2 GetPushFor(Vector2 victimPos)
    {
        float dir = victimPos.x >= transform.position.x ? 1f : -1f;
        return new Vector2(dir * pushBackForce, pushUpForce);
    }

    Vector2 PlayerCenter()
    {
        if (playerCollider != null) return playerCollider.bounds.center;
        return player != null ? (Vector2)player.position : rb.position;
    }

    float DistanceToPlayer()
    {
        if (player == null) return Mathf.Infinity;
        if (playerCollider != null && playerCollider.enabled)
            return Vector2.Distance(rb.position, playerCollider.ClosestPoint(rb.position));
        return Vector2.Distance(rb.position, player.position);
    }

    void FaceTowards(float x)
    {
        float diff = x - transform.position.x;
        if (Mathf.Abs(diff) < turnDeadZone) return;
        lookDir = Mathf.Sign(diff);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null) audioSource.PlayOneShot(clip);
    }

    void Log(string msg)
    {
        if (showDebugLogs) Debug.Log("[Bat] " + msg, this);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRange);

        if (attackHitbox == null)
        {
            float dir = Application.isPlaying ? lookDir : Mathf.Sign(transform.localScale.x) * (artLooksRight ? 1f : -1f);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bitePoint.x * dir, bitePoint.y), biteRadius);
        }
    }
}
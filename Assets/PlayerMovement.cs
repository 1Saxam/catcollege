using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private AudioClip keyPickupSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip gateOpenSound;
    [SerializeField] private AudioClip butterflySound;

    public bool canMove = false;

    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject playerCamera;
    [SerializeField] private GameObject butterflyCamera;

    private bool levelFinished = false;

    [SerializeField] private GameObject butterflies;
    [SerializeField] private Sprite openCageSprite;

    [SerializeField] private GameObject cageText;
    [SerializeField] private GameObject cageObject;

    private bool nearCage;
    private bool atTreeBottom;

    [SerializeField] private GameObject keyObject;
    [SerializeField] private GameObject keyIcon;
    [SerializeField] private GameObject pickupText;

    private bool nearKey;
    private bool hasKey;

    public Transform treeTopLanding;
    public float moveSpeed = 5f;
    private bool atTreeTop = false;

    private Rigidbody2D rb;
    private Animator animator;
    private float horizontal;

    private float originalScaleX;

    private bool nearTree = false;
    public bool isClimbing = false;

    public float climbSpeed = 3f;

    [Header("Climbing Settings")]
    public float climbHorizontalSpeed = 2f;
    public float maxClimbOffset = 1.5f;

    private float treeCenterX;
    private float currentMinX;
    private float currentMaxX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        originalScaleX = Mathf.Abs(transform.localScale.x);
    }

    void Update()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (levelFinished)
            return;

        if (!isClimbing)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            horizontal = 0;
        }

        animator.SetBool("isMoving", !isClimbing && horizontal != 0);

        bool running = Input.GetKey(KeyCode.LeftShift) && horizontal != 0 && !isClimbing;
        animator.SetBool("isRunning", running);

        if (horizontal > 0)
            transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
        else if (horizontal < 0)
            transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);

        if (nearTree && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            isClimbing = true;
            animator.SetBool("isClimbing", true);
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;

            currentMinX = treeCenterX - maxClimbOffset;
            currentMaxX = treeCenterX + maxClimbOffset;
        }

        if (isClimbing)
        {
            float vertical = Input.GetAxisRaw("Vertical");
            float climbHorizontal = Input.GetAxisRaw("Horizontal");

            animator.SetBool("isClimbingMoving", vertical != 0 || climbHorizontal != 0);

            if (climbHorizontal > 0)
                transform.localScale = new Vector3(originalScaleX, transform.localScale.y, transform.localScale.z);
            else if (climbHorizontal < 0)
                transform.localScale = new Vector3(-originalScaleX, transform.localScale.y, transform.localScale.z);

            rb.velocity = new Vector2(climbHorizontal * climbHorizontalSpeed, vertical * climbSpeed);

            float clampedX = Mathf.Clamp(rb.position.x, currentMinX, currentMaxX);
            rb.position = new Vector2(clampedX, rb.position.y);
        }
        else
        {
            animator.SetBool("isClimbingMoving", false);
        }

        if (isClimbing && atTreeTop && (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.gravityScale = 3;
            rb.velocity = Vector2.zero;

            if (treeTopLanding != null)
                transform.position = treeTopLanding.position;
        }

        if (isClimbing && atTreeBottom && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.gravityScale = 3;
            rb.velocity = Vector2.zero;
        }

        if (nearKey && !hasKey && Input.GetKeyDown(KeyCode.E))
        {
            hasKey = true;
            audioSource.PlayOneShot(keyPickupSound);
            keyIcon.SetActive(true);
            pickupText.SetActive(false);
            Destroy(keyObject);
        }

        if (nearCage && hasKey && Input.GetKeyDown(KeyCode.G))
        {
            hasKey = false;
            keyIcon.SetActive(false);
            cageText.SetActive(false);
            SpriteRenderer cageRenderer = cageObject.GetComponent<SpriteRenderer>();

            StartCoroutine(OpenGateSequence());
            cageRenderer.sprite = openCageSprite;
        }
    }

    private float knockbackTimer;
    public bool IsKnockedBack => knockbackTimer > 0f;

    public void ApplyKnockback(Vector2 pushVelocity, float duration)
    {
        if (isClimbing) return;
        knockbackTimer = duration;
        rb.velocity = pushVelocity;
    }

    void FixedUpdate()
    {
        // جلوگیری از جنگیدن فیزیک بازی با کات‌سین
        if (!canMove || levelFinished) return;

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            return;
        }

        if (isClimbing) return;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) currentSpeed = moveSpeed * 2f;

        rb.velocity = new Vector2(horizontal * currentSpeed, rb.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            nearTree = true;
            treeCenterX = other.bounds.center.x;
        }

        if (other.CompareTag("TreeTopExit")) atTreeTop = true;
        if (other.CompareTag("TreeBottomExit")) atTreeBottom = true;

        if (other.CompareTag("Key"))
        {
            nearKey = true;
            pickupText.SetActive(true);
        }

        if (other.CompareTag("Cage"))
        {
            nearCage = true;
            if (hasKey) cageText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climbable")) nearTree = false;
        if (other.CompareTag("TreeTopExit")) atTreeTop = false;
        if (other.CompareTag("TreeBottomExit")) atTreeBottom = false;

        if (other.CompareTag("Key"))
        {
            nearKey = false;
            pickupText.SetActive(false);
        }

        if (other.CompareTag("Cage"))
        {
            nearCage = false;
            cageText.SetActive(false);
        }
    }

    IEnumerator ShowLevelComplete()
    {
        yield return new WaitForSeconds(3f);
        levelCompletePanel.SetActive(true);
    }

    public void LoadLevel2()
    {
        SceneManager.LoadScene("Lvl2");
    }

    IEnumerator OpenGateSequence()
    {
        audioSource.PlayOneShot(gateOpenSound);
        yield return new WaitForSeconds(1.5f);
        audioSource.PlayOneShot(butterflySound);

        if (butterflies != null)
        {
            butterflies.SetActive(true);

            bool companionAssigned = false;

            foreach (Transform butterfly in butterflies.transform)
            {
                if (!companionAssigned)
                {
                    companionAssigned = true;

                    Animator anim = butterfly.GetComponent<Animator>();
                    if (anim != null) anim.enabled = false;

                    Rigidbody2D rb = butterfly.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = Vector2.zero;
                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }

                    ButterflyCompanion comp = butterfly.GetComponent<ButterflyCompanion>();
                    if (comp == null) comp = butterfly.gameObject.AddComponent<ButterflyCompanion>();

                    comp.target = this.transform;
                    comp.heightOffset = 2.5f;
                    comp.enabled = true;

                    break;
                }
            }

            yield return new WaitForSeconds(2f);

            foreach (Transform butterfly in butterflies.transform)
            {
                if (butterfly.GetComponent<ButterflyCompanion>() != null) continue;

                Rigidbody2D rb = butterfly.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(Random.Range(-2f, 2f), Random.Range(2f, 4f));
                }
            }

            levelFinished = true;
            playerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 5;
            butterflyCamera.GetComponent<CinemachineVirtualCamera>().Priority = 20;

            // پنل مرحله تموم شد کامنت شده تا روی کات‌سین نیاد
            // StartCoroutine(ShowLevelComplete());

            FindObjectOfType<BatCinematic>().StartCinematic();
        }
    }
}
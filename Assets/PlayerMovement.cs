using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerMovement : MonoBehaviour
{

    public float jumpForce = 10f;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private bool isGrounded;

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
    private bool isClimbing = false;

    public float climbSpeed = 3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        originalScaleX = Mathf.Abs(transform.localScale.x);

        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            Debug.Log(p.name);
        }
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


        isGrounded = Physics2D.OverlapCircle(
    groundCheck.position,
    0.2f,
    groundLayer
);


        animator.SetBool("isGrounded", isGrounded);




        if (!isClimbing)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
        }
        else
        {
            horizontal = 0;
        }

        animator.SetBool("isMoving", !isClimbing && horizontal != 0);

        bool running = Input.GetKey(KeyCode.LeftShift) &&
                       horizontal != 0 &&
                       !isClimbing;

        animator.SetBool("isRunning", running);

        if (horizontal > 0)
        {
            transform.localScale = new Vector3(
                originalScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (horizontal < 0)
        {
            transform.localScale = new Vector3(
                -originalScaleX,
                transform.localScale.y,
                transform.localScale.z
            );
        }

        if (nearTree &&
   (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)))
        {
            isClimbing = true;
            Debug.Log("Started Climbing");
            animator.SetBool("isClimbing", true);

            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }

        if (isClimbing)
        {
            float vertical = Input.GetAxisRaw("Vertical");

            animator.SetBool("isClimbingMoving", vertical != 0);

            rb.velocity = new Vector2(
                0,
                vertical * climbSpeed
            );
        }
        else
        {
            animator.SetBool("isClimbingMoving", false);
        }

        if (isClimbing &&
    atTreeTop &&
   (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)))
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.gravityScale = 3;
            rb.velocity = Vector2.zero;

            transform.position = treeTopLanding.position;
        }

        if (isClimbing &&
    atTreeBottom &&
   (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
        {
            isClimbing = false;
            animator.SetBool("isClimbing", false);
            rb.gravityScale = 3;
            rb.velocity = Vector2.zero;
        }

        if (nearKey &&
            !hasKey &&
            Input.GetKeyDown(KeyCode.E))
        {
            hasKey = true;

            keyIcon.SetActive(true);
            pickupText.SetActive(false);

            Destroy(keyObject);


        }


        if (nearCage &&
    hasKey &&
    Input.GetKeyDown(KeyCode.G))
        {
            hasKey = false;

            keyIcon.SetActive(false);
            cageText.SetActive(false);

            SpriteRenderer cageRenderer = cageObject.GetComponent<SpriteRenderer>();

            cageRenderer.sprite = openCageSprite;
            butterflies.SetActive(true);

            foreach (Transform butterfly in butterflies.transform)
            {
                Rigidbody2D rb = butterfly.GetComponent<Rigidbody2D>();

                rb.velocity = new Vector2(
                    Random.Range(-2f, 0f),
                    Random.Range(2f, 0f)
                );



                levelFinished = true;

                playerCamera.GetComponent<CinemachineVirtualCamera>().Priority = 5;

                butterflyCamera.GetComponent<CinemachineVirtualCamera>().Priority = 20;

                StartCoroutine(ShowLevelComplete());

            }


        }

        if (!isClimbing &&
    isGrounded &&
    Input.GetKeyDown(KeyCode.Space))
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                jumpForce
            );

            animator.SetBool("isJumping", true);
        }

        if (!isGrounded)
        {
            if (rb.velocity.y > 0.1f)
            {
                animator.SetBool("isJumping", true);
                ///animator.SetBool("isFalling", false);
            }
            else if (rb.velocity.y < -0.1f)
            {
                animator.SetBool("isJumping", false);
               // animator.SetBool("isFalling", true);
            }
        }
        else
        {
            animator.SetBool("isJumping", false);
           // animator.SetBool("isFalling", false);
        }
















    }

    void FixedUpdate()
    {
        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = moveSpeed * 2f;
        }

        rb.velocity = new Vector2(
            horizontal * currentSpeed,
            rb.velocity.y
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            nearTree = true;
        }

        if (other.CompareTag("TreeTopExit"))
        {
            atTreeTop = true;
        }

        if (other.CompareTag("TreeBottomExit"))
        {
            atTreeBottom = true;
        }

        if (other.CompareTag("Key"))
        {
            nearKey = true;

            pickupText.SetActive(true);
        }

        if (other.CompareTag("Cage"))
        {
            nearCage = true;

            if (hasKey)
            {
                cageText.SetActive(true);
            }
        }


    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Climbable"))
        {
            Debug.Log("Exited Tree");

            nearTree = false;
        }

        if (other.CompareTag("TreeTopExit"))
        {
            atTreeTop = false;
        }

        if (other.CompareTag("TreeBottomExit"))
        {
            atTreeBottom = false;
        }

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











    


}
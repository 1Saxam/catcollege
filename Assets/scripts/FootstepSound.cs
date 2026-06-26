using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSound : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip walkClip;
    public AudioClip runClip;
    public AudioClip jumpClip;
    public AudioClip climbClip;
    public AudioClip meowClip;

    [Header("Settings")]
    public AudioSource audioSource;
    public float walkInterval = 0.7f;
    public float runInterval = 0.6f;
    public float climbInterval = 0.5f;

    [Header("Ground Check (same as PlayerJump)")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private float timer = 0f;
    private bool wasClimbing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // اگر groundCheck تنظیم نشده، از GameObject خود گربه پیدا کن
        if (groundCheck == null)
            groundCheck = transform.Find("GroundCheck");

        // اگر groundCheck پیدا نشد، خطا بده
        if (groundCheck == null)
            Debug.LogError("❌ GroundCheck not found! Please assign it in Inspector.");
    }

    void Update()
    {
        bool isClimbing = anim.GetBool("isClimbing");
        bool isRunning = anim.GetBool("isRunning");
        float speed = Mathf.Abs(rb.velocity.x);

        // ✅ تشخیص زمین با Physics (مستقل از doLand)
        bool isGrounded = false;
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        }

        // اگر روی زمین نیست و در حال بالا رفتن نیست، صدا پخش نشود
        if (!isGrounded && !isClimbing)
        {
            timer = 0f;
            return;
        }

        // ===== شروع بالا رفتن =====
        if (isClimbing && !wasClimbing)
        {
            wasClimbing = true;
            PlayClimbStart();
            timer = 0f;
            return;
        }
        if (!isClimbing) wasClimbing = false;

        // ===== انتخاب صدای مناسب =====
        float currentInterval = walkInterval;
        AudioClip currentClip = walkClip;

        if (isClimbing)
        {
            currentInterval = climbInterval;
            currentClip = climbClip;
        }
        else if (isRunning && speed > 1f)
        {
            currentInterval = runInterval;
            currentClip = runClip != null ? runClip : walkClip;
        }
        else if (speed < 0.1f) // ✅ آستانه‌ی کم‌تر برای تشخیص حرکت
        {
            timer = 0f;
            return;
        }

        if (currentClip == null) return;

        timer += Time.deltaTime;
        if (timer >= currentInterval)
        {
            PlaySFX(currentClip, 0.1f);
            timer = 0f;
        }
    }

    public void PlayJumpSound() { if (jumpClip != null) PlaySFX(jumpClip, 0.05f, 5f); }
    public void PlayMeow() { if (meowClip != null) PlaySFX(meowClip, 0.05f); }
    public void PlayClimbStart() { if (climbClip != null) PlaySFX(climbClip, 0.05f); }

    void PlaySFX(AudioClip clip, float pitchVariation, float volume = 1f)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(clip, pitchVariation, volume);
        else if (audioSource != null)
        {
            audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            audioSource.PlayOneShot(clip, volume);
        }
    }
}

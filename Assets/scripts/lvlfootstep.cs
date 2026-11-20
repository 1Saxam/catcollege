using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvlfootstep : MonoBehaviour
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

    [Header("Stealth (Butterfly Companion)")]
    public GameObject butterflyCompanion; 
    public bool isSilent = false;

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
    }

    void Update()
    {
        //---------------
        // فقط وقتی دکمه شیفت رو نگه داشتی بی صدا میشی
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            isSilent = true;
        }
        else
        {
            isSilent = false;
        }
        //------------------
        bool isClimbing = anim.GetBool("isClimbing");
        bool isRunning = anim.GetBool("isRunning");
        float speed = Mathf.Abs(rb.velocity.x);

        
        if (isClimbing && !wasClimbing)
        {
            wasClimbing = true;
            if (!isSilent) PlayClimbStart();
            timer = 0f;
            return;
        }
        if (!isClimbing) wasClimbing = false;

        
        bool isGrounded = !anim.GetBool("doLand"); 
        if (!isGrounded && !isClimbing) { timer = 0f; return; }

        
        float currentInterval = walkInterval;
        AudioClip currentClip = walkClip;

        if (isClimbing)
        {
            
            if (Mathf.Abs(rb.velocity.y) < 0.1f && Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                timer = 0f;
                return;
            }

            if (isSilent) { timer = 0f; return; }

            currentInterval = climbInterval;
            currentClip = climbClip;
        }
        else if (isRunning && speed > 1f)
        {
            if (isSilent) { timer = 0f; return; }

            currentInterval = runInterval;
            currentClip = runClip != null ? runClip : walkClip;
        }
        else if (speed < 0.5f) { timer = 0f; return; }

        if (currentClip == null) return;

        timer += Time.deltaTime;
        if (timer >= currentInterval)
        {
            if (!isSilent) PlaySFX(currentClip, 0.1f);  
            timer = 0f;
        }
    }

    public void PlayJumpSound() { if (jumpClip != null) PlaySFX(jumpClip, 0.05f, 2f); }
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
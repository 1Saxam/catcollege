using UnityEngine;
using System.Collections;

public class SleepingBat : MonoBehaviour
{
    [Header("Bat Settings")]
    public float knockbackForce = 6f;
    public GameObject hitEffectPrefab;

    [Header("Visuals")]
    public SpriteRenderer batSprite;
    public Color sleepColor = new Color(0.5f, 0.5f, 0.5f, 1f); // رنگ خاکستری/تیره برای خواب
    public Color awakeColor = Color.white; // رنگ عادی

    private bool isAwake = false;
    private bool canHit = true;

    void Start()
    {
        if (batSprite == null) batSprite = GetComponent<SpriteRenderer>();
        batSprite.color = sleepColor; // در ابتدا خوابیده و خاکستری است
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isAwake && canHit)
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null && player.isClimbing)
            {
                // ✨ چک میکنیم آیا بازیکن دکمه Shift رو نگه داشته یا نه ✨
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    Debug.Log("گربه بی صدا رد شد. خفاش بیدار نشد!");
                    return; // خفاش کاری نمیکنه و خواب میمونه
                }

                // اگر Shift رو نگه نداشته باشه، خفاش بیدار میشه و ضربه میزنه
                StartCoroutine(WakeUpAndAttack(other));
            }
        }
    }

    IEnumerator WakeUpAndAttack(Collider2D playerCollider)
    {
        isAwake = true;
        canHit = false;
        batSprite.color = awakeColor; // تغییر رنگ به حالت بیدار

        // ۱. کم کردن جون گربه
        PlayerHealth health = playerCollider.GetComponent<PlayerHealth>();
        if (health != null) health.TakeDamage();

        // ۲. پرت کردن گربه به پایین
        Rigidbody2D rb = playerCollider.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = new Vector2(0, -knockbackForce);

        // ۳. افکت
        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, playerCollider.transform.position, Quaternion.identity);

        // ۴. تکون خوردن خفاش
        Vector3 originalPos = transform.localPosition;
        transform.localPosition += new Vector3(0.2f, -0.2f, 0);
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = originalPos;

        // ۵. بعد از چند ثانیه دوباره می‌خوابه تا بازیکن بتونه دوباره تلاش کنه
        yield return new WaitForSeconds(2f);
        isAwake = false;
        canHit = true;
        batSprite.color = sleepColor; // برمی‌گرده به حالت خواب
    }
}
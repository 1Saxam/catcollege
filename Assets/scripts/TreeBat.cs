using UnityEngine;
using System.Collections;

public class TreeBat : MonoBehaviour
{
    [Header("Bat Settings")]
    public float knockbackForce = 8f; // نیروی پرت شدن (کمی بیشتر از شاخه‌های چوبی)
    public GameObject hitEffectPrefab; // افکت دود یا ستاره

    private bool canHit = true; // جلوگیری از ضربه خوردن چندباره در یک لحظه

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canHit) return;

        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            // فقط وقتی گربه داره از درخت بالا میره این اتفاق بیفته
            if (player != null && player.isClimbing)
            {
                canHit = false; // غیرفعال کردن موقت

                // ۱. کم کردن قلب
                if (health != null)
                {
                    health.TakeDamage();
                }

                // ۲. پرت شدن به پایین
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(0, -knockbackForce);
                }

                // ۳. نمایش افکت
                if (hitEffectPrefab != null)
                {
                    Instantiate(hitEffectPrefab, other.transform.position, Quaternion.identity);
                }

                // ۴. یک انیمیشن ساده (اختیاری): خفاش یکم تکون بخوره
                StartCoroutine(BatHitReaction());
            }
        }
    }

    IEnumerator BatHitReaction()
    {
        // یک تکون ساده به خفاش میده تا حس کنه بیدار شد
        Vector3 originalPos = transform.localPosition;
        transform.localPosition += new Vector3(0.2f, -0.2f, 0);
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = originalPos;

        // بعد از ۱.۵ ثانیه دوباره میتونه ضربه بزنه (اگر بازیکن دوباره اومد)
        yield return new WaitForSeconds(1.5f);
        canHit = true;
    }
}
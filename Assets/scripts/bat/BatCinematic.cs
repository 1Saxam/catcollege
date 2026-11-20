using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BatCinematic : MonoBehaviour
{
    [Header("عکس‌ها و آبجکت‌ها")]
    public GameObject giantBat;       // آبجکت خفاش بزرگ
    public Transform cat;             // گربه
    public GameObject portal;         // پرتال (که اول بازی خاموشه)
    public string nextSceneName = "Lvl2"; // اسم مرحله بعدی

    public void StartCinematic()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // ۱. قفل کردن گربه
        PlayerMovement player = cat.GetComponent<PlayerMovement>();
        if (player != null) player.canMove = false;

        Rigidbody2D rb = cat.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        // ۲. خفاش بزرگ از راست میاد
        if (giantBat != null)
        {
            giantBat.SetActive(true);
            Vector3 startPos = cat.position + new Vector3(15f, 3f, 0); // از راست
            Vector3 endPos = cat.position + new Vector3(0, 1f, 0);      // بالای سر گربه

            giantBat.transform.position = startPos;
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 2f; // سرعت پرواز
                giantBat.transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
        }

        // ۳. ضربه! گربه پرت میشه بالا
        if (rb != null)
        {
            rb.gravityScale = 2f;
            rb.velocity = new Vector2(0, 10f);
        }

        // ۴. خفاش فرار میکنه
        if (giantBat != null)
            giantBat.transform.position += new Vector3(20f, 10f, 0);

        yield return new WaitForSeconds(1.5f); // صبر میکنیم گربه بیاد پایین

        // ۵. باز شدن پرتال زیر گربه
        if (portal != null)
        {
            portal.SetActive(true);
            portal.transform.position = new Vector3(cat.position.x, cat.position.y - 4f, 0);
            portal.transform.localScale = Vector3.zero;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 3f;
                portal.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(2f, 4f, 1f), t);
                yield return null;
            }
        }

        // ۶. مکیده شدن گربه به داخل پرتال
        yield return new WaitForSeconds(0.5f);

        Vector3 startPosCat = cat.position;
        Vector3 endPosCat = portal.transform.position;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime * 2f;
            cat.position = Vector3.Lerp(startPosCat, endPosCat, time);
            cat.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time); // کوچیک میشه
            yield return null;
        }

        // ۷. رفتن به مرحله بعد
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }
}
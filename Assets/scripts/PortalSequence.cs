using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class PortalSequence : MonoBehaviour
{
    [Header("مراجع (از صحنه)")]
    public Transform player;
    public Transform butterfly;
    public string nextSceneName = "Lvl3";

    [Header("آرت‌ها (عکس‌هایی که فرستادی)")]
    public Sprite bossBatSprite;      // خفاش بیدار (بزرگ)
    public Sprite smallBatSprite;     // خفاش بیدار (کوچیک)
    public Sprite portalSprite;       // پرتال بنفش

    [Header("تنظیمات خفاش بزرگ")]
    public float bossSpeed = 22f;
    public float bossEntryTime = 1.2f;

    [Header("تنظیمات پرت کردن گربه")]
    public float throwDuration = 1f;
    public float throwArcHeight = 2f;

    [Header("تنظیمات خفاش‌های کوچیک")]
    public int smallBatCount = 25;
    public float swarmRadius = 10f;

    [Header("تنظیمات پرتال")]
    public float portalRadius = 1.5f;

    // داخلی
    bool started;
    PlayerMovement movement;
    Rigidbody2D rb;
    Transform shieldObj;
    Transform portalObj;
    Sprite circleSprite;

    void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;

        if (player == null)
        {
            var p = FindObjectOfType<PlayerHealth>();
            if (p != null) player = p.transform;
        }
        if (player == null) { enabled = false; return; }

        movement = player.GetComponent<PlayerMovement>();
        rb = player.GetComponent<Rigidbody2D>();

        if (butterfly == null)
        {
            var bc = FindObjectOfType<ButterflyCompanion>();
            if (bc != null) butterfly = bc.transform;
        }

        // یک دایره ساده فقط برای ساخت سپر پروانه (چون عکس سپر نداشتی)
        circleSprite = MakeSprite(GenCircle(256, 1.6f));
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (started) return;
        if (other.transform != player && other.GetComponent<PlayerMovement>() == null) return;
        started = true;
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // ==== ۱. قفل کردن بازیکن ====
        if (movement != null) { movement.canMove = false; movement.enabled = false; }
        var slope = player.GetComponent<SlopeAlign>();
        if (slope != null) slope.enabled = false;
        foreach (var bat in FindObjectsOfType<BatEnemy>()) bat.gameObject.SetActive(false);
        foreach (var bat in FindObjectsOfType<SleepingBat>()) bat.gameObject.SetActive(false);

        if (rb != null) { rb.velocity = Vector2.zero; rb.gravityScale = 0f; }

        yield return new WaitForSeconds(0.5f);

        // ==== ۲. خفاش بزرگ از راست میاد ====
        Transform boss = MakeBoss();
        Vector3 bossStart = player.position + Vector3.right * 15f + Vector3.up * 2f;
        Vector3 bossTarget = player.position + Vector3.up * 1.5f;
        boss.position = bossStart;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / bossEntryTime;
            float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            boss.position = Vector3.Lerp(bossStart, bossTarget, e);
            yield return null;
        }
        yield return new WaitForSeconds(0.15f);

        // ==== ۳. ضربه پا! (فلاش سفید) ====
        var flash = MakeCircleFX(bossTarget, Color.white, 3f, 0.3f);
        StartCoroutine(FadeDestroy(flash, 0.3f));

        // ==== ۴. فرار خفاش ====
        Vector3 bossEnd = bossTarget + new Vector3(15f, 12f, 0f);
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 0.9f;
            float e = Mathf.Clamp01(t); e *= e;
            boss.position = Vector3.Lerp(bossTarget, bossEnd, e);
            yield return null;
        }
        if (boss != null) Destroy(boss.gameObject);

        // ==== ۵. پرت کردن گربه با قوس ====
        Vector3 ts = player.position;
        Vector3 te = player.position + Vector3.down * 5f;
        Vector3 tm = (ts + te) * 0.5f + Vector3.up * throwArcHeight;

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / throwDuration;
            float e = Mathf.Clamp01(t);
            Vector3 a = Vector3.Lerp(ts, tm, e);
            Vector3 b = Vector3.Lerp(tm, te, e);
            player.position = Vector3.Lerp(a, b, e);
            player.rotation = Quaternion.Euler(0, 0, e * 360f);
            yield return null;
        }
        player.rotation = Quaternion.identity;
        player.position = te;

        yield return new WaitForSeconds(0.8f);

        // ==== ۶. خفاش‌های کوچیک ====
        StartCoroutine(SpawnSwarm());

        yield return new WaitForSeconds(0.6f);

        // ==== ۷. سپر پروانه ====
        shieldObj = MakeShield();
        StartCoroutine(ShieldFollow());

        yield return new WaitForSeconds(3.5f);

        // ==== ۸. پرتال ====
        Vector3 portalPos = player.position + Vector3.down * 3f;
        portalObj = MakePortal(portalPos);
        yield return new WaitForSeconds(0.8f);

        // ==== ۹. سقوط در پرتال ====
        if (butterfly != null)
        {
            var bc = butterfly.GetComponent<ButterflyCompanion>();
            if (bc != null) bc.heightOffset = 1.1f;
        }

        Vector3 fs = player.position;
        Vector3 fe = portalPos;
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 1.2f;
            float e = Mathf.Clamp01(t);
            float easeIn = e * e;

            player.position = Vector3.Lerp(fs, fe, easeIn);
            float s = Mathf.Lerp(1f, 0f, easeIn);
            var sc = player.localScale;
            player.localScale = new Vector3(Mathf.Sign(sc.x) * Mathf.Abs(sc.x) * s, sc.y * s, 1f);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // ==== ۱۰. صحنه بعدی ====
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
    }

    // ========== سازنده‌ها ==========

    Transform MakeBoss()
    {
        var go = new GameObject("BigBat");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = bossBatSprite;
        sr.sortingOrder = 30;
        // چرخوندن خفاش به سمت چپ (چون از راست میاد)
        go.transform.localScale = new Vector3(-3f, 3f, 1f);
        return go.transform;
    }

    Transform MakePortal(Vector3 pos)
    {
        var go = new GameObject("Portal");
        go.transform.position = pos;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = portalSprite;
        sr.sortingOrder = 10;
        // کشیدگی عمودی برای پرتال
        go.transform.localScale = new Vector3(2f, 4f, 1f);

        StartCoroutine(PortalAnim(go.transform));
        StartCoroutine(OpenScale(go.transform, 0.8f));
        return go.transform;
    }

    IEnumerator PortalAnim(Transform portal)
    {
        while (portal != null)
        {
            portal.Rotate(0, 0, -100f * Time.deltaTime); // چرخش پرتال
            yield return null;
        }
    }

    IEnumerator SpawnSwarm()
    {
        for (int i = 0; i < smallBatCount; i++)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * swarmRadius;
            Vector3 spawnPos = player.position + (Vector3)offset;

            var go = new GameObject("SmallBat");
            go.transform.position = spawnPos;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = smallBatSprite;
            sr.sortingOrder = 28;

            // چرخوندن کوچیک‌ها به سمت بازیکن
            if (spawnPos.x < player.position.x) go.transform.localScale = new Vector3(-0.5f, 0.5f, 1f);
            else go.transform.localScale = new Vector3(0.5f, 0.5f, 1f);

            StartCoroutine(SmallBatBehavior(go.transform, i * 0.05f));
        }
        yield return null;
    }

    IEnumerator SmallBatBehavior(Transform bat, float delay)
    {
        yield return new WaitForSeconds(delay);
        float life = 0f;
        while (bat != null && life < 4f)
        {
            life += Time.deltaTime;
            if (player == null) yield break;

            Vector3 toPlayer = player.position - bat.position;
            float dist = toPlayer.magnitude;

            bool shielded = shieldObj != null;
            if (shielded && dist < 2f) // برخورد با سپر
            {
                Vector2 away = toPlayer.normalized * -1f;
                float t = 0f;
                while (t < 0.7f && bat != null)
                {
                    t += Time.deltaTime;
                    bat.position += (Vector3)(away * 8f * Time.deltaTime) + Vector3.up * 3f * Time.deltaTime;
                    bat.Rotate(0, 0, 800f * Time.deltaTime);
                    var sr = bat.GetComponent<SpriteRenderer>();
                    if (sr != null) sr.color = new Color(1, 1, 1, 1f - t / 0.7f);
                    yield return null;
                }
                if (bat != null) Destroy(bat.gameObject);
                yield break;
            }

            if (dist > 0.1f) bat.position += toPlayer.normalized * 5f * Time.deltaTime;
            yield return null;
        }
        if (bat != null) Destroy(bat.gameObject);
    }

    // سپر ساده پروانه (چون عکسی نداشتی)
    Transform MakeShield()
    {
        var go = new GameObject("Shield");

        var glow = new GameObject("Glow");
        glow.transform.SetParent(go.transform, false);
        var gsr = glow.AddComponent<SpriteRenderer>();
        gsr.sprite = circleSprite;
        gsr.color = new Color(1f, 0.92f, 0.6f, 0.6f); // طلایی درخشان
        gsr.sortingOrder = 25;
        glow.transform.localScale = Vector3.one * 4f;

        go.transform.position = player.position + Vector3.up * 1.2f;
        StartCoroutine(SpinRings(glow.transform));
        return go.transform;
    }

    IEnumerator SpinRings(Transform glow)
    {
        float t = 0f;
        while (glow != null)
        {
            t += Time.deltaTime;
            float pulse = 1f + Mathf.Sin(t * 5f) * 0.1f;
            if (glow != null) glow.transform.localScale = Vector3.one * 4f * pulse;
            yield return null;
        }
    }

    IEnumerator ShieldFollow()
    {
        while (shieldObj != null && player != null)
        {
            shieldObj.position = player.position + Vector3.up * 1.2f;
            yield return null;
        }
    }

    // ========== ابزارهای کمکی ==========

    IEnumerator OpenScale(Transform tr, float dur)
    {
        tr.localScale = Vector3.zero;
        float t = 0f;
        while (t < 1f && tr != null)
        {
            t += Time.deltaTime / dur;
            float e = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
            tr.localScale = new Vector3(2f * e, 4f * e, 1f);
            yield return null;
        }
        if (tr != null) tr.localScale = new Vector3(2f, 4f, 1f);
    }

    Transform MakeCircleFX(Vector3 pos, Color c, float size, float life)
    {
        var go = new GameObject("FX");
        go.transform.position = pos;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = circleSprite;
        sr.color = c;
        sr.sortingOrder = 40;
        go.transform.localScale = Vector3.one * size;
        return go.transform;
    }

    IEnumerator FadeDestroy(Transform fx, float dur)
    {
        float t = 0f;
        var sr = fx.GetComponent<SpriteRenderer>();
        while (t < dur && fx != null)
        {
            t += Time.deltaTime;
            if (sr != null) sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f - t / dur);
            yield return null;
        }
        if (fx != null) Destroy(fx.gameObject);
    }

    Texture2D GenCircle(int size, float falloff)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        float half = size * 0.5f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = (x + 0.5f - half) / half;
                float dy = (y + 0.5f - half) / half;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float a = Mathf.Pow(Mathf.Clamp01(1f - d), falloff);
                tex.SetPixel(x, y, new Color(1, 1, 1, a));
            }
        tex.Apply();
        return tex;
    }

    Sprite MakeSprite(Texture2D tex)
        => Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), tex.width);
}
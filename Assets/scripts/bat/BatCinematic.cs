using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Cinemachine;

public class BatCinematic : MonoBehaviour
{
    [Header("مراجع بازی")]
    public GameObject giantBat;
    public Transform cat;
    public GameObject lightShield;
    public Transform butterfly;
    public GameObject portal;
    public string nextSceneName = "Lvl2";

    [Header("دوربین سینمایی")]
    public CinemachineVirtualCamera cinematicCamera;
    public Transform cameraTarget;

    public void StartCinematic()
    {
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        PlayerMovement player = cat.GetComponent<PlayerMovement>();
        if (player != null) player.canMove = false;

        Rigidbody2D rb = cat.GetComponent<Rigidbody2D>();
        if (rb != null) { rb.velocity = Vector2.zero; rb.gravityScale = 0f; } // خاموش کردن جاذبه

        if (cinematicCamera != null) cinematicCamera.Priority = 30;
        if (cameraTarget != null) cameraTarget.position = cat.position;

        yield return new WaitForSeconds(0.5f);

        // ۱. خفاش میاد
        if (giantBat != null)
        {
            giantBat.SetActive(true);
            Vector3 startPos = cat.position + new Vector3(15f, 2f, 0);
            Vector3 endPos = cat.position + new Vector3(0, 1.5f, 0);
            giantBat.transform.position = startPos;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 2.5f;
                giantBat.transform.position = Vector3.Lerp(startPos, endPos, t);
                if (cameraTarget != null) cameraTarget.position = Vector3.Lerp(cat.position, giantBat.transform.position, t);
                yield return null;
            }
        }

        // ۲. لحظه برخورد (اسلوموشن + روشن شدن سپر)
        Time.timeScale = 0.2f; // اسلوموشن سنگین برای هیجان
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (lightShield != null)
        {
            lightShield.SetActive(true);
            lightShield.transform.localScale = Vector3.zero;
            StartCoroutine(GrowShield());
        }

        if (giantBat != null)
            giantBat.transform.position += new Vector3(20f, 15f, 0); // فرار خفاش

        if (cameraTarget != null) cameraTarget.position = cat.position;

        yield return new WaitForSecondsRealtime(0.8f);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // ۳. باز شدن پرتال در فاصله مشخص (مثلا 6 واحد پایین تر و 2 واحد دورتر)
        Vector3 portalPos = cat.position + new Vector3(3f, -6f, 0);

        if (portal != null)
        {
            portal.SetActive(true);
            portal.transform.position = portalPos;
            portal.transform.localScale = Vector3.zero;

            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * 3f;
                portal.transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(2f, 4f, 1f), t);
                yield return null;
            }
        }

        // ۴. پرت شدن مستقیم گربه به سمت پرتال (مثل شهاب سنگ)
        yield return new WaitForSeconds(0.2f);

        Vector3 startPosCat = cat.position;
        ButterflyCompanion comp = butterfly != null ? butterfly.GetComponent<ButterflyCompanion>() : null;

        float time = 0;
        float throwSpeed = 1.2f; // سرعت پرت شدن
        while (time < 1f)
        {
            time += Time.deltaTime * throwSpeed;

            // حرکت مستقیم و صاف به سمت پرتال
            cat.position = Vector3.Lerp(startPosCat, portalPos, time);
            cat.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);

            // چرخش گربه در هوا برای جذابیت بیشتر
            cat.Rotate(0, 0, 1000f * Time.deltaTime);

            if (butterfly != null && comp != null)
            {
                comp.heightOffset = Mathf.Lerp(1f, 0f, time);
                butterfly.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, time);
            }

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        // ۵. پاک کردن سیو و رفتن به مرحله بعد
        PlayerPrefs.SetInt("HasSavedGame", 0);
        PlayerPrefs.DeleteKey("SavedScene");
        PlayerPrefs.DeleteKey("SavedPosX");
        PlayerPrefs.DeleteKey("SavedPosY");
        PlayerPrefs.Save();

        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator GrowShield()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * 2f;
            lightShield.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * 2f, t);
            yield return null;
        }
    }
}
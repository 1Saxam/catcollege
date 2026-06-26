using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip menuMusic;
    public AudioClip level1Music;
    public AudioClip level2Music;

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    private bool isMuted = false;
    private string currentScene = "";

    void Awake()
    {
        // الگوی Singleton: فقط یک نمونه در کل بازی وجود داشته باشد
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // اگر AudioSource ها تنظیم نشده بودند، بساز
        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null)
            sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        // گوش‌دادن به تغییر Scene
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        ApplyVolumes();
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    public void PlayMusicForScene(string sceneName)
    {
        //Debug.Log($"🎵 Scene name received: {sceneName}");
        //AudioClip clip = null;

        //switch (sceneName.ToLower())
        //{
        //    case "menu":
        //        clip = menuMusic;
        //        break;
        //    case "SampleScene":
        //        clip = level1Music;
        //        break;
        //    case "lvl2":
        //        clip = level2Music;
        //        break;
        //    default:
        //        clip = menuMusic;
        //        break;
        //}

        //Debug.Log($"🎯 clip name: {clip?.name ?? "null"}");
        //Debug.Log($"🎯 current musicSource.clip name: {musicSource.clip?.name ?? "null"}");

        //if (clip == null)
        //{
        //    Debug.LogError($"❌ Clip is NULL for scene: {sceneName}! Check Inspector.");
        //    return;
        //}

        //// اگر کلیپ جدید با کلیپ فعلی فرق دارد
        //if (clip != musicSource.clip)
        //{
        //    musicSource.Stop();               // ← آهنگ قبلی را متوقف کن
        //    musicSource.clip = clip;          // ← کلیپ جدید را تنظیم کن
        //    musicSource.Play();               // ← پخش کن
        //    Debug.Log($"✅ Changed to new music: {clip.name} for scene: {sceneName}");
        //}
        //else
        //{
        //    Debug.Log($"⏩ Clip is already playing: {clip.name}");
        //}

        //// بررسی نهایی: آیا کلیپ تنظیم شده است؟
        //Debug.Log($"📢 After change: musicSource.clip = {musicSource.clip?.name ?? "null"}");
        // یک دیکشنری برای نگاشت نام Scene به کلیپ
        var musicMap = new Dictionary<string, AudioClip>(System.StringComparer.OrdinalIgnoreCase)
    {
        { "menu", menuMusic },
        { "samplescene", level1Music },
        { "lvl2", level2Music }
    };

        // تلاش برای پیدا کردن کلیپ
        AudioClip clip = null;
        if (musicMap.TryGetValue(sceneName, out clip))
        {
            Debug.Log($"🎯 Found clip for scene: {sceneName} -> {clip?.name ?? "null"}");
        }
        else
        {
            clip = menuMusic;
            Debug.Log($"⚠️ No clip found for {sceneName}, using menu music.");
        }

        if (clip == null)
        {
            Debug.LogError($"❌ Clip is NULL for scene: {sceneName}! Check Inspector.");
            return;
        }

        if (clip != musicSource.clip)
        {
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
            Debug.Log($"✅ Changed to: {clip.name}");
        }
        else
        {
            Debug.Log($"⏩ Already playing: {clip.name}");
        }
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        ApplyVolumes();
    }

    public void ToggleMute()
    {
        isMuted = !isMuted;
        ApplyVolumes();
    }

    public void SetMute(bool mute)
    {
        isMuted = mute;
        ApplyVolumes();
    }

    public void PlaySFX(AudioClip clip, float pitchVariation = 0f, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        sfxSource.PlayOneShot(clip, volume);
    }

    void ApplyVolumes()
    {
        float musicFinal = masterVolume * musicVolume * (isMuted ? 0f : 1f);
        float sfxFinal = masterVolume * sfxVolume * (isMuted ? 0f : 1f);

        if (musicSource != null) musicSource.volume = musicFinal;
        if (sfxSource != null) sfxSource.volume = sfxFinal;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
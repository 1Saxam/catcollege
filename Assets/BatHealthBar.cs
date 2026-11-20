using UnityEngine;

// Add this to the BAT (same object as BatEnemy). It builds the bar by itself - no sprites or Canvas needed.
// The bar is NOT a child of the bat, so it doesn't mirror when the bat turns around.
[RequireComponent(typeof(BatEnemy))]
public class BatHealthBar : MonoBehaviour
{
    [Header("Position & Size")]
    public Vector2 offset = new Vector2(0f, 1.2f);   // move the bar above the bat's head
    public float width = 1f;
    public float height = 0.12f;

    [Header("Colors")]
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);
    public Color fillColor = new Color(0.9f, 0.1f, 0.1f, 1f);

    [Header("Behaviour")]
    public bool hideWhenFull = false;   // tick = bar only appears after the bat gets hit
    public float drainSpeed = 3f;       // how fast the bar slides down

    BatEnemy bat;
    GameObject barRoot;
    Transform fill;
    float shownFraction = 1f;

    static Sprite pixelSprite;

    void Start()
    {
        bat = GetComponent<BatEnemy>();
        BuildBar();
    }

    void BuildBar()
    {
        if (pixelSprite == null)
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.filterMode = FilterMode.Point;
            tex.Apply();
            // pivot on the LEFT edge, 1 pixel = 1 unit
            pixelSprite = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0f, 0.5f), 1f);
        }

        SpriteRenderer batRenderer = GetComponentInChildren<SpriteRenderer>();
        int layerId = batRenderer != null ? batRenderer.sortingLayerID : 0;
        int order = batRenderer != null ? batRenderer.sortingOrder : 0;

        barRoot = new GameObject(name + " HealthBar");

        SpriteRenderer back = CreatePart("Background", backgroundColor, layerId, order + 20);
        back.transform.localScale = new Vector3(width, height, 1f);

        SpriteRenderer fillRenderer = CreatePart("Fill", fillColor, layerId, order + 21);
        fill = fillRenderer.transform;
        fill.localScale = new Vector3(width, height, 1f);
    }

    SpriteRenderer CreatePart(string partName, Color color, int layerId, int order)
    {
        GameObject go = new GameObject(partName);
        go.transform.SetParent(barRoot.transform, false);
        go.transform.localPosition = new Vector3(-width / 2f, 0f, 0f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = pixelSprite;
        sr.color = color;
        sr.sortingLayerID = layerId;
        sr.sortingOrder = order;
        return sr;
    }

    void LateUpdate()
    {
        if (barRoot == null) return;

        if (bat == null || bat.IsDead)
        {
            barRoot.SetActive(false);
            return;
        }

        float target = Mathf.Clamp01((float)bat.CurrentHealth / bat.maxHealth);
        shownFraction = Mathf.MoveTowards(shownFraction, target, drainSpeed * Time.deltaTime);

        barRoot.SetActive(!(hideWhenFull && target >= 1f));
        barRoot.transform.position = (Vector2)transform.position + offset;
        fill.localScale = new Vector3(width * shownFraction, height, 1f);
    }

    void OnDestroy()
    {
        if (barRoot != null) Destroy(barRoot);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = (Vector2)transform.position + offset;
        Gizmos.DrawWireCube(center, new Vector3(width, height, 0f));
    }
}
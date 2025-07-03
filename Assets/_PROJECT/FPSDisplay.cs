using TMPro;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    public static FPSDisplay Instance;

    [SerializeField] private TextMeshProUGUI _fpsText;

    private float timer;
    private bool visible = true;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!visible) return;

        timer += Time.deltaTime;
        if (timer >= 0.5f)
        {
            int fps = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
            _fpsText.text = $"{fps} FPS";
            timer = 0f;
        }
    }

    public void SetVisible(bool state)
    {
        visible = state;
        _fpsText.gameObject.SetActive(state);
    }
}

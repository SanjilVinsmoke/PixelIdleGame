using UnityEngine;
using UnityEngine.UI;

public class UiFullScreenManager : MonoBehaviour
{
    [SerializeField] private bool isFullScreen = false; 
    [SerializeField] private GameObject foreGround;
    private RectTransform rectTransform;

    void Start()
    {
        if (foreGround != null)
        {
            rectTransform = foreGround.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("Foreground GameObject is not assigned!");
        }
    }

    void Update()
    {
        if (rectTransform != null)
        {
            if (isFullScreen)
            {
                SetFullScreen();
            }
            else
            {
                SetNormalScreen();
            }
        }
    }

    public void ToggleFullScreen()
    {
        isFullScreen = !isFullScreen;
        Update(); // Apply changes immediately
    }

    private void SetFullScreen()
    {
        rectTransform.anchorMin = Vector2.zero;    // Bottom-left
        rectTransform.anchorMax = Vector2.one;     // Top-right
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void SetNormalScreen()
    {

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = new Vector2(720,1280);
    }
}

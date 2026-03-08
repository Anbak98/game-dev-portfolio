public class DynamicCanvasScaler : MonoBehaviour
{
    [SerializeField] private CanvasScaler scaler;
    [SerializeField] UnityEvent scaleMethods;

    void Awake()
    {
        AutoCanvasMatch();
    }

    void AutoCanvasMatch()
    {
        float aspect = (float)Screen.width / Screen.height; // 현재 비율
        float referenceAspect = scaler.referenceResolution.x / scaler.referenceResolution.y; // 기준 비율 (예: 1920/1080)
        scaler.matchWidthOrHeight = aspect > referenceAspect ? 1 : 0;

        scaleMethods?.Invoke();
        
    }

    void OnRectTransformDimensionsChange()
    {
        AutoCanvasMatch();
    }
}
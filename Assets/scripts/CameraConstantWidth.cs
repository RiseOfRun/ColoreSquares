using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraConstantWidth : MonoBehaviour
{
    [Tooltip("Required to keep the camera width constant on all aspect ratios")]
    public Vector2 DefaultResolution = new Vector2(720, 1280);

    //public bool TabletCompensation;

    private Camera componentCamera;
    private float initialSize;
    private float targetAspect;

    private float initialFov;
    private float horizontalFov;

    public float InitialSize
    {
        get => initialSize;
        set => initialSize = value;
    }

    public float InitialFov
    {
        get => initialFov;
        set
        {
            initialFov = value;
            horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);
        }
    }

    private void Awake()
    {
        componentCamera = GetComponent<Camera>();
        initialSize = componentCamera.orthographicSize;

        targetAspect = DefaultResolution.x / DefaultResolution.y;

        initialFov = componentCamera.fieldOfView;
        horizontalFov = CalcVerticalFov(initialFov, 1 / targetAspect);
    }

    private void Update()
    {
        if (componentCamera.orthographic)
        {
            componentCamera.orthographicSize = initialSize * (targetAspect / componentCamera.aspect);
        }
        else
        {
            float resultFov = CalcVerticalFov(horizontalFov, componentCamera.aspect);

            /*if (IsTablet())
            {
                resultFov = Mathf.Lerp(resultFov, initialFov, 0.8f);
            }*/

            componentCamera.fieldOfView = resultFov;
        }
    }

    public static bool IsTablet()
    {
        return Camera.main.aspect > 0.65f;
    }

    private float CalcVerticalFov(float hFovInDeg, float aspectRatio)
    {
        float hFovInRads = hFovInDeg * Mathf.Deg2Rad;

        float vFovInRads = 2 * Mathf.Atan(Mathf.Tan(hFovInRads / 2) / aspectRatio);

        return vFovInRads * Mathf.Rad2Deg;
    }
}
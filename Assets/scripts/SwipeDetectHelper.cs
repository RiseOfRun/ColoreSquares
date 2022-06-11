using UnityEngine;

public class SwipeDetectHelper : MonoBehaviour
{
    public float triggerDistance = 0.5f;
    public static SwipeDetectHelper Instance;
    
    private bool inSwipe = false;
    private Vector2 startPosition;
    private Vector2 currentPosition;
    private Camera mainCamera;
    public delegate void SwipeDone(Vector2Int diraction);
    public event SwipeDone OnSwipe;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        if (Instance == this)
        {
            Destroy(gameObject);
        }
        
        DontDestroyOnLoad(Instance);
        
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inSwipe = true;
            startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            if (inSwipe)
            {
                currentPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                CheckSwipe();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            inSwipe = false;
        }
    }

    private void CheckSwipe()
    {
        Vector2 diraction = currentPosition - startPosition;
        Vector2Int toSwipe = new Vector2Int();
        if (Mathf.Abs(diraction.x)>=triggerDistance)
        {
            inSwipe = false;
            toSwipe.x = diraction.x > 0 ? 1 : -1;
        }
        else if (Mathf.Abs(diraction.y)>=triggerDistance)
        {
            inSwipe = false;
            toSwipe.y = diraction.y > 0 ? 1 : -1;
        }
        
        if (!inSwipe)
        {
            OnSwipe?.Invoke(toSwipe);
        }
    }
}

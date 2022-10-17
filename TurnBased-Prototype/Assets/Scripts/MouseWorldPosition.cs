using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    private static MouseWorldPosition instance;

    private static Vector3 lastPosition = Vector3.zero;

    [SerializeField] private LayerMask mousePlaneLayerMask;

    void Awake()
    {
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.Instance.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        if (raycastHit.collider != null)
        {
            lastPosition = raycastHit.point;
        }
        return lastPosition;
    }
}
using UnityEngine;

public class MouseWorldPosition : MonoBehaviour
{
    private static MouseWorldPosition instance;
    [SerializeField] private LayerMask mousePlaneLayerMask;

    void Awake()
    {
        instance = this;
    }

    public static Vector3 GetPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, instance.mousePlaneLayerMask);
        return raycastHit.point;
    }
}
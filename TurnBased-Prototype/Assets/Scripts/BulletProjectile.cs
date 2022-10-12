using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitVFXPrefab;
    private Vector3 targetPosition;

    void Update()
    {
        Vector3 moveDir = (targetPosition - transform.position).normalized;

        float distanceFromTarget = Vector3.Distance(transform.position, targetPosition);

        float moveSpeed = 200f;

        if (distanceFromTarget <= moveSpeed)
        {
            transform.position = targetPosition;

            trailRenderer.transform.parent = null;

            Destroy(gameObject);
            
            Instantiate(bulletHitVFXPrefab, targetPosition, Quaternion.identity);
        }
        else
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
    }

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
using System;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    [SerializeField] private Transform crateDestroyedPrefab;

    private GridPosition gridPosition;

    void Start()
    {
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public GridPosition GetGridPosition()
    {
        return gridPosition;
    }

    public void Damage()
    {
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, +1f), 0, UnityEngine.Random.Range(-1f, +1f));
        Transform crateDestoryedTransfrom = Instantiate(crateDestroyedPrefab, transform.position, transform.rotation);
        ApplyExplosionToChildren(crateDestoryedTransfrom, 200f, transform.position + randomDir, 15f);

        Destroy(gameObject);
        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
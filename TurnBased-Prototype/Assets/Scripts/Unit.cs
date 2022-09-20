using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    private Vector3 targetPosition;
    
    void Update() 
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveSpeed * Time.deltaTime * moveDirection;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Move(MouseWorldPosition.GetPosition());
        }
    }

    private void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
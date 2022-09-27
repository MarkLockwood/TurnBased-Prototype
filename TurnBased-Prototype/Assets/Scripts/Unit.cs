using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;

    [SerializeField] float rotateSpeed = 10f;

    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;

    static int _isWalkingHash;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Start() 
    {
        _isWalkingHash = Animator.StringToHash("IsWalking");
    }
    
    void Update() 
    {
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance)
        {
            Vector3 moveDirection = (targetPosition - transform.position).normalized;
            transform.position += moveSpeed * Time.deltaTime * moveDirection;

            // Vector-Based Rotation
            //transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);

            // Quaternion-Based Rotation
            Quaternion currentRotation = transform.localRotation;
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.localRotation = Quaternion.Slerp(currentRotation, newRotation, Time.deltaTime * rotateSpeed);

            unitAnimator.SetBool(_isWalkingHash, true);
        }
        else
        {
            unitAnimator.SetBool(_isWalkingHash, false);
        }
    }

    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
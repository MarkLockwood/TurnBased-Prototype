using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;

    [SerializeField] float rotateSpeed = 10f;

    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;

    private GridPosition gridPosition;

    static int _isWalkingHash;

    void Awake()
    {
        targetPosition = transform.position;
    }

    void Start() 
    {
        _isWalkingHash = Animator.StringToHash("IsWalking");
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
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

        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != gridPosition)
        {
            // Unit Position Has Changed:
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public void Move(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
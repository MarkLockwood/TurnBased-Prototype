using System;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{
    [SerializeField] float moveSpeed = 4f;
    [SerializeField] float stoppingDistance = .1f;
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] int maxMoveDistance = 4;
    [SerializeField] private Animator unitAnimator;
    private Vector3 targetPosition;

    static int _isWalkingHash;

    protected override void Awake()
    {
        base.Awake();
        targetPosition = transform.position;
    }

    void Start()
    {
        _isWalkingHash = Animator.StringToHash("IsWalking");
    }

    void Update()
    {
        if (!isActive)
        {
            return;
        }

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
            isActive = false;
            onActionComplete();
        }
    }
    
    public void Move(GridPosition gridPosition, Action onActionComplete)
    {
        this.onActionComplete = onActionComplete;
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = unit.GetGridPosition();
        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    // This Unit Is Already On That Grid Space.
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Another Unit Is Already On That Grid Space.
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        return validGridPositionList;
    }
}
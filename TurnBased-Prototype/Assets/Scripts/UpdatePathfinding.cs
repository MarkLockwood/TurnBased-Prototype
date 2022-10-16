using UnityEngine;

public class UpdatePathfinding : MonoBehaviour
{
    private void OnDestroy() 
    {
        // Get the grid position for this transform
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        // Set the grid position to be walkable
        Pathfinding.Instance.SetIsWalkableGridPosition(gridPosition, true);
    }
}
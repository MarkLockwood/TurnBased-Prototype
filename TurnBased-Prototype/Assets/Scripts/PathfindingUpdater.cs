using System;
using UnityEngine;

public class PathfindingUpdater : MonoBehaviour
{
    void Start()
    {
        DestructibleCrate.OnAnyDestroyed += DestructibleCrate_OnAnyDestoryed;
    }

    private void DestructibleCrate_OnAnyDestoryed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GetGridPosition(), true);
    }
}
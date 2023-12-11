using System.Collections.Generic;
using DataStructures;
using UnityEngine;

/// <summary>
/// Similar to A*, but doesn't find the shortest path, it just finds all possible positions
/// where travel cost is less than or equal to the max travel cost.
/// </summary>
public class TargetPossiblePositions : PossiblePositions
{
    public TargetPossiblePositions(bool[,] aStarGrid, int maxTravelCost, Vector2Int startPos)
        : base(aStarGrid, maxTravelCost, startPos) { }

    public TargetPossiblePositions(
        bool[] aStarGrid,
        int width,
        int height,
        int maxTravelCost,
        Vector2Int startPos
    )
        : base(aStarGrid, width, height, maxTravelCost, startPos) { }

    /// <summary>
    /// Returns a list of adjacent nodes that do not contain obstacles.
    /// </summary>
    public override List<Node> GetAdjacentNodes(Node node)
    {
        List<Vector2Int> adjacentNodeDirections;
        var directionFromStartToNode = node.position - startPos;

        // Normalize the direction vector so that it is either up, down, left, or right
        if (directionFromStartToNode.x != 0)
        {
            directionFromStartToNode.x /= Mathf.Abs(directionFromStartToNode.x);
        }
        else if (directionFromStartToNode.y != 0)
        {
            directionFromStartToNode.y /= Mathf.Abs(directionFromStartToNode.y);
        }

        // If the node is the start node, then the all directions are possible
        if (directionFromStartToNode.magnitude.Equals(0))
        {
            adjacentNodeDirections = new List<Vector2Int>
            {
                Vector2Int.up,
                Vector2Int.right,
                Vector2Int.down,
                Vector2Int.left
            };
        }
        else
        {
            adjacentNodeDirections = new List<Vector2Int> { directionFromStartToNode };
        }

        Debug.Log($"adjacentNodeDirections: {string.Join(", ", adjacentNodeDirections)}");

        var adjacentNodes = new List<Node>();

        foreach (var posDirection in adjacentNodeDirections)
        {
            var adjacentNodePos = node.position + posDirection;

            // Check if the node is in the grid
            if (
                adjacentNodePos.x < 0
                || adjacentNodePos.x >= aStarGrid.GetLength(0)
                || adjacentNodePos.y < 0
                || adjacentNodePos.y >= aStarGrid.GetLength(1)
            )
            {
                continue;
            }

            // Check if the node contains an obstacle
            if (aStarGrid[adjacentNodePos.x, adjacentNodePos.y])
            {
                continue;
            }

            adjacentNodes.Add(new(node, adjacentNodePos));
        }

        return adjacentNodes;
    }
}

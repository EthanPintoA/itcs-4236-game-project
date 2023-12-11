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

        // If the node is the start node, then the all directions are possible
        if (directionFromStartToNode == Vector2Int.zero)
        {
            adjacentNodeDirections = new List<Vector2Int>
            {
                new(-1, -1), // Top Left
                new(0, -1), // Top
                new(1, -1), // Top Right
                new(-1, 0), // Left
                new(1, 0), // Right
                new(-1, 1), // Down Left
                new(0, 1), // Down
                new(1, 1), // Down Right
            };
        }
        // If node is left or right of the start node
        else if (
            Mathf.Abs(directionFromStartToNode.x) > 0 && Mathf.Abs(directionFromStartToNode.y) == 0
        )
        {
            adjacentNodeDirections = new List<Vector2Int>
            {
                new(-1, 0), // Left
                new(1, 0), // Right
            };
        }
        // If node is above or below the start node
        else if (
            Mathf.Abs(directionFromStartToNode.x) == 0 && Mathf.Abs(directionFromStartToNode.y) > 0
        )
        {
            adjacentNodeDirections = new List<Vector2Int>
            {
                new(0, -1), // Top
                new(0, 1), // Down
            };
        }
        // If node is diagonal, then ignore it
        else
        {
            return new List<Node>();
        }

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

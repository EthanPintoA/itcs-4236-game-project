using System.Collections.Generic;
using System.Linq;
using DataStructures;
using UnityEngine;

/// <summary>
/// Similar to A*, but doesn't find the shortest path, it just finds all possible positions
/// where travel cost is less than or equal to the max travel cost.
/// </summary>
public class PossiblePositions
{
    /// <summary>
    /// A grid of obstacles. True means there is an obstacle.
    /// </summary>
    private readonly bool[,] aStarGrid;
    private readonly int maxTravelCost;

    public PossiblePositions(bool[,] aStarGrid, int maxTravelCost)
    {
        this.aStarGrid = aStarGrid;
        this.maxTravelCost = maxTravelCost;
    }

    public PossiblePositions(bool[] aStarGrid, int width, int height, int maxTravelCost)
    {
        this.aStarGrid = new bool[width, height];

        for (int i = 0; i < aStarGrid.Length; i++)
        {
            this.aStarGrid[i % width, i / width] = aStarGrid[i];
        }

        this.maxTravelCost = maxTravelCost;
    }

    /// <summary>
    /// Returns a list of positions where travel cost is less than or equal to the max travel cost.
    /// </summary>
    public Vector2Int[] GetPositions(Vector2Int startPos)
    {
        var startNode = new Node(null, startPos);

        var openQueue = new PriorityQueue<Node>();
        var closedSet = new HashSet<Node>();

        openQueue.Enqueue(startNode);

        while (!openQueue.IsEmpty)
        {
            var currentNode = openQueue.Dequeue();

            closedSet.Add(currentNode);

            var adjacentNodes = GetAdjacentNodes(currentNode, aStarGrid);

            foreach (var neighbor in adjacentNodes)
            {
                // Ignore nodes that have already been evaluated
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the travel cost
                neighbor.g = currentNode.g + 1f;
                // There is no heuristic, so h is 0

                // Add the node if it is not already in the open set
                // and the travel cost is less than or equal to the max travel cost
                if (!openQueue.Contains(neighbor) && neighbor.g <= maxTravelCost)
                {
                    openQueue.Enqueue(neighbor);
                }
            }
        }

        return closedSet.Select(node => node.position).ToArray();
    }

    /// <summary>
    /// Returns a list of adjacent nodes that do not contain obstacles.
    /// </summary>
    public static List<Node> GetAdjacentNodes(Node node, bool[,] aStarGrid)
    {
        var adjacentNodeDirections = new Vector2Int[]
        {
            new(-1, -1), // Top left
            new(0, -1), // Top
            new(1, -1), // Top right
            new(-1, 0), // Left
            new(1, 0), // Right
            new(-1, 1), // Bottom left
            new(0, 1), // Bottom
            new(1, 1), // Bottom right
        };

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

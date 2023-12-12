using System.Collections.Generic;
using UnityEngine;
using DataStructures;

public class AStar
{
    /// <summary>
    /// A grid of obstacles. True means there is an obstacle.
    /// </summary>
    private readonly bool[,] aStarGrid;
    private readonly Vector2Int startPos;
    private readonly Vector2Int endPos;

    public AStar(bool[,] aStarGrid, Vector2Int startPos, Vector2Int endPos)
    {
        this.aStarGrid = aStarGrid;
        this.startPos = startPos;
        this.endPos = endPos;
    }

    public AStar(bool[] aStarGrid, int width, int height, Vector2Int startPos, Vector2Int endPos)
    {
        this.aStarGrid = new bool[width, height];

        for (int i = 0; i < aStarGrid.Length; i++)
        {
            this.aStarGrid[i % width, i / width] = aStarGrid[i];
        }

        this.startPos = startPos;
        this.endPos = endPos;
    }

#nullable enable
    /// <summary>
    /// Returns a path from startPos to endPos in terms of grid positions.
    /// Returns null if no path is found.
    /// </summary>
    public Vector2Int[]? GetPath()
    {
        var startNode = new Node(null, startPos);
        var endNode = new Node(null, endPos);

        var openQueue = new PriorityQueue<Node>();
        var closedSet = new HashSet<Node>();

        openQueue.Enqueue(startNode);

        while (!openQueue.IsEmpty)
        {
            var currentNode = openQueue.Dequeue();

            closedSet.Add(currentNode);

            if (currentNode.Equals(endNode))
            {
                return GetPath(currentNode).ToArray();
            }

            var adjacentNodes = GetAdjacentNodes(currentNode, aStarGrid);

            foreach (var neighbor in adjacentNodes)
            {
                // Ignore nodes that have already been evaluated
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the g, and h values
                neighbor.g = currentNode.g + 1f;
                neighbor.h = Vector2Int.Distance(neighbor.position, endNode.position);

                // Don't add the node if it is already in the open set
                if (!openQueue.Contains(neighbor))
                {
                    openQueue.Enqueue(neighbor);
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Returns a path from the start node to the end node by following the parent nodes
    /// until null is reached.
    /// </summary>
    private static List<Vector2Int> GetPath(Node endNode)
    {
        var path = new List<Vector2Int>();
        var current = endNode;

        // Construct the path from the end node to the start node
        while (current != null)
        {
            path.Add(current.position);
            current = current.parent;
        }

        // Reverse the path so that it goes from the start node to the end node
        path.Reverse();
        return path;
    }

    /// <summary>
    /// Returns a list of adjacent nodes that do not contain obstacles.
    /// </summary>
    private static List<Node> GetAdjacentNodes(Node node, bool[,] aStarGrid)
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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * A generic implementation of the BFS algorithm.
 * @author Erel Segal-Halevi
 * @since 2020-02
 */
public class BFS {

    public static List<Vector3Int> GetReachableTiles(TilemapGraph tilemapGraph, Vector3Int startPosition, int maxIterations = 1000)
    {
        List<Vector3Int> reachableTiles = new List<Vector3Int>();

        Queue<Vector3Int> openQueue = new Queue<Vector3Int>();
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        openQueue.Enqueue(startPosition);
        visited.Add(startPosition);

        int iterations = 0;

        while (openQueue.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            Vector3Int currentTile = openQueue.Dequeue();
            reachableTiles.Add(currentTile);

            foreach (var neighbor in tilemapGraph.Neighbors(currentTile))
            {
                if (!visited.Contains(neighbor))
                {
                    openQueue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return reachableTiles;
    }

    // This is the existing GetPath method in the BFS class
    public static List<NodeType> GetPath<NodeType>(IGraph<NodeType> graph, NodeType startNode, NodeType endNode, int maxiterations = 1000)
    {
        List<NodeType> path = new List<NodeType>();
        FindPath(graph, startNode, endNode, path, maxiterations);
        return path;
    }



    public static void FindPath<NodeType>(
            IGraph<NodeType> graph, 
            NodeType startNode, NodeType endNode, 
            List<NodeType> outputPath, int maxiterations=1000)
    {
        Queue<NodeType> openQueue = new Queue<NodeType>();
        HashSet<NodeType> openSet = new HashSet<NodeType>();
        Dictionary<NodeType, NodeType> previous = new Dictionary<NodeType, NodeType>();
        openQueue.Enqueue(startNode);
        openSet.Add(startNode);
        int i; for (i = 0; i < maxiterations; ++i) { // After maxiterations, stop and return an empty path
            if (openQueue.Count == 0) {
                break;
            } else {
                NodeType searchFocus = openQueue.Dequeue();

                if (searchFocus.Equals(endNode)) {
                    // We found the target -- now construct the path:
                    outputPath.Add(endNode);
                    while (previous.ContainsKey(searchFocus)) {
                        searchFocus = previous[searchFocus];
                        outputPath.Add(searchFocus);
                    }
                    outputPath.Reverse();
                    break;
                } else {
                    // We did not found the target yet -- develop new nodes.
                    foreach (var neighbor in graph.Neighbors(searchFocus)) {
                        if (openSet.Contains(neighbor)) {
                            continue;
                        }
                        openQueue.Enqueue(neighbor);
                        openSet.Add(neighbor);
                        previous[neighbor] = searchFocus;
                    }
                }
            }
        }
    }

    public static List<NodeType> GetPath<NodeType>(TilemapGraph tilemapGraph, IGraph<NodeType> graph, NodeType startNode, NodeType endNode, int maxiterations=1000) {
        List<NodeType> path = new List<NodeType>();
        FindPath(graph, startNode, endNode, path, maxiterations);
        return path;
    }

}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Dijkstra : Search
{
     private PriorityQueue<Node, float> _priorityQueue = new PriorityQueue<Node, float>();

     // 상하좌우 및 대각선 방향 탐색
     int[] deltaY = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
     int[] deltaX = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
     int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14 };
     
    public override void StartSearch()
    {
        base.StartSearch();
        Debug.Log("Dijkstra StartSearch");
        StartCoroutine(SearchCoroutine());
    }

    protected override IEnumerator SearchCoroutine()
    {
        PathFinder.StartNode.Dist = 0;
        _priorityQueue.Enqueue(PathFinder.StartNode, 0);
        
        while (_priorityQueue.Count > 0)
        {
            Node currentNode = _priorityQueue.Dequeue();
            if (currentNode.Visited) continue;

            currentNode.Visited = true;
            currentNode.PathType = Node.Type.SearchComplete;
            Debug.Log($"Visited Node: ({currentNode.X}, {currentNode.Y})");

            yield return new WaitForSeconds(0.05f);

            if (currentNode == PathFinder.DestNode || IsSearchFinished)
            {
                IsSearchFinished = true;
                _priorityQueue.Clear();
                yield break;
            }

            UpdateNeighbors(currentNode);
        }
    }

    protected override void UpdateNeighbors(Node currentNode)
    {
        for (int i = 0; i < deltaY.Length; i++)
        {
            int nextY = currentNode.X + deltaY[i];
            int nextX = currentNode.Y + deltaX[i];

            EnqueueNeighbor(currentNode, nextY, nextX, cost[i]);
        }
    }

    protected override void EnqueueNeighbor(Node currentNode, int x, int y, int moveCost)
    {
        if (x < 0 || x >= PathFinder.Height || y < 0 || y >= PathFinder.Width)
            return;

        Node neighbor = PathFinder.Nodes[x, y];
        if (neighbor.Visited || neighbor.PathType == Node.Type.Wall) return;

        float newCost = currentNode.Dist + moveCost;
        
        if (newCost < neighbor.Dist)
        {
            neighbor.Dist = newCost;
            _priorityQueue.Enqueue(neighbor, newCost);
            neighbor.GetComponentInChildren<TextMeshPro>().text = $"Dist : {neighbor.Dist}";
        }
    }
}

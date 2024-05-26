using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFS : Search
{
    private Queue<Node> _queue = new Queue<Node>();
    
    public override void StartSearch()
    {
        base.StartSearch();
        Debug.Log("BFS StartSearch");
        StartCoroutine(SearchCoroutine());
    }
    
    protected override IEnumerator SearchCoroutine()
    {
        _queue.Enqueue(PathFinder.StartNode);
        PathFinder.StartNode.Dist = 0;
        
        while (_queue.Count > 0)
        {
            Node currentNode = _queue.Dequeue();
            
            if (currentNode == PathFinder.DestNode || IsSearchFinished)
            {
                IsSearchFinished = true;
                _queue.Clear();
                yield break;
            }
            
            if (currentNode.Visited || currentNode.PathType == Node.Type.Wall) continue;
            
            currentNode.Visited = true;
            currentNode.PathType = Node.Type.SearchComplete;
            
            Debug.Log($"Visited Node: ({currentNode.X}, {currentNode.Y})");
            
            yield return new WaitForSeconds(0.05f);
            
            UpdateNeighbors(currentNode);
        }
    }

    protected override void UpdateNeighbors(Node currentNode)
    {
        EnqueueNeighbor(currentNode, currentNode.X - 1, currentNode.Y); // 상
        EnqueueNeighbor(currentNode, currentNode.X, currentNode.Y - 1); // 좌
        EnqueueNeighbor(currentNode, currentNode.X + 1, currentNode.Y); // 하
        EnqueueNeighbor(currentNode, currentNode.X, currentNode.Y + 1); // 우
    }

    protected override void EnqueueNeighbor(Node currentNode, int height, int width, int moveCost = 1)
    {
        Node neighbor = PathFinder.Nodes[height, width];
        if (neighbor.Visited || neighbor.PathType == Node.Type.Wall) return;
        
        _queue.Enqueue(neighbor);
        neighbor.Dist = currentNode.Dist + moveCost;
    }
}

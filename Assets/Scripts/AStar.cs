using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AStar : Search
{
    private PriorityQueue<Node, float> _priorityQueue = new PriorityQueue<Node, float>();
    
    // 상하좌우 및 대각선 방향 탐색
    int[] deltaHeight = new int[] { -1, 0, 1, 0, -1, 1, 1, -1 };
    int[] deltaWidth = new int[] { 0, -1, 0, 1, -1, -1, 1, 1 };
    int[] cost = new int[] { 10, 10, 10, 10, 14, 14, 14, 14};
    
    public override void StartSearch()
    {
        base.StartSearch();
        Debug.Log("A* StartSearch");
        StartCoroutine(SearchCoroutine());
    }

    protected override IEnumerator SearchCoroutine()
    {
        // 시작 노드 탐색
        for (int i = 0; i < PathFinder.Height; i++)
        {
            for (int j = 0; j < PathFinder.Width; j++)
            {
                Node node = PathFinder.Nodes[i, j];
                node.HCost = CalculateHeuristic(PathFinder.Nodes[i, j]);
                if (node.NodeType == Node.StartDestState.Start)
                {
                    node.GCost = 0;
                    _priorityQueue.Enqueue(node, node.FCost);
                    node.GetComponentInChildren<TextMeshPro>().text =
                        $"GCost : {node.GCost}\nHCost : {node.HCost}\nFCost : {node.FCost}";
                }
            }
        }

        while (_priorityQueue.Count > 0)
        {
            Node currentNode = _priorityQueue.Dequeue();
            if (currentNode.Visited) continue;

            currentNode.Visited = true;
            
            yield return new WaitForSeconds(0.05f);

            if (currentNode == PathFinder.DestNode || IsSearchFinished)
            {
                IsSearchFinished = true;
                _priorityQueue.Clear();
                yield break;
            }
            
            currentNode.PathType = Node.Type.SearchComplete;
            Debug.Log($"Visited Node: ({currentNode.X}, {currentNode.Y})");

            UpdateNeighbors(currentNode);
        }
    }

    protected override void UpdateNeighbors(Node node)
    {
        for (int i = 0; i < deltaHeight.Length; i++)
        {
            int nextX = node.X + deltaWidth[i];
            int nextY = node.Y + deltaHeight[i];

            EnqueueNeighbor(node, nextX, nextY, cost[i]);
        }
    }

    protected override void EnqueueNeighbor(Node currentNode, int x, int y, int moveCost)
    {
        if (y < 0 || y >= PathFinder.Height || x < 0 || x >= PathFinder.Width)
            return;

        Node neighbor = PathFinder.Nodes[x, y];
        if (neighbor.Visited || neighbor.PathType == Node.Type.Wall)
            return;

        float gCost = currentNode.GCost + moveCost; // 노드 간 가중치를 1로 설정 (필요시 변경)
        float hCost = CalculateHeuristic(neighbor);
        
        if (neighbor.FCost < gCost + hCost) return;
        
        neighbor.GCost = gCost;
        neighbor.HCost = hCost;
        _priorityQueue.Enqueue(neighbor, neighbor.FCost);
        neighbor.GetComponentInChildren<TextMeshPro>().text =
            $"GCost : {neighbor.GCost}\nHCost : {neighbor.HCost}\nFCost : {neighbor.FCost}";
    
    }

    private float CalculateHeuristic(Node node)
    {
        // 맨해튼 거리 계산
        // return  0.8f * Mathf.Sqrt(Mathf.Pow(Mathf.Abs(x - destination.x),2) + Mathf.Pow(Mathf.Abs(y - destination.y), 2));

        var dx = Mathf.Abs(node.X - PathFinder.DestNode.X);
        var dy = Mathf.Abs(node.Y - PathFinder.DestNode.Y);

        float diagonal = 10 * (dx + dy) + -6 * Mathf.Min(dx, dy);
        // 2D 대각선 거리 공식 (D⋅(dx+dy)+(D2−2⋅D)⋅min(dx,dy))
        float additionalCost = CalculateAdditionalCost(node);
        
        Debug.Log($"Node[{node.X},{node.Y}]의 additional Cost = {additionalCost}");

        return 1.2f * diagonal + additionalCost;
        // 체비쇼프 거리 공식 mathf.Max(dx, dy)
        //return 10 * Mathf.Max(dx, dy);
    }
    
    private int CalculateAdditionalCost(Node node) {
        
        int additionalCost = 0;
        
        var dh = Mathf.Abs(node.Y - PathFinder.DestNode.Y);
        var dw = Mathf.Abs(node.X - PathFinder.DestNode.X);
        
        for (int i = 0; i < deltaHeight.Length; i++)
        {
            int newHeight = node.Y + deltaHeight[i];
            int newWidth = node.X + deltaWidth[i];

            // 새 노드와 목적지 사이의 거리
            int nextDh = Mathf.Abs(newHeight - PathFinder.DestNode.Y);
            int nextDw = Mathf.Abs(newWidth - PathFinder.DestNode.X);

            if(dh + dw < nextDh + nextDw) continue;
            
            // 맵 경계 확인 및 벽 확인
            if (0 < nextDh && nextDh < PathFinder.Height && 0 < nextDw && nextDw < PathFinder.Width) {
                if (PathFinder.Nodes[nextDh, nextDw].PathType == Node.Type.Wall)
                   additionalCost += 10; // 가중치 값은 상황에 따라 조절 가능
            }
        }
        return additionalCost;
    }
}

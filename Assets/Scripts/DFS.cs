using System;
using System.Collections;
using UnityEngine;

public class DFS : Search
{
    public override void StartSearch()
    {
        base.StartSearch();
        Debug.Log("DFS StartSearch");
        
        StartCoroutine(SearchCoroutine(PathFinder.StartNode.X, PathFinder.StartNode.Y));
    }

    protected override IEnumerator SearchCoroutine(int height, int width)
    {
        if (PathFinder.Nodes[height, width] == PathFinder.DestNode || IsSearchFinished)
        {
            IsSearchFinished = true;
            yield break;
        }
        
        if (PathFinder.Nodes[height, width].Visited ||
            PathFinder.Nodes[height, width].PathType == Node.Type.Wall)
        {
            yield break;
        }

        PathFinder.Nodes[height, width].Visited = true;
        PathFinder.Nodes[height, width].PathType = Node.Type.SearchComplete;
        Debug.Log($"Visited Node: ({height}, {width})");

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(SearchCoroutine(height - 1, width)); // 상
        yield return StartCoroutine(SearchCoroutine(height, width - 1)); // 좌
        yield return StartCoroutine(SearchCoroutine(height + 1, width)); // 하
        yield return StartCoroutine(SearchCoroutine(height, width + 1)); // 우
    }
}

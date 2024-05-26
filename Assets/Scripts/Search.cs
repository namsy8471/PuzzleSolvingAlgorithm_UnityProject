using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Search : MonoBehaviour
{
    protected PathFinder PathFinder;
    protected bool IsSearchFinished;
    
    public void Init()
    {
        PathFinder = GameObject.Find("PathFinder").GetComponent<PathFinder>();
        IsSearchFinished = false;
    }

    public virtual void StartSearch()
    {
        ResetNodes(false);
    }

    protected virtual IEnumerator SearchCoroutine()
    {
        yield break;
    }
    
    protected virtual IEnumerator SearchCoroutine(int height, int width)
    {
        yield break;
    }

    protected virtual void UpdateNeighbors(Node currentNode) { }
    protected virtual void EnqueueNeighbor(Node currentNode, int x, int y, int moveCost) {}
    
    public void ResetNodes(bool isResetForStop)
    {
        IsSearchFinished = isResetForStop;
        
        for (int i = 0; i < PathFinder.Height; i++)
        {
            for (int j = 0; j < PathFinder.Width; j++)
            {
                Node node = PathFinder.Nodes[i, j];
                node.ResetNode();
                
                if (node.PathType == Node.Type.SearchComplete) node.PathType = Node.Type.Path;
                if(node.NodeType != Node.StartDestState.None) node.NodeType = node.NodeType;
            }
        }
    }
}

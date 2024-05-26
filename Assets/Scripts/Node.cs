using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{

    public enum Type
    {
        Wall,
        Path,
        SearchComplete,
        Way
    }

    public enum StartDestState
    {
        None,
        Start,
        Destination
    }
    
    private Material _material;
    private Type _pathType;
    private StartDestState _nodeType;

    public int X;
    public int Y;
    
    public float Dist { get; set; }
    public bool Visited { get; set; }

    public float GCost = float.MaxValue;
    public float HCost = 0;
    public float FCost => GCost + HCost;
    
    public Type PathType
    {
        get => _pathType;
        set
        {
            _pathType = value;
            CheckPathType();
            if (NodeType == StartDestState.None) return;
            CheckNodeType();
        }
    }

    public StartDestState NodeType
    {
        get => _nodeType;
        set
        {
            _nodeType = value;
            CheckNodeType();
        }
    }
    
    public void Init()
    {
        Dist = float.MaxValue;
        GCost = float.MaxValue;
        HCost = 0;

        _pathType = Type.Path;
        Visited = false;
        
        _material = GetComponent<MeshRenderer>().materials[0];
    }

    private void CheckPathType()
    {
        switch (PathType)
        {
            case Type.Wall:
                ChangeColor(Color.red);
                break;
            case Type.Path:
                ChangeColor(Color.white);
                break;
            case Type.SearchComplete:
                ChangeColor(Color.green);
                break;
            case Type.Way:
                ChangeColor(Color.yellow);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void CheckNodeType()
    {
        switch (NodeType)
        {
            case StartDestState.None:
                ChangeColor(Color.white);
                break;
            case StartDestState.Start:
                ChangeColor(Color.blue);
                break;
            case StartDestState.Destination:
                ChangeColor(Color.yellow + Color.red);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChangeColor(Color color)
    {
        _material.color = color;
    }

    public void ResetNode()
    {
        Dist = float.MaxValue;
        GCost = float.MaxValue;
        HCost = 0;
        
        GetComponentInChildren<TextMeshPro>().text = "";
        Visited = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        ChangeColor(Color.magenta);

    }

    public void OnTriggerExit(Collider other)
    {
        CheckPathType();
        if (NodeType == StartDestState.None) return;
        CheckNodeType();
    }
} 

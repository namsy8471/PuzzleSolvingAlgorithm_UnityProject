using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathFinder : MonoBehaviour
{
    
    public enum PathFinding
    {
        SearchWay,
        DFS,
        BFS,
        Dijkstra,
        AStar
    }
    public PathFinding type;
    
    public GameObject planePrefab;
    public readonly Node[,] Nodes = new Node[21, 21];
    public Node StartNode { get; set; }
    public Node DestNode { get; set; }

    private TMP_InputField _widthInputField;
    private TMP_InputField _heightInputField;
    private Button _startButton;
    private Button _resetButton;
    private TMP_Dropdown _dropdown;
    
    private Camera _mainCam;

    private Search _search;

    private Collider _lastHitCollider;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    private void Start()
    {
        InitSearch();
    }
    private void Update()
    {
        Ray ray = _mainCam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Node")))
        {
            var collider = hit.collider;
            collider.GetComponent<Node>().OnTriggerEnter(collider);
            
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider.GetComponent<Node>().PathType == Node.Type.Wall) return;
                
                if (DestNode == null)
                {
                    if (StartNode == null)
                    {
                        StartNode = hit.collider.GetComponent<Node>();
                        StartNode.NodeType = Node.StartDestState.Start;
                    }
                
                    else
                    {
                        DestNode = hit.collider.GetComponent<Node>();
                        DestNode.NodeType = Node.StartDestState.Destination;
                    }
                }
                
                else
                {
                    EraseStartAndDestNode();
                    
                    StartNode = hit.collider.GetComponent<Node>();
                    StartNode.NodeType = Node.StartDestState.Start;
                }
            }
            
            else if (Input.GetMouseButtonDown(1))
            {
                hit.collider.GetComponent<Node>().PathType = 
                    hit.collider.GetComponent<Node>().PathType == Node.Type.Wall ?
                        Node.Type.Path : Node.Type.Wall;
            }

            if (_lastHitCollider == collider) return;
            if(_lastHitCollider != null)
                _lastHitCollider.GetComponent<Node>().OnTriggerExit(_lastHitCollider);
                
            _lastHitCollider = collider;
        }
        else
        {
            if(_lastHitCollider != null)
                _lastHitCollider.GetComponent<Node>().OnTriggerExit(_lastHitCollider);
        }
    }
    private void InitSearch()
    {
        _mainCam = Camera.main;

        Width = 20;
        Height = 20;

        _search = gameObject.AddComponent<Search>();
        _search.Init();
        
        GridInit();
        DropDownInit();
        InputFieldsInit();
        ButtonsInit();
    }
    private void GridInit()
    {
        // 그리드 생성
        for (var i = 0; i < Height; i++)
        {
            for (var j = 0; j < Width; j++)
            {
                // 인스턴스화할 프리팹 생성
                var scaleFactor = planePrefab.transform.localScale;
                
                GameObject plane = Instantiate(planePrefab, 
                    new Vector3(i * scaleFactor.x * 10, 0, j * scaleFactor.z * 10), Quaternion.identity);
                
                plane.transform.parent = transform;
                plane.AddComponent<Node>();
                plane.name = $"Plane({i},{j})";
                
                Nodes[i, j] = plane.GetComponent<Node>();
                Nodes[i, j].Init();
                Nodes[i, j].X = i;
                Nodes[i, j].Y = j;

                if (i == 0 || i == Height - 1 || j == 0 || j == Width - 1)
                {
                    Nodes[i, j].PathType = Node.Type.Wall;
                }
            }
        }

        SetCameraCenter();
    }
    private void DropDownInit()
    {
        _dropdown = GameObject.Find("SearchDropdown").GetComponent<TMP_Dropdown>();
        _dropdown.options.Clear();
        
        List<string> strs = new List<string>();
        foreach (var value in Enum.GetNames(typeof(PathFinding)))
        {
            strs.Add(value);
        }

        _dropdown.AddOptions(strs);
        _dropdown.onValueChanged.AddListener(ChangeSearch);
    }
    private void InputFieldsInit()
    {
        _widthInputField = GameObject.Find("WidthInputField").GetComponent<TMP_InputField>();
        _heightInputField = GameObject.Find("HeightInputField").GetComponent<TMP_InputField>();
        
        _widthInputField.onValueChanged.AddListener(ChangeWidth);
        _heightInputField.onValueChanged.AddListener(ChangeHeight);
    }
    private void ButtonsInit()
    {
        _startButton = GameObject.Find("StartButton").GetComponent<Button>();
        _resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
        
        _startButton.onClick.AddListener(StartSearch);
        _resetButton.onClick.AddListener(ResetNodes);
    }
    private void StartSearch()
    {
        _search.StartSearch();
    }
    private void ResetNodes()
    {
        _search.ResetNodes(true);
    }
    private void EraseStartAndDestNode()
    {
        StartNode.NodeType = Node.StartDestState.None;
        StartNode = null;
        DestNode.NodeType = Node.StartDestState.None;
        DestNode = null;
    }
    private void SetCameraCenter()
    {
        Node midNode = Nodes[Height / 2, Width / 2];

        var pos = midNode.transform.position;
        _mainCam.transform.position = new Vector3(pos.x, Mathf.Clamp(Height * Width, 80, 200), pos.z);
        _mainCam.transform.LookAt(midNode.transform);
    }

    private void ChangeSearch(int index)
    {
        switch (_dropdown.GetComponent<TMP_Dropdown>().captionText.text)
        {
            case "DFS":
                if(GetComponent<DFS>() == null)
                    _search = gameObject.AddComponent<DFS>();
                _search = GetComponent<DFS>();
                break;
            case "BFS":
                if(GetComponent<BFS>() == null)
                    _search = gameObject.AddComponent<BFS>();
                _search = GetComponent<BFS>();
                break;
            case "Dijkstra":
                if(GetComponent<Dijkstra>() == null)
                    _search = gameObject.AddComponent<Dijkstra>();
                _search = GetComponent<Dijkstra>();
                break;
            case "AStar":
                if(GetComponent<AStar>() == null)
                    _search = gameObject.AddComponent<AStar>();
                _search = GetComponent<AStar>();
                break;
            default:
                break;
        }
        
        _search.Init();
    }
    private void ChangeWidth(string newValue)
    {
        Width = int.Parse(newValue);

        if (Width > 20) Width = 20;
        
        UpdateNodes();
    }
    private void ChangeHeight(string newValue)
    {
        Height = int.Parse(newValue);
        
        if (Height > 20) Height = 20;
        
        UpdateNodes();
    }
    private void UpdateNodes()
    {
        for (var width = 0; width < 20; width++)
        {
            for (var height = 0; height < 20; height++)
            {
                if (width < Width && height < Height)
                {
                    Nodes[height, width].gameObject.SetActive(true);

                    Nodes[height, width].PathType = width == 0 || height == 0 || width == Width - 1 || height == Height - 1
                        ? Node.Type.Wall
                        : Node.Type.Path;
                }

                else
                {
                    Nodes[height, width].gameObject.SetActive(false);
                }
            }
        }

        SetCameraCenter();
    }
}

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] Transform start;
    [SerializeField] Transform destiny;
    [SerializeField] float distanceNode = 1.25f;

    class MapGrid
    {
        public Queue<Node> Nodes { get { return nodes; } }

        Queue<Node> nodes = new();

        public void AddNode(Node node)
        {
            nodes.Enqueue(node);
        }

        public Node GetNodeByPosition(Vector2 position)
        {
            foreach (var node in nodes)
                if (node.Position.Equals(position))
                    return node;

            return null;
        }

        public Queue<Node> GetNeighborsNode(Node nodeToSearch, float distanceNode)
        {
            var neighbors = new Queue<Node>();

            foreach (var node in nodes)
                for (var i = 1; i <= 4; i++)
                {
                    Vector2 positionToSearch;
                    switch (i)
                    {
                        case 1:
                            positionToSearch = nodeToSearch.Position + Vector2.up * distanceNode;
                            break;
                        case 2:
                            positionToSearch = nodeToSearch.Position + Vector2.right * distanceNode;
                            break;
                        case 3:
                            positionToSearch = nodeToSearch.Position + Vector2.down * distanceNode;
                            break;
                        default:
                            positionToSearch = nodeToSearch.Position + Vector2.left * distanceNode;
                            break;
                    }

                    if (node.Position.Equals(positionToSearch))
                    {
                        neighbors.Enqueue(node);
                        break;
                    }
                }

            return neighbors;
        }
    }

    public class Node
    {
        public Vector2 Position { get { return position; } } 
        public float DistanceToDestiny { get { return distanceToDestiny; } } 

        Vector2 position;
        float distanceToDestiny;

        public Node(Vector2 position, float distanceToDestiny)
        {
            this.position = position;
            this.distanceToDestiny = distanceToDestiny;
        }
    }

    MapGrid mapGrid;

    void Awake()
    {
        mapGrid = new MapGrid();

        var listTransform = GetComponentsInChildren<Transform>();
        foreach (var transform in listTransform)
            if (transform.CompareTag("Ground"))
            {
                var distanceToDestiny = Vector2.Distance(transform.position, destiny.position);
                var node = new Node(transform.position, distanceToDestiny);
                mapGrid.AddNode(node);
            }
    }

    public Vector2 GetPositionFromStart()
    {
        return start.position;
    }

    public Node GetNodeByPosition(Vector2 position)
    {
        return mapGrid.GetNodeByPosition(position);
    }

    public Queue<Node> GetNeighborsNodeByPosition(Vector2 position)
    {
        var node = mapGrid.GetNodeByPosition(position);
        var neighborsNode = mapGrid.GetNeighborsNode(node, distanceNode);
        return neighborsNode;
    }

    public bool IsDestinyNodePosition(Vector2 position)
    {
        return destiny.position.Equals(position);
    }
}

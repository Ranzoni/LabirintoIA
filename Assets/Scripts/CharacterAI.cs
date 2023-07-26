using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GroundManager;

public class CharacterAI : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    GroundManager groundManager;
    Vector2 nextPosition;
    Queue<Node> nodesVisited = new();
    Dictionary<Node, Queue<Node>> pathTestedBetweenNodes = new();
    Node currentNode;

    void Start()
    {
        groundManager = FindObjectOfType<GroundManager>();
        transform.position = groundManager.GetPositionFromStart();
        nextPosition = transform.position;
        nodesVisited.Enqueue(groundManager.GetNodeByPosition(transform.position));

        StartCoroutine(NextPositionRoutine());
    }

    IEnumerator NextPositionRoutine()
    {
        while(true)
        {
            if (nextPosition.Equals(transform.position))
            {
                currentNode = groundManager.GetNodeByPosition(transform.position);
                var neighbors = groundManager.GetNeighborsNodeByPosition(transform.position);
                var nearestPosition = GetNearestPosition(neighbors);
                nextPosition = nearestPosition;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    void Update()
    {
        if (groundManager.IsDestinyNodePosition(transform.position))
        {
            var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            var nextSceneIndex = currentSceneIndex + 1;
            if (nextSceneIndex > SceneManager.sceneCountInBuildSettings - 1)
                return;

            SceneManager.LoadScene(currentSceneIndex + 1);
        }

        transform.position = Vector2.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);
    }

    Vector2 GetNearestPosition(Queue<Node> nodes)
    {
        Node nearestNode = null;
        var latestLowerCostNode = float.MaxValue;
        foreach (var node in nodes)
        {
            var distanceToCurrentNode = Vector2.Distance(transform.position, node.Position);
            var totalCost = node.DistanceToDestiny + distanceToCurrentNode;
            if (totalCost <= latestLowerCostNode && !nodesVisited.Any(n => n.Equals(node)) && !PathTested(currentNode, node))
            {
                nearestNode = node;
                latestLowerCostNode = totalCost;
            }
        }

        if (nearestNode is null)
        {
            nodesVisited.Clear();
            nodesVisited.Enqueue(currentNode);
            return GetNearestPosition(groundManager.GetNeighborsNodeByPosition(currentNode.Position));
        }

        AddPathTested(currentNode, nearestNode);
        nodesVisited.Enqueue(nearestNode);
        return nearestNode is not null ? nearestNode.Position : new Vector2(0, 0);
    }

    void AddPathTested(Node currentNode, Node neighbor)
    {
        if (!pathTestedBetweenNodes.ContainsKey(currentNode))
            pathTestedBetweenNodes.Add(currentNode, new Queue<Node>());

        var neighbors = pathTestedBetweenNodes[currentNode];
        neighbors.Enqueue(neighbor);
    }

    bool PathTested(Node currentNode, Node neighbor)
    {
        if (!pathTestedBetweenNodes.ContainsKey(currentNode))
            return false;

        var neighbors = pathTestedBetweenNodes[currentNode];
        var pathIsTested = neighbors.Any(n => n.Equals(neighbor));
        return pathIsTested;
    }
}

using UnityEngine;
using System.Collections.Generic;

public class AStar {
    //Reference to client class
    public static Map clientMap;

    class Node {
        public Node parent;
        public Tuple<int, int> position;
        public float fScore, gScore, heuristicScore;

        public Node(Node par, Tuple<int, int>pos, float g, float h) {
            parent = par;
            position = pos;
            fScore = g + h;
            gScore = g;
            heuristicScore = h;
        }

        public Node(Tuple<int, int> pos) {
            parent = null;
            position = pos;
            fScore = 0f;
            gScore = 0f;
            heuristicScore = 0f;
        }

        public void SetNode(Node par, float g, float h) {
            parent = par;
            fScore = g + h;
            gScore = g;
            heuristicScore = h;
        }
    }

    private static List<Node> closedSet = new List<Node>();
    private static List<Node> openSet = new List<Node>();
    private static List<Node> allSet = new List<Node>();
    private static Node Origin;
    private static Node Destination;

    public static void InitializeAStar(Map cMap, Tuple<int, int>[] nodeTupleArray) {
        clientMap = cMap;
        for (int i = 0; i < nodeTupleArray.Length; i++) {
            allSet.Add(new Node(nodeTupleArray[i]));
        }
    }

    public static List<Tuple<int, int>> PathFind(Tuple<int, int> origin, Tuple<int, int> destination) {
        Origin = allSet[clientMap.NodeIndex(origin)];
        Origin.parent = Origin;
        Destination = allSet[clientMap.NodeIndex(destination)];
        openSet.Clear();
        closedSet.Clear();
        openSet.Add(Origin);
        Node current = null;
        Node previous = null;
        while (openSet.Count > 0) {
            if (openSet.Count == 1) { current = openSet[0]; }         
            else {
                int bestFIndex = 0;
                previous = current;
                for (int i = 0; i < openSet.Count; i++) {
                    if(openSet[i].fScore < openSet[bestFIndex].fScore) { bestFIndex = i; }
                }
                current = openSet[bestFIndex];
                openSet.Remove(current);
            }
            if (Tuple<int, int>.CompareItems(current.position, destination)) {
                return ReconstructPath(current);
            }
            foreach (Tuple<int, int> nodePos in clientMap.GetAdjList(current.position, true)) {
                float tentativeGScore = current.gScore + clientMap.GetCellWeight(nodePos);
                int neighborNodeIndex = clientMap.NodeIndex(nodePos);
                Node currentSuccessor = allSet[neighborNodeIndex];

                if (openSet.Contains(currentSuccessor)) {
                    if (currentSuccessor.gScore <= tentativeGScore) { continue; }
                }
                else if (closedSet.Contains(currentSuccessor)) {
                    if (currentSuccessor.gScore <= tentativeGScore) { continue; }
                    closedSet.Remove(currentSuccessor);
                    openSet.Add(currentSuccessor);
                }
                else {
                    openSet.Add(currentSuccessor);
                    currentSuccessor.heuristicScore = ComputeHeuristicScore(nodePos, destination);
                }
                currentSuccessor.gScore = tentativeGScore;
                currentSuccessor.parent = current;
            }
            closedSet.Add(current);
        }
        return null;
    }

    private static float ComputeHeuristicScore(Tuple<int, int> orig, Tuple<int, int> dest) {
        return (Mathf.Abs(orig.First - dest.First) + Mathf.Abs(orig.Second - dest.Second));
    }

    private static List<Tuple<int, int>> ReconstructPath(Node current) {
        var totalPath = new List<Tuple<int, int>>();
        totalPath.Add(current.position);
        current = current.parent;
        while (current != current.parent) {
            //clientMap.HightLightCell(current.position);
            totalPath.Add(current.position);
            current = current.parent;
        }

        return totalPath;
    }
}

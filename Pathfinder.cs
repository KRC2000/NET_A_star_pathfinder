using System;
using System.Collections.Generic;

namespace Framework.Pathfinder
{
    enum PathMode
    {
        Aligned, Diagonal, Free
    }
    // n - neighbour, t - target
    // >> Aligned (Only vertical and horizontal movement)
    //        n
    //      n t n
    //        n
    // >> Diagonal (Only diagonal movement)
    //      n   n
    //        t
    //      n   n 
    // >> Free (Movement in any direction)
    //      n n n
    //      n t n
    //      n n n
    class Pathfinder
    {
        public PathMode Mode { get; set; } = PathMode.Free;
        private List<Node> nodes;
        private List<Node> openedNodes;
        private Vector2i mapSize;
        private Vector2i startPos, finishPos;

///<summary> Creates node grid for passed map
        // 0 - empty space
        // 1 - obstacle
        public void Init(in uint[,] map)
        {
            mapSize.X = map.GetLength(1);
            mapSize.Y = map.GetLength(0);
            GenNodes(map);
        }
        ///<summary>Calculates path. Use after Init() call.</summary>
        ///<returns>true - if path was found, false otherwise</returns>
        ///<param name="path">Path list to be filled with solving positions</param>
        public bool GetPath(out List<Vector2i> path, in Vector2i startNodePos, in Vector2i finishNodePos)
        {
            if (GetNodeAt(startNodePos) == null || GetNodeAt(finishNodePos) == null)
                throw new Exception("Start/Finish position is out of map bounds or this position is occupied by obstacle.");

            startPos = startNodePos; finishPos = finishNodePos;
            ClearNodesCache();

            path = new List<Vector2i>();

            Node currentNode = GetNodeAt(startPos);

            while (true)
            {
                currentNode.state = NodeState.Expired;
                openedNodes.Remove(currentNode);
               
                OpenNeighbourNodes(currentNode);

                // If there are no available opened nodes - all nodes are discovered, target node not found, there is no posible path
                if (openedNodes.Count == 0) return false;

                currentNode = GetLowestCostNode_f();

                // If finish node reached -> collect solving node's positions and break;
                if (currentNode.pos == finishPos)
                {
                    FillPath(path, currentNode);
                    break;
                }
            }
            
            return true;
        }

        private void FillPath(List<Vector2i> pathList, Node currentNode)
        {
            pathList.Add(currentNode.pos);
            while (currentNode.parent != null)
            {
                currentNode = currentNode.parent;
                pathList.Insert(0, currentNode.pos);
            }
        }
        private Node GetLowestCostNode_f()
        {
            Node returnNode = null;
            openedNodes.Sort((Node node1, Node node2) => { if (node1.f < node2.f) return -1; else if (node1.f > node2.f) return 1; else return 0; });
            float lowestCost_f = openedNodes[0].f;
            float lowestCost_h = float.MaxValue;
            
            foreach (var node in openedNodes)
            {
               if (node.f == lowestCost_f)
               {
                    if (node.h < lowestCost_h)
                    {
                        lowestCost_h = node.h;
                        returnNode = node;
                    }
               } 
               else break;
            }
                return returnNode;
        }
        private void OpenNeighbourNodes(Node currentNode)
        {
            List<Node> neighbors = GetNeighbors(currentNode);

             foreach (var node in neighbors)
                {
                    if (node.state == NodeState.Idle)
                    {
                        node.parent = currentNode;
                        node.RecalculateValues(finishPos);
                        node.state = NodeState.Opened;
                        openedNodes.Add(node);
                    }
                    else if (node.state == NodeState.Opened)
                    {
                        if (node.GetValueAccordingToParent_g(currentNode) < node.g)
                        {
                            node.parent = currentNode;
                            node.RecalculateValuesBut_h();
                        }
                    }
                }
        }
        private void GenNodes(in uint[,] map)
        {
            nodes = new List<Node>();
            openedNodes = new List<Node>();
            for (int y = 0; y < mapSize.Y; y++)
            {
                for (int x = 0; x < mapSize.X; x++)
                {
                    if (map[y,x] == 0)
                    {
                        Node n = new Node(){pos=new Vector2i(x, y)};
                        nodes.Add(n);
                    }
                }
            }
        }

        private List<Node> GetNeighbors(Node targetNode)
        {
            List<Node> neighbors = new List<Node>();

            if (Mode == PathMode.Aligned)
            {
                foreach (var node in nodes)
                {
                    if (((node.pos.X == targetNode.pos.X + 1 || node.pos.X == targetNode.pos.X - 1) && node.pos.Y == targetNode.pos.Y) ||
                        ((node.pos.Y == targetNode.pos.Y + 1 || node.pos.Y == targetNode.pos.Y - 1) && node.pos.X == targetNode.pos.X))
                        neighbors.Add(node);
                }
            }
            else if (Mode == PathMode.Diagonal)
            {
                foreach (var node in nodes)
                {
                    if ((node.pos.X == targetNode.pos.X + 1 || node.pos.X == targetNode.pos.X - 1) &&
                        (node.pos.Y == targetNode.pos.Y + 1 || node.pos.Y == targetNode.pos.Y - 1))
                        neighbors.Add(node);
                }
            }
            else if (Mode == PathMode.Free)
            {
                foreach (var node in nodes)
                {
                    if ((node.pos.X <= targetNode.pos.X + 1 && node.pos.X >= targetNode.pos.X - 1) &&
                        (node.pos.Y <= targetNode.pos.Y + 1 && node.pos.Y >= targetNode.pos.Y - 1))
                        neighbors.Add(node);
                }
            }

            return neighbors;
        }

        public Node GetNodeAt(Vector2i pos)
        {
            foreach (var node in nodes)
            {
                if (node.pos == pos)
                    return node;
            }
            return null;
        }
        
        private void ClearNodesCache()
        {
            openedNodes.Clear();
            foreach (var node in nodes)
                node.Reset();
        }
    }
}
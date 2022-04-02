using System;
using System.Collections.Generic;

namespace Framework.Pathfinder
{
    /// <summary>
    ///  n - neighbour, t - target 
    ///>> Aligned (Only vertical and horizontal movement)
    ///       n
    ///     n t n
    ///       n
    ///>> Diagonal (Only diagonal movement)
    ///     n   n
    ///       t
    ///     n   n 
    ///>> Free (Movement in any direction)
    ///     n n n
    ///     n t n
    ///     n n n
    /// </summary>
    enum PathMode
    {
        Aligned, Diagonal, Free
    }
    
    class Pathfinder
    {
        public PathMode Mode { get; set; } = PathMode.Free;
        private List<Node> nodes;
        private List<Node> openedNodes;

        private uint mapSizeX, mapSizeY;
        private uint startPosX, startPosY;
        private uint finishPosX, finishPosY;

        ///<summary> Creates node grid that is used to calculate paths for passed map </summary>
        ///<param name="map"> 2d array map where 0 - empty "walkable" space, any other positive number - obstacle</param>
        public void Init(in uint[,] map)
        {
            mapSizeX = (uint)map.GetLength(1);
            mapSizeY = (uint)map.GetLength(0);
            GenNodes(map);
        }

        ///<summary>Calculates path. Use after Init() call.</summary>
        ///<returns>true - if path was found, false otherwise</returns>
        ///<param name="path">Path list to be filled with solving positions</param>
        public bool GetPath(out List<uint> path, uint startNodePosX, uint startNodePosY, uint finishNodePosX, uint finishNodePosY)
        {
            if (GetNodeAt(startNodePosX, startNodePosY) == null || GetNodeAt(finishNodePosX, finishNodePosY) == null)
                throw new Exception("Start/Finish position is out of map bounds or this position is occupied by obstacle.");

            startPosX = startNodePosX; finishPosX = finishNodePosX;
            startPosY = startNodePosY; finishPosY = finishNodePosY;
            ClearNodesCache();

            Node currentNode = GetNodeAt(startPosX, startPosY);

            while (true)
            {
                currentNode.state = NodeState.Expired;
                openedNodes.Remove(currentNode);
               
                OpenNeighbourNodes(currentNode);

                // If there are no available opened nodes - all nodes are discovered, target node not found, there is no posible path
                if (openedNodes.Count == 0) { 
                    path = null; return false; 
                }

                currentNode = GetLowestCostNode_f();

                // If finish node reached -> collect solving node's positions and break;
                if (currentNode.posX == finishPosX && currentNode.posY == finishPosY)
                {
                    FillPath(out path, currentNode);
                    break;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Traces back all the final node heritage adding their positions to the list and filling up "pathList" parameter with them. 
        /// </summary>
        /// <param name="pathList"> List to be filled with path solving positions </param>
        /// <param name="currentNode"> Finish node that has been discovered through path finding </param>
        private void FillPath(out List<uint> pathList, Node currentNode)
        {
            pathList = new List<uint>();
            pathList.Add(currentNode.posX);
            pathList.Add(currentNode.posY);
            while (currentNode.parent != null)
            {
                currentNode = currentNode.parent;
                pathList.Insert(0, currentNode.posY);
                pathList.Insert(0, currentNode.posX);
            }
        }

        /// <summary>
        /// Searches open node with the lowest f-cost which is summ of g(distance to the start through parents) and h-cost(distance to the finish) 
        /// if there are multiple - returns the one with lowest h-cost.
        /// </summary>
        /// <returns> Node with lowest f cost, if there are multiple - the one with lowest h-cost </returns>
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
        /// <summary>
        /// Discovers nodes around passed node. Calculates all costs and puts nodes in opened state. 
        /// </summary>
        /// <param name="currentNode"> Node to discover nodes around </param>
        private void OpenNeighbourNodes(Node currentNode)
        {
            List<Node> neighbors = GetNeighbors(currentNode);

             foreach (var node in neighbors)
                {
                    if (node.state == NodeState.Idle)
                    {
                        node.parent = currentNode;
                        node.RecalculateValues(finishPosX, finishPosY);
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

        /// <summary>
        /// Creates nodes list according to the map to use for path calculation. Needs to be done only once for the map. 
        /// </summary>
        /// <param name="map"> Array to gen nodes of. In array: 0 - empty "walkable" space, any other value - obstacle </param>
        private void GenNodes(in uint[,] map)
        {
            nodes = new List<Node>();
            openedNodes = new List<Node>();
            for (uint y = 0; y < mapSizeY; y++)
            {
                for (uint x = 0; x < mapSizeX; x++)
                {
                    if (map[y,x] == 0)
                    {
                        Node n = new Node() { posX = x, posY = y };
                        nodes.Add(n);
                    }
                }
            }
        }

        /// <summary>
        /// Returns list of node neighbours according to the pathfinding mode. See "PathMode" enum comment for modes description. 
        /// </summary>
        /// <param name="targetNode"> Node to look for neighbour nodes around </param>
        /// <returns> List of found neighbour nodes, empty list if no found </returns>
        private List<Node> GetNeighbors(Node targetNode)
        {
            List<Node> neighbors = new List<Node>();

            if (Mode == PathMode.Aligned)
            {
                foreach (var node in nodes)
                {
                    if (((node.posX == targetNode.posX + 1 || node.posX == (int)targetNode.posX - 1) && node.posY == targetNode.posY) ||
                        ((node.posY == targetNode.posY + 1 || node.posY == (int)targetNode.posY - 1) && node.posX == targetNode.posX))
                        neighbors.Add(node);
                }
            }
            else if (Mode == PathMode.Diagonal)
            {
                foreach (var node in nodes)
                {
                    if ((node.posX == targetNode.posX + 1 || node.posX == (int)targetNode.posX - 1) &&
                        (node.posY == targetNode.posY + 1 || node.posY == (int)targetNode.posY - 1))
                        neighbors.Add(node);
                }
            }
            else if (Mode == PathMode.Free)
            {
                foreach (var node in nodes)
                {
                    if ((node.posX <= targetNode.posX + 1 && node.posX >= (int)targetNode.posX - 1) &&
                        (node.posY <= targetNode.posY + 1 && node.posY >= (int)targetNode.posY - 1))
                        neighbors.Add(node);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// Gets node with exact coordinates. 
        /// </summary>
        /// <param name="pos"> Position of needed node </param>
        /// <returns> Node with passed position, null if none found. </returns>
        public Node GetNodeAt(uint posX, uint posY)
        {
            foreach (var node in nodes)
            {
                if (node.posX == posX && node.posY == posY)
                    return node;
            }
            return null;
        }
        
        /// <summary>
        /// Clears node's variables and resets state to NodeState.Idle 
        /// </summary>
        private void ClearNodesCache()
        {
            openedNodes.Clear();
            foreach (var node in nodes)
                node.Reset();
        }
    }
}
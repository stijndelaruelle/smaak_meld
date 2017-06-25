using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    public class Node
    {
        private Waypoint m_Waypoint; //The actual waypoint this is a wrapper around
        public Waypoint Waypoint
        {
            get { return m_Waypoint; }
        }

        private Node m_Parent;
        public Node Parent
        {
            get { return m_Parent; }
            set { m_Parent = value; }
        }

        private float m_G; //Cost to get here
        public float G
        {
            get { return m_G; }
            set { m_G = value; }
        }

        private float m_H; //Cost to get to the target
        public float H
        {
            get { return m_H; }
            set { m_H = value; }
        }

        public float F
        {
            get { return m_G + m_H; }
        }


        public Node(Waypoint waypoint, Node parent)
        {
            m_Waypoint = waypoint;
            m_Parent = parent;

            m_G = float.MaxValue;
            m_H = float.MaxValue;
        }

        public void CalculateValues(Node targetNode)
        {
            //G
            if (m_Parent != null)
            {
                float newG = m_Parent.G + (m_Parent.Waypoint.Position - m_Waypoint.Position).magnitude;

                //Only update when better
                if (newG < m_G)
                    m_G = newG;
            }
            else
            {
                m_G = 0.0f;
            }

            //H
            float distance = (targetNode.Waypoint.Position - m_Waypoint.Position).magnitude;
            m_H = distance;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            Node otherNode = (Node)obj;
            if (otherNode == null)
                return false;

            return (this.m_Waypoint == otherNode.m_Waypoint);
        }

        public override int GetHashCode()
        {
            //Implemented anti-warning. Will never use this.
            //In case you do: please implement.
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return m_Waypoint.name;
        }
    }

    private List<Node> m_OpenList;
    private List<Node> m_ClosedList;

    private List<Waypoint> m_LastPath;
    public List<Waypoint> LastPath
    {
        get { return m_LastPath; }
    }

    public PathFinder()
    {
        m_OpenList = new List<Node>();
        m_ClosedList = new List<Node>();
        m_LastPath = new List<Waypoint>();
    }

    public void CalculatePath(Waypoint startWaypoint, Waypoint targetWaypoint)
    {
        //Wrap Node around startWaypoint & targetWaypoint
        Node startNode = new Node(startWaypoint, null);
        Node targetNode = new Node(targetWaypoint, null);

        startNode.CalculateValues(targetNode);

        //Add start node to the open list
        m_OpenList.Add(startNode);

        //Begin searching!
        Node currentNode = null;
        bool isRunning = true;
        while (isRunning)
        {
            //Find node in open list with lowest overal (F) value.
            Node lastCurrentNode = currentNode;
            currentNode = FindOpenNodeWithLowestF();
            if (currentNode == null)
            {
                //This should never happen, but you never know.
                //(OpenList is empty and we didn't find a path!)
                Debug.LogError("No path found! Last checked Node: " + lastCurrentNode.ToString(), lastCurrentNode.Waypoint);
                isRunning = false;
                continue;
            }

            //Remove the current node from the open list (so it doesn't get searched again), and add it to the closed list.
            m_OpenList.Remove(currentNode);
            m_ClosedList.Add(currentNode);

            //We reached our goal!
            if (currentNode.Equals(targetNode)) //Equals is important as they are not the same instance!
            {
                isRunning = false;
                continue;
            }

            //Iterate trough all the available neighbours
            List<Waypoint> neighbours = currentNode.Waypoint.GetNeighbours();
            for (int neighbourID = 0; neighbourID < neighbours.Count; ++neighbourID)
            {
                //If this neighbour is in the closed list, ignore.
                Node neighbourNode = new Node(neighbours[neighbourID], currentNode);
                if (m_ClosedList.Contains(neighbourNode))
                    continue;

                //If the neighbour is in the open list, update. (if not: add)
                int foundIndex = m_OpenList.IndexOf(neighbourNode);
                if (foundIndex >= 0)
                {
                    neighbourNode = m_OpenList[foundIndex];
                }
                else
                {
                    m_OpenList.Add(neighbourNode);
                }

                neighbourNode.CalculateValues(targetNode);
            }
        }

        //We can now trace back our path from currentNode to StartNode
        TracePath(currentNode);
    }

    private Node FindOpenNodeWithLowestF()
    {
        if (m_OpenList == null)
            return null;

        if (m_OpenList.Count <= 0)
            return null;

        Node bestNode = m_OpenList[0];

        foreach (Node node in m_OpenList)
        {
            if (node.F < bestNode.F)
                bestNode = node;
        }

        return bestNode;
    }

    private void TracePath(Node finalNode)
    {
        if (finalNode == null)
            return;

        m_LastPath.Clear();
        m_LastPath.Add(finalNode.Waypoint);

        Node parent = finalNode.Parent;

        while (parent != null)
        {
            m_LastPath.Add(parent.Waypoint);
            parent = parent.Parent;
        }

        m_LastPath.Reverse();
        Debug.Log("Path completed!");
    }

#if UNITY_EDITOR
    public void DrawPath()
    {
        if (m_LastPath == null)
            return;

        if (m_LastPath.Count <= 0)
            return;

        //Draw ourselves
        Color originalColor = Gizmos.color;
        Gizmos.color = Color.green;

        Vector3 offset = new Vector3(0, 1, 0);
        for (int i = 0; i < m_LastPath.Count - 1; ++i)
        {
            Gizmos.DrawLine(m_LastPath[i].Position + offset, m_LastPath[i + 1].Position + offset);
            //DrawArrow.GizmoLine(m_LastPath[i].Position + offset, m_LastPath[i + 1].Position + offset, 1.0f, 25, true);
        }

        Gizmos.color = originalColor;
    }
#endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> m_Neighbours; //Waypoint & distance to waypoint combined (we can include speed limits here?)

    //NOTE YET USED. OPTIMIZTION FOR LATER
    //[HideInInspector]
    [SerializeField]
    private List<float> m_NeighbourDistance; //Cache it. Thought about using a dictionary but that doesn't work as easily with the inspector.

    public Vector3 Position
    {
        //Small shortcut
        get { return gameObject.transform.position; }
    }

    private void Awake()
    {
        m_NeighbourDistance = new List<float>();
    }

    public void AddNeighbour(Waypoint neighbour)
    {
        if (m_Neighbours == null)
            return;

        float distance = (neighbour.transform.position - gameObject.transform.position).magnitude;

        m_Neighbours.Add(neighbour);
        m_NeighbourDistance.Add(distance);
    }

    public List<Waypoint> GetNeighbours()
    {
        return m_Neighbours;
    }

    public float GetNeighbourDistance(int id)
    {
        if (id < 0 || id >= m_NeighbourDistance.Count)
            return float.MaxValue;

        return m_NeighbourDistance[id];
    }

    public bool IsNeighbour(Waypoint waypoint)
    {
        return (m_Neighbours.Contains(waypoint));
    }

    private void RecalculateDistances()
    {
        m_NeighbourDistance.Clear();

        foreach (Waypoint neighbour in m_Neighbours)
        {
            float distance = (neighbour.transform.position - gameObject.transform.position).magnitude;
            m_NeighbourDistance.Add(distance);
        }

        Debug.Log("Recalculated distances!");
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //Draw ourselves
            Color originalColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 1.0f);

            Gizmos.color = originalColor;
        }

        private void OnDrawGizmosSelected()
        {
            Color originalColor = Gizmos.color;
            Gizmos.color = Color.yellow;

            //Draw lines to our neighbours
            if (m_Neighbours != null)
            {
                foreach (Waypoint neighbour in m_Neighbours)
                {
                    //Very heavy.
                    //Vector3 diff = neighbour.transform.position - transform.position;
                    //Vector3 perpendicular = new Vector3(diff.z, diff.y, -diff.x);
                    //Vector3 offset = perpendicular.normalized * 0.5f;
                    Vector3 offset = Vector3.zero;

                    if (neighbour != null)
                        DrawArrow.GizmoLine(transform.position + offset, neighbour.transform.position + offset, 1.0f, 25, true);
                }
            }
            Gizmos.color = originalColor;
        }

        private void OnValidate()
        {
            //RecalculateDistances();     
        }
    #endif

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        Waypoint otherWaypoint = (Waypoint)obj;
        if (otherWaypoint == null)
            return false;

        return (this.Position == otherWaypoint.Position &&
                this.m_Neighbours == otherWaypoint.m_Neighbours);
    }
}

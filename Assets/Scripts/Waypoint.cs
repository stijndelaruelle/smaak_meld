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

    public Vector3 Position
    {
        //Small shortcut
        get { return gameObject.transform.position; }
    }

    public void AddNeighbour(Waypoint neighbour)
    {
        if (m_Neighbours == null)
            return;

        m_Neighbours.Add(neighbour);
    }

    public List<Waypoint> GetNeighbours()
    {
        return m_Neighbours;
    }

    public bool IsNeighbour(Waypoint waypoint)
    {
        return (m_Neighbours.Contains(waypoint));
    }

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

    public override int GetHashCode()
    {
        //Implemented anti-warning. Will never use this.
        //In case you do: please implement.
        return base.GetHashCode();
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
                Vector3 diff = neighbour.transform.position - transform.position;
                Vector3 perpendicular = new Vector3(diff.z, diff.y, -diff.x);
                Vector3 offset = perpendicular.normalized * 0.5f;
                //Vector3 offset = Vector3.zero;

                if (neighbour != null)
                {
                    Gizmos.DrawLine(transform.position + offset, neighbour.transform.position + offset);
                    //DrawArrow.GizmoLine(transform.position + offset, neighbour.transform.position + offset, 1.0f, 25, true);
                }
            }
        }
        Gizmos.color = originalColor;
    }
#endif
}

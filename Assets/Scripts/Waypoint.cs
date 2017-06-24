using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

public class Waypoint : MonoBehaviour
{
    [SerializeField]
    private List<Waypoint> m_Neighbours;

    public void AddNeightbour(Waypoint neighbour)
    {
        if (m_Neighbours != null)
            m_Neighbours.Add(neighbour);
    }

    public bool IsNeighbour(Waypoint waypoint)
    {
        return (m_Neighbours.Contains(waypoint));
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

    void OnDrawGizmosSelected()
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

#endif
}

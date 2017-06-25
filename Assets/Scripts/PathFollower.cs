using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFollower : MonoBehaviour
{
    [SerializeField]
    private float m_MaxSpeed;

    [SerializeField]
    private float m_RotationSpeed;

    [SerializeField]
    private Waypoint m_CurrentWaypoint;

    [SerializeField]
    private Waypoint m_TargetWaypoint;

    private PathFinder m_Pathfinder;

    //Cache
    private List<Waypoint> m_LastPath;
    private float m_LastPathLength;
    private Coroutine m_DrivingCoroutine;

    public void CalculatePath()
    {
        m_Pathfinder = new PathFinder();
        m_Pathfinder.CalculatePath(m_CurrentWaypoint, m_TargetWaypoint);

        m_LastPath = m_Pathfinder.LastPath;
        m_LastPathLength = m_Pathfinder.LastPathLength;
    }

    public void StartFollowing()
    {
        if (m_LastPath == null)
        {
            Debug.LogWarning("No path has yet been calculated!");
            return;
        }

        if (m_LastPath.Count <= 0)
        {
            Debug.LogWarning("Path has no waypoints!");
            return;
        }

        if (m_DrivingCoroutine != null)
            StopCoroutine(m_DrivingCoroutine);

        m_DrivingCoroutine = StartCoroutine(DriveRoutine());
    }

    private IEnumerator DriveRoutine()
    {
        //Hard set to position
        transform.position = m_LastPath[0].Position;

        //Loop trough the path, going from waypoint to waypoint
        for (int targetPathID = 1; targetPathID < m_LastPath.Count; ++targetPathID)
        {
            Vector3 direction = (m_LastPath[targetPathID].Position - m_LastPath[targetPathID - 1].Position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            //Hard set to rotation
            if (targetPathID == 1)
                transform.rotation = targetRotation;

            float speed = m_MaxSpeed; //Can later change from node to node.

            //Drive in the direction until we reach it
            Vector3 newDirection = (m_LastPath[targetPathID].Position - transform.position);
            float dot = Vector3.Dot(direction, newDirection);

            //If the dot product is negative it means we've driven past our destination. Time to move on!
            while (dot > 0)
            {
                //Move
                transform.position += direction * speed * Time.deltaTime;

                //Rotate slowly
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_RotationSpeed);

                //Calculate variables to check if we reached or waypoint.
                newDirection = (m_LastPath[targetPathID].Position - transform.position);
                dot = Vector3.Dot(direction, newDirection);

                yield return null;
            }
        }

        Debug.Log("Finished driving!");
        m_DrivingCoroutine = null;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_Pathfinder != null)
            m_Pathfinder.DrawPath();
    }
    #endif
}
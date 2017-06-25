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
    private GameObject m_SelectionRing;

    private Waypoint m_CurrentWaypoint;
    private Waypoint m_TargetWaypoint;

    private PathFinder m_Pathfinder;
    private bool m_IsSelected = false;

    //Cache
    private List<Waypoint> m_LastPath;
    private Coroutine m_DrivingCoroutine;

    private void Start()
    {
        SetSelection(m_IsSelected);    
    }

    public void CalculatePath()
    {
        m_Pathfinder = new PathFinder();

        if (m_CurrentWaypoint == null)
        {
            m_CurrentWaypoint = FindClosestWaypoint(transform.position, 50.0f);
            if (m_CurrentWaypoint == null)
                return;
        }

        m_Pathfinder.CalculatePath(m_CurrentWaypoint, m_TargetWaypoint);

        m_LastPath = m_Pathfinder.LastPath;
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
        //Loop trough the path, going from waypoint to waypoint
        for (int targetPathID = 0; targetPathID < m_LastPath.Count; ++targetPathID)
        {
            m_CurrentWaypoint = m_LastPath[targetPathID];

            Vector3 direction;

            if (targetPathID <= 0)
            {
                //Drive to the start node first
                direction = (m_LastPath[targetPathID].Position - transform.position).normalized;
            }
            else
            {
                //Drive to the next node
                direction = (m_LastPath[targetPathID].Position - m_LastPath[targetPathID - 1].Position).normalized;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction);

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

    //The following 2 functions are just for demonstration purposes.
    //It's not meant to be "the way" to move these vehicles around
    private void OnMouseDown()
    {
        ToggleSelection();
    }

    private void Update()
    {
        //Left click = select unit
        //Implemented in "OnMouseDown"

        //Right click = go to position
        if (Input.GetMouseButtonDown(1) && m_IsSelected)
        {
            Deselect();

            //Transform mouse position orthographically
            Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldMousePosition.y = 0.0f;

            //Set the closest waypoint as our target
            m_TargetWaypoint = FindClosestWaypoint(worldMousePosition, 10.0f);
            CalculatePath();
            StartFollowing();
        }
    }

    private Waypoint FindClosestWaypoint(Vector3 position, float range)
    {
        //Find the closest waypoint within a range of 10 units.
        Collider[] colliders = Physics.OverlapSphere(position, range);
        Waypoint closestWaypoint = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; ++i)
        {
            Waypoint testingWaypoint = colliders[i].GetComponent<Waypoint>();

            //Check if this object has is a waypoint
            if (testingWaypoint == null)
                continue;

            //Do the distance check
            float sqDistance = (testingWaypoint.transform.position - position).sqrMagnitude;
            if (sqDistance < closestDistance)
            {
                closestWaypoint = testingWaypoint;
                closestDistance = sqDistance;
            }
        }

        return closestWaypoint;
    }

    //Selection
    private void Select()
    {
        SetSelection(true);
    }

    private void Deselect()
    {
        SetSelection(false);
    }

    private void ToggleSelection()
    {
        SetSelection(!m_IsSelected);
        
    }

    private void SetSelection(bool value)
    {
        m_IsSelected = value;
        m_SelectionRing.SetActive(m_IsSelected);
    }



    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_Pathfinder != null)
            m_Pathfinder.DrawPath();
    }
    #endif
}
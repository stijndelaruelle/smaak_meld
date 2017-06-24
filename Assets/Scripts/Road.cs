using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

[RequireComponent(typeof(MeshFilter))]
public class Road : MonoBehaviour
{
    [SerializeField]
    private bool m_Debug = false;
    private Mesh m_Mesh;

    //Temporary cache (during serialization) of all our waypoints. Don't use this at runtime
    private List<Waypoint> m_Waypoints;

    //Generating
    public void GenerateWaypoints(Waypoint waypointPrefab)
    {
        m_Mesh = GetComponent<MeshFilter>().sharedMesh;

        if (m_Waypoints != null)
            m_Waypoints.Clear();
        else
            m_Waypoints = new List<Waypoint>();

        //Runtime generated
        if (m_Mesh.name == "New Game Object")
        {
            GenerateWaypointsPreSerialization(waypointPrefab, m_Waypoints);
        }

        //From OBJ
        else
        {
            GenerateWaypointsPostSerialization(waypointPrefab, m_Waypoints);
        }
    }

    public void GenerateWaypointsPreSerialization(Waypoint waypointPrefab, List<Waypoint> waypointList)
    {
        m_Mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = m_Mesh.vertices;
        int[] indices = m_Mesh.triangles;

        //In an unoptimized obj the vertices and indices relate like this.
        //---------------------------------------------------------------------
        //In the first triangle the waypoint should be placed between the 3rd (i + 2) and 1st (i + 0) point in the triangle
        //In the second triangle the waypoint should be placed between the 1st (i + 0) and 2nd (i + 1) point in the triangle

        //Apart from the first and last triangle these overlap 100%
        //Because of this I'm only placing waypoints every other triangle, except for the last triangle

        Waypoint newWaypoint = null;
        for (int i = 0; i < indices.Length; i += 6)
        {
            newWaypoint = SpawnWaypoint(vertices[indices[i + 2]], vertices[indices[i + 0]], waypointPrefab, waypointList);
        }

        //Last waypoint at the end of the mesh
        Vector3 thirdLastVertexPosition = vertices[indices[indices.Length - 3]];
        Vector3 secondLastVertexPosition = vertices[indices[indices.Length - 2]];

        newWaypoint = SpawnWaypoint(thirdLastVertexPosition, secondLastVertexPosition, waypointPrefab, waypointList);
    }

    public void GenerateWaypointsPostSerialization(Waypoint waypointPrefab, List<Waypoint> waypointList)
    {
        m_Mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = m_Mesh.vertices;
        int[] indices = m_Mesh.triangles;

        //AFTER saving to an obj the vertices and indices relate like this.
        //---------------------------------------------------------------------
        //In the first triangle the waypoint should be placed between the 2nd (i + 1) and 3rd (i + 2) point in the triangle
        //In the second triangle the waypoint should be placed between the 3rd (i + 2) and 1st (i + 0) point in the triangle

        //Apart from the first and last triangle these overlap 100%
        //Because of this I'm only placing waypoints every other triangle, except for the last triangle

        Waypoint newWaypoint = null;
        for (int i = 0; i < indices.Length; i += 6)
        {
            newWaypoint = SpawnWaypoint(vertices[indices[i + 1]], vertices[indices[i + 2]], waypointPrefab, waypointList);
        }

        //Last waypoint at the end of the mesh
        Vector3 thirdLastVertexPosition = vertices[indices[indices.Length - 3]];
        Vector3 lastVertexPosition = vertices[indices[indices.Length - 1]];

        newWaypoint = SpawnWaypoint(thirdLastVertexPosition, lastVertexPosition, waypointPrefab, waypointList);
    }

    private Waypoint SpawnWaypoint(Vector3 vertex1, Vector3 vertex2, Waypoint waypointPrefab, List<Waypoint> waypointList)
    {
        //Get the world space of these 2 vertices
        Vector3 position1 = transform.TransformPoint(vertex1);
        Vector3 position2 = transform.TransformPoint(vertex2);

        //Calculate the average
        Vector3 average = (position1 + position2) / 2.0f;

        //Spawn a waypoint at the average
        Waypoint newWaypoint = GameObject.Instantiate(waypointPrefab, average, Quaternion.identity, this.transform) as Waypoint;
        waypointList.Add(newWaypoint);

        return newWaypoint;
    }

    //Linking
    public void LinkWaypoints()
    {
        for (int i = 0; i < m_Waypoints.Count - 1; ++i)
        {
            //Link both ways.
            m_Waypoints[i].AddNeightbour(m_Waypoints[i + 1]);
            m_Waypoints[i + 1].AddNeightbour(m_Waypoints[i]);
        }
    }

    public void LinkRoads()
    {
        //For both the first and last waypoint:
        //Look for the nearest waypoint that it not yet linked with within a certain range.
        LinkWaypointWithNearest(m_Waypoints[0]);
        LinkWaypointWithNearest(m_Waypoints[m_Waypoints.Count - 1]);
    }

    private void LinkWaypointWithNearest(Waypoint currentWaypoint)
    {
        Vector3 waypointPosition = currentWaypoint.transform.position;
        Collider[] colliders = Physics.OverlapSphere(waypointPosition, 2.0f);

        Waypoint closestWaypoint = null;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < colliders.Length; ++i)
        {
            Waypoint testingWaypoint = colliders[i].GetComponent<Waypoint>();

            //Check if this object has is a waypoint
            if (testingWaypoint == null)
                continue;

            //Check if we're not comparing with ourselves
            if (currentWaypoint == testingWaypoint)
                continue;

            //make sure the waypoint isn't part of the same road as ours
            if (m_Waypoints.Contains(testingWaypoint))
                continue;

            //Do the distance check
            float sqDistance = (testingWaypoint.transform.position - waypointPosition).sqrMagnitude;
            if (sqDistance < closestDistance)
            {
                closestWaypoint = testingWaypoint;
                closestDistance = sqDistance;
            }
        }

        //They are now neighbours!
        if (closestWaypoint != null)
        {
            //Unless we already were neighbours.
            if (currentWaypoint.IsNeighbour(closestWaypoint) == false)
            {
                currentWaypoint.AddNeightbour(closestWaypoint);
                closestWaypoint.AddNeightbour(currentWaypoint);
            }
        }
    }

    //Cleanup
    private void RemoveWaypointColliders()
    {

    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!m_Debug)
            return;

        //Show the indices at the vertices. Useful while figuring out how the triangles are generated by MapBox
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<int> usedIndices = new List<int>();

        GUIStyle yellowText = new GUIStyle();
        yellowText.normal.textColor = Color.yellow;

        //Draw indices
        for (int i = 0; i < triangles.Length; ++i)
        {
            int index = triangles[i];
            Vector3 worldPosition = transform.TransformPoint(vertices[index]);

            //Avoid duplicate drawing
            if (usedIndices.Contains(index))
                continue;

            Handles.Label(worldPosition, index.ToString(), yellowText);
            usedIndices.Add(index);
        }
    }
#endif

}

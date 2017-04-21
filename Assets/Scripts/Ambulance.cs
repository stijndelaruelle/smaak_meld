using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ambulance : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent m_NavMeshAgent;

    [SerializeField]
    private Transform m_Target;

    private void Start()
    {
        m_NavMeshAgent.destination = m_Target.position;
    }
}

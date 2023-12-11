using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindRail : MonoBehaviour
{
    public float m_GrindCooldown = 10f;
    public float m_Speed = 1f;
    public float m_LaunchSpeed = 10f;
    public bool m_LockRotation = true;
    public char m_Direction = 'x';
    public GameObject[] m_Nodes;

    private int m_CurrentNode;
    private int m_StartNode;
    private int m_FinishNode;
    private float m_GrindTimer;
    private float m_ReactivateTime;
    private bool m_Grinding;
    private bool m_GrindingDirection;
    private Vector3 m_PrevPos;
    private Vector3 m_NextPos;
    private Quaternion m_PrevRotation;
    private Quaternion m_NextRotation;
    private GameObject m_GrindingPlayer;

    // Start is called before the first frame update
    void Start()
    {
        m_Grinding = false;
        m_GrindingDirection = true;
        m_GrindingPlayer = null;

        m_CurrentNode = 0;
        m_StartNode = 0;
        m_FinishNode = m_Nodes.Length - 1;

        m_PrevPos = Vector3.zero;
        m_NextPos = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_Grinding)
        {
            m_GrindTimer += Time.deltaTime * m_Speed;
            // Check if player is at intended position
            if (m_GrindingPlayer.transform.position != m_NextPos)
            {
                // Lerp position and rotation to next node
                m_GrindingPlayer.transform.position = Vector3.Lerp(m_PrevPos, m_NextPos, m_GrindTimer);
                // Only rotate if the rail is told to rotate the board
                if (m_LockRotation)
                {
                    m_GrindingPlayer.transform.rotation = Quaternion.Lerp(m_PrevRotation, m_NextRotation, m_GrindTimer);
                }
            }
            else
            {
                // Check if we should continue grinding
                bool cont = false;
                cont |= m_GrindingDirection && m_CurrentNode < m_FinishNode - 1;
                cont |= !m_GrindingDirection && m_CurrentNode > m_FinishNode + 1;
                if (cont)
                {
                    m_CurrentNode += m_GrindingDirection ? 1 : -1;
                    UpdateNode();
                }
                else
                {
                    m_Grinding = false;
                    m_ReactivateTime = Time.time + m_GrindCooldown;
                    // Set Player internal grinding flag
                    m_GrindingPlayer.transform.rotation = m_NextRotation;
                    m_GrindingPlayer.GetComponent<SkateMovement>().OnGrindEnd();
                    Rigidbody playerRigidbody = m_GrindingPlayer.GetComponent<Rigidbody>();
                    playerRigidbody.isKinematic = false;
                    playerRigidbody.AddForce(m_GrindingPlayer.transform.forward * m_LaunchSpeed, ForceMode.VelocityChange);
                }
            }
        }
    }

    /**
     * Called whenever something enters the trigger collider
     * Check to make sure that no one is grinding and that it is a player grinding
     * 
     * Set flags and find start and end nodes
     */
    void OnTriggerEnter(Collider collision)
    {
        // Verify we are able and should begin grind
        if (!m_Grinding && collision.gameObject.CompareTag("player"))
        {
            // If we are on cooldown, dont grind
            if (Time.time < m_ReactivateTime)
            {
                return;
            }
            // Set grinding flag, update time to reactivate
            m_Grinding = true;
            m_ReactivateTime = float.PositiveInfinity;
            // Set player member variable
            m_GrindingPlayer = collision.gameObject;
            // Set Player internal grinding flags
            m_GrindingPlayer.GetComponent<SkateMovement>().OnGrind(!m_LockRotation);
            // Check player current direction
            CheckDirection(m_GrindingPlayer);
            // Get player's rigidbody and set to kinematic
            m_GrindingPlayer.GetComponent<Rigidbody>().isKinematic = true;
            // Find start node from player pos, find end node from direction
            m_StartNode = FindNearestNode(m_GrindingPlayer.transform.position);
            m_FinishNode = m_GrindingDirection ? m_Nodes.Length - 1 : 0;
            m_CurrentNode = m_StartNode;
            UpdateNode();
            // Make player lerp from current position instead of snapping to start node
            m_PrevPos = m_GrindingPlayer.transform.position;
        }
    }

    /**
     * Find which direction the player is moving along the axis of the rail
     * 
     * @param player: gameobject corresponding to the player to check
     */
    private void CheckDirection(GameObject player)
    {
        // Get velocity of vector of player
        Vector3 velocity = player.GetComponent<Rigidbody>().velocity;
        switch (char.ToUpper(m_Direction))
        {
            case 'Y':
                m_GrindingDirection = velocity.y >= 0;
                break;
            case 'Z':
                m_GrindingDirection = velocity.z >= 0;
                break;
            // use X axis by default
            default:
                m_GrindingDirection = velocity.x >= 0;
                break;
        }
    }

    /**
     * Searches the list of node and finds the one nearest
     * to a given position
     * 
     * @param position, vector containing position to compare to
     * @return int containing the index of the node that is nearest
     */
    private int FindNearestNode(Vector3 position)
    {
        int nearest = 0;
        float nearestDistance = float.PositiveInfinity;

        // Check each node and calc distance
        for (int i = 0; i < m_Nodes.Length; ++i)
        {
            Vector3 diff = m_Nodes[i].transform.position - position;
            if (diff.sqrMagnitude < nearestDistance)
            {
                nearestDistance = diff.sqrMagnitude;
                nearest = i;
            }
        }

        return nearest;
    }

    private void UpdateNode()
    {
        m_GrindTimer = 0f;
        // If we are already on last node, dont do anything
        bool cont = false;
        cont |= m_GrindingDirection && m_CurrentNode < m_FinishNode;
        cont |= !m_GrindingDirection && m_CurrentNode > m_FinishNode;
        if (cont)
        {
            // Update previous and current nodes
            m_PrevPos = m_Nodes[m_CurrentNode].transform.position;
            m_PrevRotation = m_Nodes[m_CurrentNode].transform.rotation;
            m_NextPos = m_Nodes[m_CurrentNode + (m_GrindingDirection ? 1 : -1)].transform.position;
            m_NextRotation = m_Nodes[m_CurrentNode + (m_GrindingDirection ? 1 : -1)].transform.rotation;
            if (!m_GrindingDirection)
            {
                m_PrevRotation = Quaternion.Euler(m_PrevRotation.eulerAngles.x, m_PrevRotation.eulerAngles.y + 180, m_PrevRotation.eulerAngles.z);
                m_NextRotation = Quaternion.Euler(m_NextRotation.eulerAngles.x, m_NextRotation.eulerAngles.y + 180, m_NextRotation.eulerAngles.z);
            }
        }
    }
}

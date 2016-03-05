using UnityEngine;
using System.Collections;
using Pathfinding;

public class UnitPath : MonoBehaviour
{
    public Path path;
    public float speed;

    public float defaultNextWaypointDistance = 20f;


    private Seeker seeker_;
    private CharacterController controller_;
    private int currentWaypoint_ = 0;
    private UnitScript unitScript_;

    public void Start()
    {
        seeker_ = GetComponent<Seeker>();
        controller_ = GetComponent<CharacterController>();
        unitScript_ = GetComponent<UnitScript>();
    }

    public void LateUpdate()
    {
        if(unitScript_.selected && unitScript_.isWalkable)
        {
            if(Input.GetMouseButtonDown(1))
            {
                seeker_.StartPath(transform.position, GameModeController.rightClickPoint, OnPathComplete);
            }
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;

            currentWaypoint_ = 0;
        }
    }

    public void FixedUpdate()
    {
        if(!unitScript_.isWalkable)
        {
            return;
        }

        if (path == null)
        {
            return;
        }

        if (currentWaypoint_ >= path.vectorPath.Count)
        {
            return;
        }

        Vector3 dir = (path.vectorPath[currentWaypoint_] - transform.position).normalized;
        dir = dir * speed * Time.fixedDeltaTime;
        controller_.SimpleMove(dir);

        transform.LookAt(new Vector3(path.vectorPath[currentWaypoint_].x, transform.position.y, path.vectorPath[currentWaypoint_].z));

        float nextWaypointDistance = defaultNextWaypointDistance;
        if(currentWaypoint_ == path.vectorPath.Count - 1)
        {
            nextWaypointDistance = 0f;
        }

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint_]) < nextWaypointDistance)
        {
            currentWaypoint_++;
            return;
        }
    }
}

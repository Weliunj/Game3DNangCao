using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    protected NavMeshAgent agent;
    protected GameObject player;

    public enum Mode { Stand, Patrol, Random }
    
    [Header("Behavior Settings")]
    public Mode selectMode; 

    [Header("Base Settings")]
    public float attackDistance = 2f;

    [Header("Random Mode Settings")]
    public GameObject center; 
    public float wanderRadius = 10f; // Bán kính đi lang thang
    public float wanderTimer = 5f;   // Thời gian đổi điểm mới
    private float timer;

    protected virtual void Start()
    {
        player = GameObject.FindWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        
        if (agent != null)
        {
            agent.stoppingDistance = attackDistance;
        }
        timer = wanderTimer; // Khởi tạo timer
    }

    protected virtual void Update()
    {
        if (agent == null) return;

        switch (selectMode)
        {
            case Mode.Stand:
                if(player != null) LookAtPlayer();
                agent.isStopped = true;
                break;

            case Mode.Patrol:
                if(player != null) MoveToPlayer();
                break;

            case Mode.Random:
                WanderLogic();
                break;
        }
    }

    // LOGIC ĐI LUNG TUNG (WANDER)
    protected virtual void WanderLogic()
    {
        timer += Time.deltaTime;

        // Nếu hết thời gian chờ hoặc quái đã đến gần điểm đích hiện tại
        if (timer >= wanderTimer || (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending))
        {
            Vector3 newPos = RandomNavMeshLocation(wanderRadius);
            agent.SetDestination(newPos);
            agent.isStopped = false;
            timer = 0; // Reset thời gian
        }
    }

    // Hàm tìm một điểm ngẫu nhiên hợp lệ trên NavMesh
    public Vector3 RandomNavMeshLocation(float radius)
    {
        // Lấy tâm là vị trí của object 'center', nếu không có thì lấy vị trí hiện tại của quái
        Vector3 origin = center != null ? center.transform.position : transform.position;
        
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += origin;
        
        NavMeshHit navHit;
        // Kiểm tra xem điểm ngẫu nhiên có nằm trên NavMesh không trong phạm vi radius
        if (NavMesh.SamplePosition(randomDirection, out navHit, radius, -1))
        {
            return navHit.position;
        }

        return origin; // Nếu không tìm được điểm nào, đứng yên tại tâm
    }

    protected virtual void MoveToPlayer()
    {
        if (player == null) return;
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > agent.stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
            LookAtPlayer();
        }
    }

    protected void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        direction.y = 0;
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center.transform.position, wanderRadius);
    }
}
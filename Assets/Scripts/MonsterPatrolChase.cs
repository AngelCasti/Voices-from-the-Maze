using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MonsterPatrolChase : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public Transform[] waypoints;

    [Header("Distancias")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Velocidades")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Jumpscare")]
    public AudioSource jumpscareAudio;
    public GameObject jumpscarePanel;
    public GameManager gameManager;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypoint = 0;
    private bool hasJumpscared = false;

    private enum State
    {
        Patrol,
        Chase
    }

    private State currentState;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        currentState = State.Patrol;

        if (jumpscarePanel != null)
        {
            jumpscarePanel.SetActive(false);
        }

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }

        // Busca GameManager para el menu final
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    void Update()
    {
        if (player == null || hasJumpscared) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Jumpscare
        if (distance <= attackRange)
        {
            TriggerJumpscare();
            return;
        }

        // Detectar jugador
        if (distance <= detectionRange)
        {
            currentState = State.Chase;
        }
        else
        {
            currentState = State.Patrol;
        }

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;

            case State.Chase:
                Chase();
                break;
        }

        UpdateAnimations();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }

            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void TriggerJumpscare(){
        hasJumpscared = true;
        agent.isStopped = true;

        // Iniciamos la secuencia de tiempo
        StartCoroutine(SecuenciaJumpscareYMenu());
    }

    // Este nuevo método se encarga de pausar el tiempo entre el susto y el menú
    IEnumerator SecuenciaJumpscareYMenu(){
        // 1. Activamos el sonido y la imagen del susto
        if (jumpscareAudio != null) jumpscareAudio.Play();
        if (jumpscarePanel != null) jumpscarePanel.SetActive(true);

        Debug.Log("JUMPSCARE INICIADO");

        // 2. Esperamos 2.5 segundos (puedes cambiar este número si el audio es más corto o largo)
        yield return new WaitForSeconds(2.5f);

        // 3. Quitamos la imagen del jumpscare de la pantalla para que no estorbe
        if (jumpscarePanel != null) jumpscarePanel.SetActive(false);

        // 4. Ahora sí, activamos la interfaz de Game Over limpia y libre de obstáculos
        if (gameManager != null)
        {
            gameManager.Derrota();
        }

        Debug.Log("MENÚ DE DERROTA ACTIVADO");
    }
    

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = agent.velocity.magnitude > 0.1f;

        animator.SetBool("isWalking", isMoving);
    }
}
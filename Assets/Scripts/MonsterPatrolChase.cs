using System.Collections; // <-- ¡ESTA ES LA QUE FALTA!
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Video;

public class MonsterPatrolChase : MonoBehaviour
{
    public GameManager gameManager; // <-- Añade esta línea

    [Header("Referencias")]
    public Transform player;
    public Transform[] waypoints;

    [Header("Distancias")]
    public float detectionRange = 5f;
    public float attackRange = 1.5f;

    [Header("Velocidades")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;

    [Header("Audio Monstruo")]
    public AudioSource monsterFootstepAudio;
    public float monsterStepInterval = 0.6f;
    public AudioSource monsterChaseAudio;

    [Header("Audio Jugador")]
    public AudioSource heartbeatAudio;

    [Header("Jumpscare")]
    public AudioSource jumpscareAudio;
    public GameObject jumpscarePanel;
    public VideoPlayer screamerVideo;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentWaypoint = 0;
    private float monsterStepTimer = 0f;
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
            jumpscarePanel.SetActive(false);

        if (monsterChaseAudio != null)
            monsterChaseAudio.Stop();

        if (heartbeatAudio != null)
            heartbeatAudio.Stop();

        if (screamerVideo != null)
            screamerVideo.Stop();

        if (waypoints.Length > 0)
            agent.SetDestination(waypoints[currentWaypoint].position);
    }

    void Update()
    {
        if (player == null || hasJumpscared) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            TriggerJumpscare();
            return;
        }

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
                StopChaseSounds();
                break;

            case State.Chase:
                Chase();
                PlayChaseSounds();
                break;
        }

        HandleMonsterFootsteps();
        UpdateAnimations();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
                currentWaypoint = 0;

            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void HandleMonsterFootsteps()
    {
        if (monsterFootstepAudio == null) return;

        if (agent.velocity.magnitude > 0.1f)
        {
            monsterStepTimer += Time.deltaTime;

            if (monsterStepTimer >= monsterStepInterval)
            {
                if (!monsterFootstepAudio.isPlaying)
                {
                    monsterFootstepAudio.Play();
                }
                monsterStepTimer = 0f;
            }
        }
        else
        {
            monsterStepTimer = 0f;
        }
    }

    void PlayChaseSounds()
    {
        if (monsterChaseAudio != null && !monsterChaseAudio.isPlaying)
            monsterChaseAudio.Play();

        if (heartbeatAudio != null && !heartbeatAudio.isPlaying)
            heartbeatAudio.Play();
    }

    void StopChaseSounds()
    {
        if (monsterChaseAudio != null && monsterChaseAudio.isPlaying)
            monsterChaseAudio.Stop();

        if (heartbeatAudio != null && heartbeatAudio.isPlaying)
            heartbeatAudio.Stop();
    }

    void TriggerJumpscare()
    {
        hasJumpscared = true;
        agent.isStopped = true;

        StopChaseSounds();

        if (jumpscarePanel != null)
            jumpscarePanel.SetActive(true);

        if (screamerVideo != null)
        {
            screamerVideo.Stop();
            screamerVideo.Play();
        }

        if (jumpscareAudio != null)
            jumpscareAudio.Play();

        StartCoroutine(SecuenciaJumpscareYMenu());
        Debug.Log("JUMPSCARE");
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
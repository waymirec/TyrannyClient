using UnityEngine;
using UnityEngine.AI;

public class LocomotionSimpleAgent : MonoBehaviour
{
    public AudioClip[] footStepsAudio;
    private Animator _anim;
    private AudioSource _audio;
    private NavMeshAgent _agent;
    private Vector2 _smoothDeltaPosition = Vector2.zero;
    private Vector2 _velocity = Vector2.zero;

    public void Footsteps()
    {
        _audio.clip = footStepsAudio[0];
        _audio.Play();
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
        _agent = GetComponent<NavMeshAgent>();
        // Don’t update position automatically
        _agent.updatePosition = false;
    }

    private void Update()
    {
        var worldDeltaPosition = _agent.nextPosition - transform.position;

        // Map 'worldDeltaPosition' to local space
        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2(dx, dy);

        // Low-pass filter the deltaMove
        var smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
        _smoothDeltaPosition = Vector2.Lerp(_smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            _velocity = _smoothDeltaPosition / Time.deltaTime;

        _anim.SetFloat("Forward", _velocity.magnitude);

        //bool shouldMove = _velocity.magnitude > 0.5f && _agent.remainingDistance > _agent.radius;

        // Update animation parameters
        //_anim.SetBool("move", shouldMove);
        //_anim.SetFloat("velx", _velocity.x);
        //_anim.SetFloat("vely", _velocity.y);

        //GetComponent<LookAt>().lookAtTargetPosition = _agent.steeringTarget + transform.forward;
    }

    private void OnAnimatorMove()
    {
        // Update position to agent position
        transform.position = _agent.nextPosition;
    }
}

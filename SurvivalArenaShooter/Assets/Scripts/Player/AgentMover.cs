using UnityEngine;

public class AgentMover : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private Vector3 _previousPosition;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _previousPosition = transform.position;
    }

    private void FixedUpdate()
    {
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        Vector3 currentPosition = transform.position;
        Vector3 displacement = currentPosition - _previousPosition;
        
        Vector3 localDisplacement = transform.InverseTransformDirection(displacement);
        Vector3 normalizedDisplacement = localDisplacement.normalized;
        
        float xInput = normalizedDisplacement.x;
        float yInput = normalizedDisplacement.z; 
        
        _animator.SetFloat("xInput", xInput);
        _animator.SetFloat("yInput", yInput);
        
        _previousPosition = currentPosition;
    }

    public void StopMovement()
    {
        _previousPosition = transform.position;
        _animator.SetFloat("xInput", 0);
        _animator.SetFloat("yInput", 0);
    }
}
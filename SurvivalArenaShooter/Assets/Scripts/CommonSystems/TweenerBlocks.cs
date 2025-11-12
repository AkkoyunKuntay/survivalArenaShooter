using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class TweenerBlocks : MonoBehaviour
{
    [Header("Path")] public Vector3 startPosition;
    public Vector3 endPosition;

    [Header("Tween")] public float duration = 0.75f;
    public Ease ease = Ease.OutCubic;
    public LoopType loopType = LoopType.Restart;
    public int loops = 0;

    [Header("FX")] public bool playParticle = true;
    public ParticleSystem particle;

    [Header("Advanced")] public bool playOnEnable = true;
    public bool setPhysicsIndependentUpdate = false;

    private Sequence _seq;
    private Tween _moveTween;
    private bool _built;

    void OnEnable()
    {
        BuildIfNeeded();

        if (playOnEnable)
            PlayFromStart();
    }

    void OnDisable()
    {
        if (_seq != null)
        {
            _seq.Pause();
            _seq.Rewind();
        }
    }

    private void BuildIfNeeded()
    {
        if (_built) return;

        transform.position = startPosition;
        _moveTween = transform.DOMove(endPosition, duration)
            .SetEase(ease); 

        if (setPhysicsIndependentUpdate)
            _moveTween.SetUpdate(true);
        
        _seq = DOTween.Sequence();

        _seq.Append(_moveTween);
        _seq.AppendCallback(PlayDustFX);

        if (loops != 0)
        {
            _seq.SetLoops(loops, loopType);
        }

        _seq.SetAutoKill(false);
        _seq.Pause();
        _seq.Rewind();

        _built = true;
    }

    public void PlayFromStart()
    {
        BuildIfNeeded();
        transform.position = startPosition;
        _seq.Restart();
    }

    public void PlayFromCurrent()
    {
        BuildIfNeeded();
        _seq.PlayForward();
    }
    private void PlayDustFX()
    {
        if (!playParticle || particle == null) return;

        var tr = particle.transform;
        tr.position = endPosition;
        particle.Clear(true);
        particle.Play(true);
    }
}
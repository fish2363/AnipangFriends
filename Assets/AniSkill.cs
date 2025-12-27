using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AniSkill : Skill
{
    [Header("Cinemachine Camera")]
    [SerializeField] private CinemachineCamera vcam;

    [Header("Camera FOV")]
    [SerializeField] private float fovShrink = 8f;
    [SerializeField] private float fovOvershoot = 4f;
    [SerializeField] private float fovAnimTime = 0.12f;

    [Header("Move Speed")]
    [SerializeField] private float speedMultiplier = 1.5f;

    [Header("Duration")]
    [SerializeField] private float duration = 2.0f;

    [Header("Motion Blur")]
    [SerializeField] private Volume volume; // Volume 컴포넌트
    [SerializeField] private float blurIntensityMax = 1.5f;
    [SerializeField] private float blurAnimTime = 0.2f;

    private MotionBlur _motionBlur;
    private float _baseBlurIntensity;
    private bool _hasBaseBlur;
    private bool _isSpin;

    private Sequence _seq;

    private float _baseFov;
    private bool _hasBaseFov;

    private float _baseMoveSpeed;
    private bool _hasBaseSpeed;

    [Header("attack data"), SerializeField] private AttackDataSO attackData;
    [SerializeField] private StatSO physicalDamageStat;
    [SerializeField] private DamageCaster damageCaster;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private void Awake()
    {
        if (volume != null && volume.profile.TryGet(out _motionBlur))
        {
            _baseBlurIntensity = _motionBlur.intensity.value;
            _hasBaseBlur = true;
        }
    }
    protected override void Update()
    {
        base.Update();
        if(_isSpin)
            HandleDamageCasterTrigger();
    }

    private void HandleDamageCasterTrigger()
    {

        AttackDataSO attackData = this.attackData;
        DamageData damageData = _skillComponent._damageCompo.CalculateDamage(physicalDamageStat, attackData);

        Vector3 position = damageCaster.transform.position;
        bool isSuccess = damageCaster.CastDamage(damageData, position, _owner.transform.forward, attackData);

        if (isSuccess)
        {
            impulseSource.GenerateImpulse(attackData.impulseForce);
        }
    }

    public override void UseSkill()
    {
        base.UseSkill();
        _isSpin = true;

        CharacterMovement movement = _owner.GetCompo<CharacterMovement>();
        if (movement == null)
        {
            Debug.LogWarning("[AniSkill] CharacterMovement not found.");
            return;
        }

        if (!_hasBaseFov)
        {
            _baseFov = GetFov();
            _hasBaseFov = true;
        }

        if (!_hasBaseSpeed)
        {
            _baseMoveSpeed = movement.MoveSpeed;
            _hasBaseSpeed = true;
        }

        KillTweens();
        Restore(movement);

        float shrinkFov = _baseFov - fovShrink;
        float overFov = _baseFov + fovOvershoot;

        Tween FovTween(float to, float time, Ease ease) =>
            DOTween.To(GetFov, SetFov, to, time).SetEase(ease);

        Tween BlurTween(float to, float time, Ease ease)
        {
            if (_motionBlur == null) return null;
            return DOTween.To(() => _motionBlur.intensity.value,
                              x => _motionBlur.intensity.value = x,
                              to, time).SetEase(ease);
        }

        // 타임스케일 감소 (슬로우모션)
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        DOTween.To(() => Time.timeScale, x =>
        {
            Time.timeScale = x;
            Time.fixedDeltaTime = 0.02f * x;

            // 타임스케일 복원에 맞춰 블러도 자연스럽게 줄이기
            if (_motionBlur != null)
                _motionBlur.intensity.value = Mathf.Lerp(blurIntensityMax, _baseBlurIntensity, x);
        }, 1f, 0.4f)
        .SetEase(Ease.OutQuad);

        _seq = DOTween.Sequence();

        // 블러 증가
        _seq.Append(BlurTween(blurIntensityMax, blurAnimTime, Ease.OutSine));

        // FOV 점점 줄이기 (기 모으기)
        _seq.Append(FovTween(shrinkFov, fovAnimTime * 2.5f, Ease.InOutSine));

        // 속도 증가 & VFX 실행
        _seq.AppendCallback(() =>
        {
            movement.MoveSpeedChangeHandle(_baseMoveSpeed * speedMultiplier);
            _owner.GetCompo<EntityVFX>().PlayVfx("AniVFX", Vector3.zero, Quaternion.identity);
        });

        // FOV 팍 증가
        _seq.Append(FovTween(overFov, fovAnimTime * 1.5f, Ease.OutExpo));

        // 유지 시간
        _seq.AppendInterval(duration);

        // FOV 복귀
        _seq.Append(FovTween(_baseFov, fovAnimTime * 1.5f, Ease.InOutSine));

        // 블러 복귀
        _seq.Append(BlurTween(_baseBlurIntensity, blurAnimTime, Ease.InSine));

        _seq.OnComplete(() =>
        {
            _owner.GetCompo<EntityVFX>().StopVfx("AniVFX");
            Restore(movement);
            _seq = null;
            _isSpin = false;
        });
        // _seq.SetUpdate(true); // 필요시 타임스케일 무시
    }

    private float GetFov()
    {
        return vcam != null ? vcam.Lens.FieldOfView : 60f;
    }

    private void SetFov(float value)
    {
        if (vcam == null) return;

        var lens = vcam.Lens;
        lens.FieldOfView = value;
        vcam.Lens = lens;
    }

    private void Restore(CharacterMovement movement)
    {
        if (movement != null && _hasBaseSpeed)
            movement.MoveSpeedChangeHandle(_baseMoveSpeed);

        if (_hasBaseFov)
            SetFov(_baseFov);

        if (_motionBlur != null && _hasBaseBlur)
            _motionBlur.intensity.value = _baseBlurIntensity;
    }

    private void KillTweens()
    {
        if (_seq != null && _seq.IsActive())
        {
            _seq.Kill();
            _seq = null;
        }
    }

    private void OnDisable()
    {
        KillTweens();
        var movement = _owner != null ? _owner.GetCompo<CharacterMovement>() : null;
        Restore(movement);
    }
}
using UnityEngine;
using DG.Tweening;

public class Bomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private float growTime = 0.5f;
    [SerializeField] private float explodeDelay = 1.5f;
    [SerializeField] private Vector3 finalScale = Vector3.one;

    [Header("VFX")]
    [SerializeField] private GameObject explosionVfx;
    [SerializeField] private ParticleSystem jumpVfx;

    private void OnEnable()
    {
        // 초기 스케일 0
        transform.localScale = Vector3.zero;
        jumpVfx.Play();

        // DOTween으로 점점 커짐
        transform.DOScale(finalScale, growTime)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                // 일정 시간 대기 후 폭발
                Invoke(nameof(Explode), explodeDelay);
            });
    }

    private void Explode()
    {
        // 폭발 이펙트 생성
        if (explosionVfx != null)
        {
            Instantiate(explosionVfx, transform.position, Quaternion.identity);
        }

        // 자신 제거
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // 안전하게 DOTween 제거
        transform.DOKill();
        CancelInvoke();
    }
}

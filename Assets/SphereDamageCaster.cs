using UnityEngine;

public class SphereDamageCaster : DamageCaster
{
    [SerializeField, Range(0.5f, 5f)] private float castRadius = 1f;
    [SerializeField, Range(0f, 1f)] private float forwardOffset = 0.5f; // 방향 기준 위치 보정

    public override bool CastDamage(DamageData damageData, Vector3 position, Vector3 direction, AttackDataSO attackData)
    {
        // 회전베기 중심 위치: 플레이어 앞쪽으로 살짝
        Vector3 center = position + direction * forwardOffset;

        Collider[] hits = Physics.OverlapSphere(center, castRadius, whatIsEnemy);

        bool hasHit = false;
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Entity enemy) && !enemy.IsDead)
            {
                Debug.Log(hit.name);
                ApplyDamageAndKnockBack(hit.transform, damageData, hit.transform.position, Vector3.up, attackData);
                hasHit = true;
                Debug.Log(hasHit);
            }
        }

        return hasHit;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector3 center = transform.position + transform.forward * forwardOffset;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, castRadius);
    }
#endif
}
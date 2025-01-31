using UnityEngine;
using System.Linq;

public class PlayerTargeting : MonoBehaviour
{
    public float detectionRadius = 10f;
    public LayerMask enemyLayer;
    private Transform currentTarget;

    private void Update()
    {
        FindNearestTarget();
        RotateTowardsTarget();
    }

    private void FindNearestTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            // Encontra o inimigo mais próximo
            currentTarget = enemies
                .Select(e => e.transform)
                .OrderBy(t => Vector3.Distance(transform.position, t.position))
                .FirstOrDefault();
        }
        else
        {
            currentTarget = null;
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget != null)
        {
            Vector3 directionToTarget = (currentTarget.position - transform.position).normalized;
            directionToTarget.y = 0; // Mantém a rotação apenas no eixo Y

            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        else
        {
            // Se não houver alvo, o Player fica virado para a direita
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 90, 0), Time.deltaTime * 5f);
        }
    }
}

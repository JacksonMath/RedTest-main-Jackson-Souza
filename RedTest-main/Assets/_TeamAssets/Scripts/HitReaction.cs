using UnityEngine;

public class HitReaction : MonoBehaviour
{
    private Animator _animator;
    private void Awake()
    {
        _animator = GetComponent<Animator>();

    }

    private void OnTriggerEnter(Collider player)
    {
        Debug.Log("Hit Reaction");

        int randomTrigger = Random.Range(0, 2);

        if (player.CompareTag("Player"))
        {
            if (randomTrigger == 0)
            {
                _animator.SetTrigger("Hit_01");
            }
            else 
            {
                _animator.SetTrigger("Hit_02");
            }
        }
    }
}

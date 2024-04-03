// Ignore Spelling: DMG

using UnityEngine;
using static GameManager;

public class Harvey : MonoBehaviour, IDamage, IAlive
{
    public float DMG { get; set; }
    public float Health { get; set; }

    bool attackCooldown = false;
    RefreshCooldown RefreshAttack => () => attackCooldown = false;

    float detectDisctance = 50f;
    float attackRange = 15f;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    public void TakeDMG(IDamage DMGSource)
    {
        if (DMGSource == null) return;

        if (Health - DMGSource.DMG <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void DealDMG(IAlive DMGTarget)
    {
        attackCooldown = true;

        // Deal direct damage, as target is known
        DMGTarget.TakeDMG(from: this);
    }
}

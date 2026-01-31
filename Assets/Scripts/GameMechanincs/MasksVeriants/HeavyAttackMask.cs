using TarodevController;
using UnityEngine;

[CreateAssetMenu(menuName = "Masks/HeavyAttack Mask")]
public class HeavyAttackMask : Mask
{

    public override void Activate(GameObject owner)
    {
        CombatController combatController = owner.GetComponent<CombatController>();
        combatController._HeavyAttack = true;
    }

    public override void Deactivate(GameObject owner)
    {
        CombatController combatController = owner.GetComponent<CombatController>();
        combatController._HeavyAttack = false;
    }
}
 
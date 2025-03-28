using UnityEngine;

public abstract class SpellBehaviourBase : MonoBehaviour
{

    public virtual void Initialize() { }

    public virtual void Initialize(Transform target) { }

    public virtual void AdvanceSpell(int value) { }

    public virtual void ActivateSpell() { }

    public virtual void OnExpire() { }

    public virtual void StopCast() { }
}

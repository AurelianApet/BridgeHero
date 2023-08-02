using UnityEngine;
using System.Collections;

public class IBridgesCharacterController : MonoBehaviour{

    public bool AnyBoxSideCounts = true;

    public bool UseBridgeAsFinalPoint = true;

    public virtual void Reset() { }

    public virtual void OnStartMove() { }

    public virtual void OnStartFall() { }

    public virtual void OnEndMove() { }

    public virtual void OnSuccessLanding(bool HasExtraScore, GameObject pillar) { }

    public virtual void OnStartShift() { }

    public virtual void OnEndShift() { }

    public virtual void ModifyANewBlock(GameObject aNewObject) { }

    public void OnTriggerEnter(Collider other)
    {
        Bonus bonus = other.gameObject.GetComponent<Bonus>();
        if (bonus != null) { bonus.Activate(); }

    }

}

using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(VirtualHandHolder))]
public abstract class CardPlayAI : MonoBehaviour
{
    protected VirtualHandHolder virtualHandHolder;
    public virtual void SetVirtualHandHolder(VirtualHandHolder virtualHandHolder)
    {
        this.virtualHandHolder = virtualHandHolder;
    }
    public abstract List<Card> GetCardPlayingAI();
}

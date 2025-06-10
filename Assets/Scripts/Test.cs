using UnityEngine;

public class Test : MonoBehaviour
{
    // public Card card;
    // public bool flip = false;
    // public void FlipCard()
    // {
    //     flip = !flip;
    //     if (flip)
    //     {
    //         _ = card.FaceCardDown(true);
    //     }
    //     else
    //     {
    //         _ = card.FaceCardUp(true);
    //     }
    // }

    public ParticleSystem ps;
    public void Stop()
    {
        ps.Stop();
    }
    
}

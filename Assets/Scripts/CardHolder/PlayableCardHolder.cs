using UnityEngine;

public abstract class PlayableCardHolder : CardHolder, IHelpPlayingCard
{
    // Used to assign chosen card list in RuleGameHandle.
    [SerializeField] protected ChosenCardEventSO chosenCardEventSO;
    public abstract bool HelpPlayingCard();
}

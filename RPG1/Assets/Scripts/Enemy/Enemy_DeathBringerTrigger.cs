using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_DeathBringerTrigger : Enemy_AnimationTriggers
{
    private Enemy_DeathBringer deathBringer => GetComponentInParent<Enemy_DeathBringer>();

    private void RelocateTrigger() => deathBringer.FindPosition();

    private void MakeInvicible() => deathBringer.fx.MakeTransparent(true);
    private void MakeVicible() => deathBringer.fx.MakeTransparent(false);
}

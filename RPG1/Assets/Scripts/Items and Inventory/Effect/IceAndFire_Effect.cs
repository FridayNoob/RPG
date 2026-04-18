using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ice and fire effect", menuName = "Data/ Item effect/Ice and Fire")]
public class IceAndFire_Effect : ItemEffect
{
    [SerializeField] private GameObject iceAndFirePrefab;
    [SerializeField] private float xVelocity;

    public override void ExecuteEffect(Transform _respondPosition)
    {
        Player player = PlayerManager.instance.player;

        //Debug.Log("Á¬»÷Êý£º" + player.primaryAttackState.comboCounter);
        bool thridAttack =  player.primaryAttackState.comboCounter == 2;

        if(thridAttack)
        {
            GameObject newIceAndFire = Instantiate(iceAndFirePrefab, _respondPosition.position, player.transform.rotation);
            newIceAndFire.GetComponent<Rigidbody2D>().velocity = new Vector2(xVelocity * player.facingDir, 0);

            Destroy(newIceAndFire, 10);
        }
    }
}

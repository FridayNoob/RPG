using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole_Skill_Controller : MonoBehaviour
{

    private int cloneAttackAmount = 4;
    private float cloneAttackCooldown = .3f;
    private float maxSize;
    private float growSpeed;
    private float shrinkSpeed;
    private float cloneAttackTimer;
    private float blackholeTimer;

    [SerializeField] private GameObject hotKeyPrefab;
    [SerializeField] private List<KeyCode> keyCodeList;

    private List<Transform> targets = new List<Transform>();
    private List<GameObject> createdHotKey = new List<GameObject>();

    private bool cloneAttackReleased;
    private bool canCreateHotKey = true;
    private bool canGrow = true;
    private bool canShrink;
    private bool playerCanDisappear = true;
    public bool playerCanExitState { get; private set; }




    public void SetupBlackhole(float _maxSize, float _growSpeed, float _shrinkSpeed, int _cloneAttackAmout, float _cloneAttackCooldown, float _blackholeDuration)
    {
        maxSize = _maxSize;
        growSpeed = _growSpeed;
        shrinkSpeed = _shrinkSpeed;
        cloneAttackAmount = _cloneAttackAmout;
        cloneAttackCooldown = _cloneAttackCooldown;
        blackholeTimer = _blackholeDuration;

        if (SkillManager.instance.clone.crystalInsteadOfClone)
            playerCanDisappear = false;
    }

    private void Update()
    {
        cloneAttackTimer -= Time.deltaTime;
        blackholeTimer -= Time.deltaTime;

        if(blackholeTimer < 0)
        {
            blackholeTimer = Mathf.Infinity;
            if (targets.Count > 0)
                ReleaseCloneAttack();
            else
                FInishBlackholeAbility();
        }

        CloneAttackLogic();

        if (canGrow && !canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(maxSize, maxSize), growSpeed * Time.deltaTime);
        }

        if (canShrink)
        {
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(-1, -1), shrinkSpeed * Time.deltaTime);

            if (transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    private void CloneAttackLogic()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseCloneAttack();
        }

        if (cloneAttackTimer < 0 && cloneAttackReleased)
        {
            cloneAttackTimer = cloneAttackCooldown;

            int randomIndex = Random.Range(0, targets.Count);

            float xOffset;
            if (Random.Range(0, 100) > 50)
                xOffset = 1f;
            else
                xOffset = -1f;
            if (SkillManager.instance.clone.crystalInsteadOfClone)
            {
                SkillManager.instance.crystal.CreateCrystal();
                SkillManager.instance.crystal.CurrentCrystalChooseRandomEnemy();
            }
            else
            {
                SkillManager.instance.clone.CreateClone(targets[randomIndex], new Vector3(xOffset, 0));
            }
            cloneAttackAmount--;

            if (cloneAttackAmount <= 0)
            {
                Invoke("FInishBlackholeAbility", 1);
            }

        }
    }

    private void FInishBlackholeAbility()
    {
        DestroyHotKeys();
        cloneAttackReleased = false;
        canShrink = true;
        //PlayerManager.instance.player.ExitBlackholeAbility();
        playerCanExitState = true;
    }

    private void ReleaseCloneAttack()
    {
        if(targets.Count <= 0)
        {
            return;
        }
        if (playerCanDisappear)
        {
            PlayerManager.instance.player.fx.MakeTransparent(true);
            playerCanDisappear = false;
        }
        canCreateHotKey = false;
        cloneAttackReleased = true;
        DestroyHotKeys();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Enemy>() != null)
        {
            collision.GetComponent<Enemy>().FreezeTime(true);
            CreateHotKey(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Enemy>() != null)
            collision.GetComponent<Enemy>().FreezeTime(false);
    }

    private void CreateHotKey(Collider2D collision)
    {
     
        if(keyCodeList.Count <= 0)
        {
            Debug.LogWarning("CodeListÖÐÖµÌ«ÉÙ");
            return;
        }

        if (!canCreateHotKey)
            return;
        
        GameObject newHotKey = Instantiate(hotKeyPrefab, collision.transform.position + new Vector3(0, 2), Quaternion.identity);
        createdHotKey.Add(newHotKey);

        KeyCode choosenKey = keyCodeList[Random.Range(0, keyCodeList.Count)];
        keyCodeList.Remove(choosenKey);

        Blackhole_HotKey_Controller newBlackholeHotKeyScript = newHotKey.GetComponent<Blackhole_HotKey_Controller>();
        newBlackholeHotKeyScript.SetupHotKey(choosenKey, collision.transform, this);
    }

    public void AddEnemyToList(Transform _enemyTransform) => targets.Add(_enemyTransform);

    private void DestroyHotKeys()
    {
        if (createdHotKey.Count <= 0)
            return;
        for (int i = 0; i < createdHotKey.Count; i++)
        {
            Destroy(createdHotKey[i]);
        }
    }


}

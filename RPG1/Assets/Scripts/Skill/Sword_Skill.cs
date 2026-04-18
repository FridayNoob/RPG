using UnityEngine;
using UnityEngine.UI;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}


public class Sword_Skill : Skill
{
    [Header("Spin info")]
    [SerializeField] private UI_SkillTreeSlot swordSpinUnlockButton;
    [SerializeField] private float maxTravelDistance = 7;
    [SerializeField] private float spinDuration = 2;
    [SerializeField] private float spinGravity = 1;

    [Header("Pierce info")]
    [SerializeField] private UI_SkillTreeSlot swordPierceUnlockButton;
    [SerializeField] private int pierceAmount;
    [SerializeField] private float pierceGravity;

    public SwordType swordType = SwordType.Regular;

    [Header("Bounce info")]
    [SerializeField] private UI_SkillTreeSlot swordBounceUnlcokButton;
    [SerializeField] private int bounceAmount;
    [SerializeField] private float bounceGravity;
    [SerializeField] private float bounceSpeed;


    [Header("Aim dots")]
    [SerializeField] private int numberOfDots;
    [SerializeField] private float spaceBetweenDots;
    [SerializeField] private GameObject dotsPrefab;
    [SerializeField] private Transform dotsParent;

    private GameObject[] dots;

    protected Vector2 finalDir;
    [Header("Skill info")]
    [SerializeField] private UI_SkillTreeSlot swordUnlockButton;
    public bool swordUnlocked { get; private set; }
    [SerializeField] private GameObject swordPrefab;
    [SerializeField] private Vector2 launchForce;
    [SerializeField] private float swordGravity;
    [SerializeField] private float freezeTimeDuration;
    [SerializeField] private float returnSpeed;

    [Header("Passive skills")]
    [SerializeField] private UI_SkillTreeSlot swordTimeStopUnlockButton;
    public bool swordTImeStopUnlocked { get; private set; }
    [SerializeField] private UI_SkillTreeSlot swordVulnerableUnlockButton;
    public bool swordVulnerableUnlocked { get; private set; }

    protected override void Start()
    {
        base.Start();
        GenerateDots();
        SetupGravity();

        swordUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSword);
        swordTimeStopUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordTimeStop);
        swordVulnerableUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordVulnerable);
        swordSpinUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordSpin);
        swordBounceUnlcokButton.GetComponent<Button>().onClick.AddListener(UnlockSwordBounce);
        swordPierceUnlockButton.GetComponent<Button>().onClick.AddListener(UnlockSwordPierce);
    }

    #region Unlock

    protected override void CheckUnlock()
    {
        UnlockSword();
        UnlockSwordSpin();
        UnlockSwordBounce();
        UnlockSwordPierce();
        UnlockSwordTimeStop();
        UnlockSwordVulnerable();
    }
    private void UnlockSword()
    {
        if (swordUnlockButton.unlocked)
            swordUnlocked = true;
    }

    private void UnlockSwordTimeStop()
    {
        if (swordTimeStopUnlockButton.unlocked)
            swordTImeStopUnlocked = true;
    }

    private void UnlockSwordVulnerable()
    {
        if (swordVulnerableUnlockButton.unlocked)
            swordVulnerableUnlocked = true;
    }

    private void UnlockSwordSpin()
    {
        if (swordSpinUnlockButton.unlocked)
        {
            swordType = SwordType.Spin;
        }
    }

    private void UnlockSwordPierce()
    {
        if (swordPierceUnlockButton.unlocked)
            swordType = SwordType.Pierce;
    }

    private void UnlockSwordBounce()
    {
        if (swordBounceUnlcokButton.unlocked)
            swordType = SwordType.Bounce;
    }

    #endregion

    private void SetupGravity()
    {
        if (swordType == SwordType.Bounce)
            swordGravity = bounceGravity;
        else if (swordType == SwordType.Pierce)
            swordGravity = pierceGravity;
        else if (swordType == SwordType.Spin)
            swordGravity = spinGravity;
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKey(KeyCode.Mouse1))
        {
            for (int i = 0; i < numberOfDots; i++)
            {
                dots[i].transform.position = DotsPostion(i * spaceBetweenDots);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
            finalDir = new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y);

    }

    public void CreateSword()
    {
        GameObject newSword = Instantiate(swordPrefab, player.transform.position, player.transform.rotation);
        Sword_Skill_Controller newSwordScript = newSword.GetComponent<Sword_Skill_Controller>();

        if (swordType == SwordType.Bounce)
            newSwordScript.SetupBounce(true, bounceAmount, bounceSpeed);
        else if (swordType == SwordType.Pierce)
            newSwordScript.SetupPierce(pierceAmount);
        else if (swordType == SwordType.Spin)
            newSwordScript.SetupSpin(true, maxTravelDistance, spinDuration);

        newSwordScript.SetupSword(finalDir, swordGravity, player, freezeTimeDuration, returnSpeed);

        player.AssignNewSword(newSword);

        DotsActive(false);
    }

    #region Aim Region
    public Vector2 AimDirection()
    {
        Vector2 playerPosition = player.transform.position;
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dirction = mousePosition - playerPosition;

        return dirction;
    }

    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            //if (player== null)
            //    Debug.Log("player为空");
            dots[i] = Instantiate(dotsPrefab, player.transform.position, Quaternion.identity, dotsParent);
            //if (dots[i] == null)
            //    Debug.Log("dots实例化失败！");
            dots[i].SetActive(false);
        }
    }

    public void DotsActive(bool _isActive)
    {
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i].SetActive(_isActive);
        }
    }

    private Vector2 DotsPostion(float t)
    {
        //Vector2 position = (Vector2)player.transform.position + finalDir * t + .5f * (Physics2D.gravity * swordGravity) * (t * t);//不能事实调整方向
        Vector2 position = (Vector2)player.transform.position
            + new Vector2(AimDirection().normalized.x * launchForce.x, AimDirection().normalized.y * launchForce.y) * t
            + .5f * (Physics2D.gravity * swordGravity) * (t * t);
        return position;
    }
    #endregion
}

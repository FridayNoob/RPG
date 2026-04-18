using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour,ISaveManager
{
    [Header("End Screen")]
    [SerializeField] private UI_FadeScreen fadeScreen;
    [SerializeField] private GameObject endText;
    [SerializeField] private GameObject restartButton;
    [Space]

    [SerializeField] private GameObject characterUI;
    [SerializeField] private GameObject skillTreeUI;
    [SerializeField] private GameObject craftUI;
    [SerializeField] private GameObject optionsUI;
    [SerializeField] private GameObject inGameUI;

    public UI_SkillTooltip skillTooltip;
    public UI_ItemTooltip itemTooltip;
    public UI_StatTooltip statTooltip;
    public UI_CraftWindow craftWindow;
    // Start is called before the first frame update

    [SerializeField] private UI_VolumnSlider[] volumnSettings;

    private void Awake()
    {
        if (skillTreeUI == null)
        {
            Debug.Log("null");
            return;
        }
        SwitchTo(skillTreeUI);  //保证技能按钮点击事件中技能UI更新操作先于技能解锁操作（里面有个函数条件中包含UI信息）触发
        fadeScreen.gameObject.SetActive(true);
    }
    void Start()
    {
        SwitchTo(inGameUI);

        itemTooltip.gameObject.SetActive(false);
        statTooltip.gameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            SwitchWithKeyTo(characterUI);
        if (Input.GetKeyDown(KeyCode.B))
            SwitchWithKeyTo(skillTreeUI);
        if (Input.GetKeyDown(KeyCode.K))
            SwitchWithKeyTo(craftUI);
        if (Input.GetKeyDown(KeyCode.O))
            SwitchWithKeyTo(optionsUI);
            

    }
    
    public void SwitchTo(GameObject _menu)
    {
        //关闭所有菜单，打开参数指定菜单

        for(int i = 0; i < transform.childCount; i++)
        {
            //保证DarkScreen不被当初一般菜单界面被关闭
            bool fadeScreen = transform.GetChild(i).GetComponent<UI_FadeScreen>() != null;
            if(!fadeScreen)
                transform.GetChild(i).gameObject.SetActive(false);
        }

        if (_menu != null)
        {
            AudioManager.instance.PlaySFX(11, null);
            _menu.SetActive(true);
        }
        //打开菜单时，游戏会自动暂停
        if(GameManager.instance != null)
        {
            Debug.Log("SwitchTo: " + _menu.name);
            if (_menu == inGameUI)
            {
                //如果打开的是InGameUI界面，就把游戏继续
                GameManager.instance.PauseGame(false);
                //显示提示文本UI
                for(int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).tag == "TipUI")
                    {
                        transform.GetChild(i).gameObject.SetActive(true);
                    }
                }

            }
            else
                GameManager.instance.PauseGame(true);
        }
    }

    public void SwitchWithKeyTo(GameObject _menu)
    {
        //如果参数指定菜单打开，就把它关闭，否则打开参数指定菜单
        if(_menu != null && _menu.activeSelf)
        {
            _menu.SetActive(false);
            CheckForInGame();
            return;
        }
        SwitchTo(_menu);
    }
    private void CheckForInGame()
    {
        //没有打开任何菜单时打开InGameUI
        for(int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf && transform.GetChild(i).GetComponent<UI_FadeScreen>() == null && transform.GetChild(i).tag != "TipUI")
            {
                Debug.Log("CheckForInGame: " + transform.GetChild(i).name);
                return;
            }
        }

        SwitchTo(inGameUI);
    }

    public void SwitchOnEndScreen()
    {
        //死亡后显示效果
        fadeScreen.FadeOut();
        StartCoroutine(EndScreenCoroutine());
    }

    IEnumerator EndScreenCoroutine()
    {
        yield return new WaitForSeconds(1.5f);

        endText.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        restartButton.SetActive(true);
    }

    public void RestartGameButton()
    {
        GameManager.instance.RestartGame();
    }

    public void LoadData(GameData _gameData)
    {
        foreach(KeyValuePair<string, float> pair in _gameData.volumnSettings)
        {
            foreach(UI_VolumnSlider slider in volumnSettings)
            {
                if (pair.Key == slider.parameter)
                    slider.LoadSlider(pair.Value);
            }
        }
    }

    public void SaveData(ref GameData _gameData)
    {
        //把Option界面的声音设置保存到GameData对象
        _gameData.volumnSettings.Clear();

        foreach(UI_VolumnSlider item in volumnSettings)
        {
            _gameData.volumnSettings.Add(item.parameter, item.slider.value);
        }
    }
}

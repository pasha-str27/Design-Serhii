using DG.Tweening;
using UnityEngine;


public class GameStartController : MonoBehaviour
{
    void Awake()
    {
        // app config
        Application.targetFrameRate = 60;

        // load match3 data
        ServerDataTable.Instance.LoadTableFromLocalFile();

        // init dotween
        DOTween.Init(false, false, LogBehaviour.Default);
        DOTween.SetTweensCapacity(500, 150);

        // load playerdata
        Model.Instance.Load();

        // init match3 mapdata
        MapData.main = new MapData(PlayerData.current.match3Data.level);

        Input.multiTouchEnabled = false;

        // init facebook
        // FB.Init();

        CustomLocalization.Load();

#if UNITY_ANDROID

        LanguageType language = DateTimeUtility.GetLanguageType();
        if (language == LanguageType.VIETNAMESE)
        {
            AppTempData.watch_ads_reward_3moves_limit = 1;
        }
        CustomLocalization.SetLanguage(language);
#endif
    }

    void Start()
    {
        // init mobile notification
#if UNITY_ANDROID
        GameNotificationManager.Instance.Initialize();
#endif 

        Popup.PopupSystem.Instance.PlaySFXOnShowEvent += (type) =>
        {
            if (type != PopupType.PopupGameComplete
            && type != PopupType.PopupGameLose
            && type != PopupType.PopupGameMessageStart
            && type != PopupType.PopupGameWin
            && type != PopupType.PopupReward
            && type != PopupType.PopupLiteTutorial
            && type != PopupType.PopupPiggyBankCollect)
                AudioManager.Instance.PlaySFX(AudioClipId.PopupOpen);
        };

        Popup.PopupSystem.Instance.PlaySFXOnCloseEvent += (type) =>
        {
            if (type != PopupType.PopupGameComplete
            && type != PopupType.PopupGameLose
            && type != PopupType.PopupGameMessageStart
            && type != PopupType.PopupGameWin
            && type != PopupType.PopupReward
            && type != PopupType.PopupLiteTutorial
            && type != PopupType.PopupPiggyBankCollect)
                AudioManager.Instance.PlaySFX(AudioClipId.PopupClose);
        };


        bool musicEnabled = PlayerPrefs.GetInt("music_enabled", 1) == 1 ? true : false;
        bool sfxEnabled = PlayerPrefs.GetInt("sfx_enabled", 1) == 1 ? true : false;
        AudioSettingInterface.SetMusicEnabled(musicEnabled);
        AudioSettingInterface.SetSFXEnabled(sfxEnabled);

        AudioManager.Instance.PlayMusic(AudioClipId.DecorMusic);

        if (QuestUtility.NeedToResetCurrentDate())
        {
            QuestUtility.ResetCurrentDate();
            QuestUtility.BuildQuestList();
        }

        QuestUtility.UpdateQuestList();      
    }
    
    public void Play()
    {
        LoadSceneUtility.LoadScene(LoadSceneUtility.HomeDesignSceneName);
    }

    public void OpenPolicy()
    {
        Application.OpenURL("https://polarisgamestudio.github.io/policy.html");
    }
    public void LoginFb()
    {
        PopupUtility.OpenPopupLiteMesage("Coming soon");
    }


}

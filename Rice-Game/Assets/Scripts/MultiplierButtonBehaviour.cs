using UnityEngine;
using System.Collections;

public class MultiplierButtonBehaviour : MonoBehaviour
{
    public GameObject Play;
    public GameObject FastForward;
    public GameObject SuperFastForward;

    public enum TypeMultiplierButton { PLAY, FAST_FORWARD, SUPER_FAST_FORWARD };
    public TypeMultiplierButton Type;
    private TimeManager m_timeManager;

    public void Start()
    {
        m_timeManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<TimeManager>();
    }
    public void clicked()
    {
        //GameSaveDataManager.Save();
        //return;
        if (!m_timeManager.isOnPause()) {
            switch (Type) {
                case TypeMultiplierButton.PLAY:
                    Play.SetActive(false);
                    FastForward.SetActive(true);
                    SuperFastForward.SetActive(false);
                    break;
                case TypeMultiplierButton.FAST_FORWARD:
                    Play.SetActive(false);
                    FastForward.SetActive(false);
                    SuperFastForward.SetActive(true);
                    break;
                case TypeMultiplierButton.SUPER_FAST_FORWARD:
                    Play.SetActive(true);
                    FastForward.SetActive(false);
                    SuperFastForward.SetActive(false);
                    break;
            }
            m_timeManager.nextMode();
        }
        else {
            m_timeManager.switchPauseTime();
        }
    }
}



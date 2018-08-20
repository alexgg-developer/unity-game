using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CalendarButtonBehaviour : MonoBehaviour
{
    private Image m_calendarImage;
    private const string PATH_CALENDAR = "Textures/UI/calendar_phase/calendar";
    private const string PATH_CALENDAR_ALERT = "Textures/UI/calendar_phase/calendaralert";
    public GameObject CalendarPanel;
	private TutorialManager _tutMan;
    public void Start()
    {
		_tutMan = GameObject.FindGameObjectWithTag ("Tutorial").GetComponent<TutorialManager> ();
        m_calendarImage = this.gameObject.GetComponent<Image>();
        GameObject.FindGameObjectWithTag("Logic").GetComponent<PhaseManager>().addListenerToPhaseChange(changeToAlert);
    }

    public void changeToAlert()
    {
        if (!CalendarPanel.activeSelf) {
            m_calendarImage.sprite = Resources.Load<Sprite>(PATH_CALENDAR_ALERT);
        }
    }


    public void clicked()
	{
		if (_tutMan.getState () == TutorialManager.STATES.END) {
			CalendarPanel.SetActive (true);
			m_calendarImage.sprite = Resources.Load<Sprite> (PATH_CALENDAR);
		}
    }
}

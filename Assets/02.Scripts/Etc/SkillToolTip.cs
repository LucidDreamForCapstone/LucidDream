using TMPro;
using UnityEngine;

public class SkillTooltip : MonoBehaviour// IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltipPanel;    // 스킬 설명을 표시할 패널
    [SerializeField] private TMP_Text tooltipText;       // 스킬 설명을 표시할 텍스트
    [SerializeField] private string skillDescription;    // 스킬 설명 내용
    private bool isLocked;
    //private EventTrigger eventTrigger;

    // 툴팁 설명 업데이트
    public void SetSkillDescription(string description) {
        skillDescription = description;
    }

    private void Awake() {
        //eventTrigger = GetComponent<EventTrigger>();    
    }

    private void Start() {
        //tooltipPanel.SetActive(false);  // 시작할 때는 설명 패널을 비활성화 
    }

    // 마우스가 스킬 아이콘 위로 들어왔을 때 호출되는 메서드
    /*public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLocked)
        {
            ShowTooltip();
        }
    }

    // 마우스가 스킬 아이콘에서 벗어났을 때 호출되는 메서드
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }
    */
    public void ShowTooltip() {
        if (!isLocked) {
            tooltipPanel.SetActive(true);        // 설명 패널 활성화
            tooltipText.text = skillDescription; // 설명 텍스트 업데이트
        }
    }

    public void HideTooltip() {
        tooltipPanel.SetActive(false);
    }

    public void SetSkillLocked(bool locked) {
        /*
        if (locked)
        {
            if(eventTrigger != null)
            {
                eventTrigger.enabled = false;
            }
        }

        else
        {
            if(eventTrigger != null) 
            { 
                eventTrigger.enabled = true;
            }
        }
        */
        isLocked = locked;
    }
}

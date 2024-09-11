using TMPro;
using UnityEngine;

public class SkillTooltip : MonoBehaviour// IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltipPanel;    // ��ų ������ ǥ���� �г�
    [SerializeField] private TMP_Text tooltipText;       // ��ų ������ ǥ���� �ؽ�Ʈ
    [SerializeField] private string skillDescription;    // ��ų ���� ����
    private bool isLocked;
    //private EventTrigger eventTrigger;

    // ���� ���� ������Ʈ
    public void SetSkillDescription(string description) {
        skillDescription = description;
    }

    private void Awake() {
        //eventTrigger = GetComponent<EventTrigger>();    
    }

    private void Start() {
        //tooltipPanel.SetActive(false);  // ������ ���� ���� �г��� ��Ȱ��ȭ 
    }

    // ���콺�� ��ų ������ ���� ������ �� ȣ��Ǵ� �޼���
    /*public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLocked)
        {
            ShowTooltip();
        }
    }

    // ���콺�� ��ų �����ܿ��� ����� �� ȣ��Ǵ� �޼���
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }
    */
    public void ShowTooltip() {
        if (!isLocked) {
            tooltipPanel.SetActive(true);        // ���� �г� Ȱ��ȭ
            tooltipText.text = skillDescription; // ���� �ؽ�Ʈ ������Ʈ
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

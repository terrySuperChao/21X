using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HoverSecondSkill : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    public GameObject secondSkillPop;
    public GameObject secondSkillName;
    public GameObject secondSkillDesc;

    private Skill _skill;
    public void setSecondSkill(Skill skill) {
        this._skill = skill;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (this.secondSkillPop != null && this._skill != null)
        {   
            this.secondSkillPop.SetActive(true);
            this.secondSkillName.GetComponent<Text>().text = this._skill.name;
            this.secondSkillDesc.GetComponent<Text>().text = this._skill.desc;

            Transform obj1Transform = this.gameObject.transform;
            Transform obj2Transform = this.secondSkillPop.transform.parent.transform;

            Vector3 vector = this.secondSkillPop.transform.position;
            vector.x = obj1Transform.transform.position.x;
            this.secondSkillPop.transform.position = vector;
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (this.secondSkillPop != null) {
            this.secondSkillPop.SetActive(false);
        }
    }
}
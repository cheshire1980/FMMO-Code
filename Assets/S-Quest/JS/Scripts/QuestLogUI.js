import UnityEngine.UI;

var UICanvas : RectTransform;
var Panel : RectTransform;

public class QuestButtonVars
{
    var ButtonObj : Button;
    var ButtonText : Text;
    @HideInInspector
    var ChildQuest : Quest;
}
var QuestButtons : QuestButtonVars[];
@HideInInspector
var CurrentQuest : int = -1;

var QuestInfo : GameObject;
var QuestGiverImage : Image;
var QuestGiverName : Text;
var QuestTitleText : Text;
var QuestDescriptionText : Text;
var QuestAwardText : Text;
var QuestAwardImage : Image;
var AbandonButton : Button;

public enum QuestLogPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3} 
var QuestLogPosition = QuestLogPositions.TopRightCorner & QuestLogPositions.LowerRightCorner & QuestLogPositions.TopLeftCorner & QuestLogPositions.LowerLeftCorner; //Quest default position.

//Drag and drop GUI:
var IsMovable : boolean = true; //Can the player change the position of the inventory GUI in game?
@HideInInspector
var Dragging : boolean = false;
var PanelDragPos : GameObject;

function Awake () 
{
    CloseQuestLog();
    
    SetQuestLogPosition();
}

function SetQuestLogPosition ()
{
    Panel.anchorMax = new Vector2(0.5f,0.5f);
    Panel.anchorMin = new Vector2(0.5f,0.5f);
    Panel.pivot = new Vector2(0f,0f);
    
    var OffsetY : float;
    var OffsetX : float;
    
    OffsetY = UICanvas.rect.height/2-Panel.rect.height;
    OffsetX = UICanvas.rect.width/2-Panel.rect.width;

    switch(QuestLogPosition)
    {
        case 0: //Top Right corner
        Panel.localPosition = new Vector3(OffsetX,OffsetY, 0);
        break;
        
        case 1: //Lower right corner
        Panel.localPosition = new Vector3(OffsetX,-(UICanvas.rect.height/2), 0);
        break;
        
        case 2: //Top Left corner
        Panel.localPosition = new Vector3(-(UICanvas.rect.width/2),OffsetY, 0);
        break;
        
        case 3: //Lower Left corner
        Panel.localPosition = new Vector3(-(UICanvas.rect.width/2),-(UICanvas.rect.height/2), 0);
        break; 
    }
}

public function OpenQuestLog ()
{
    LoadActiveQuests();
    
    Panel.gameObject.SetActive(true);
}

public function LoadActiveQuests ()
{
    CurrentQuest = -1; QuestInfo.gameObject.SetActive(false);
    
    var ActiveQuests : int = 0;
    var Quests : Quest[] = FindObjectsOfType(Quest) as Quest[];
    for(var TempQuest : Quest in Quests) 
	{
        if(TempQuest.QuestActive == true) //Check if the current quest is active.
        {
            QuestButtons[ActiveQuests].ChildQuest = TempQuest;
            QuestButtons[ActiveQuests].ButtonText.text = TempQuest.QuestTitle;
            ActiveQuests++;
        }
	}

	for(var i : int = 0; i < QuestButtons.Length; i++)
	{
	    if(i >= ActiveQuests)
	    {
	        QuestButtons[i].ButtonObj.gameObject.SetActive(false);
	    }
	    else
	    {
	        QuestButtons[i].ButtonObj.gameObject.SetActive(true);
	    }
	}
}

public function ShowQuestInfo (ID : int)
{
    QuestInfo.gameObject.SetActive(true);
    
    //Quest giver name:
    QuestGiverName.text = QuestButtons[ID].ChildQuest.QuestGiverName;
    //Quest giver image:
    if(QuestButtons[ID].ChildQuest.QuestGiverSprite != null)
    {
        QuestGiverImage.sprite = QuestButtons[ID].ChildQuest.QuestGiverSprite;
    }
    else
    {
        QuestGiverImage.gameObject.SetActive(false);
    }
    
    //Quest title:
    if(QuestButtons[ID].ChildQuest.Meeting == true)
    {
        QuestTitleText.text = QuestButtons[ID].ChildQuest.Sender.QuestTitle;
    }
    else
    {
        QuestTitleText.text = QuestButtons[ID].ChildQuest.QuestTitle;
    }
    
    //Quest description:
    QuestDescriptionText.text = QuestButtons[ID].ChildQuest.GetQuestString();
    
    SetQuestReward(ID);
    
    CurrentQuest = ID;
}

public function SetQuestReward (ID : int)
{
    //Quest reward:
    var GiveXP : int = QuestButtons[ID].ChildQuest.XP;
    if(QuestButtons[ID].ChildQuest.Meeting == true)
    {
       GiveXP = QuestButtons[ID].ChildQuest.Sender.XP;
    }
    
    if(QuestButtons[ID].ChildQuest.RewardPlayer == true && QuestButtons[ID].ChildQuest.QuestFinished == false)
    {
        QuestAwardText.text = "You will receive: \n" + GiveXP.ToString() + " XP Points.\n \n";
    }
    else
    {
        QuestAwardText.gameObject.SetActive(false);
    }
    
   if(QuestAwardImage)  QuestAwardImage.gameObject.SetActive(false);
    
    /*var ItemToGive : GameObject = QuestButtons[ID].ChildQuest.Item;
    
    if(QuestButtons[ID].ChildQuest.Meeting == true)
    {
        ItemToGive = QuestButtons[ID].ChildQuest.Sender.Item;
    }
    
    if(ItemToGive != null && QuestButtons[ID].ChildQuest.RewardPlayer == true && QuestButtons[ID].ChildQuest.QuestFinished == false)
    {
        var ItemScript : Item = ItemToGive.gameObject.GetComponent("Item");
        QuestAwardText.text = ItemScript.Name+"("+ItemScript.Amount.ToString()+") - "+GiveXP.ToString() + " XP Points.\n";
        QuestAwardImage.sprite = ItemScript.Icon;
        QuestAwardImage.gameObject.SetActive(true);
    }*/
}

public function RemoveQuest()
{
    QuestButtons[CurrentQuest].ChildQuest.AbandonQuest ();
    LoadActiveQuests();
}

public function CloseQuestLog ()
{
    Panel.gameObject.SetActive(false);
}

function Update () 
{
    if(Dragging == true)
    {
        var TempPos : Vector3 = Input.mousePosition - UICanvas.localPosition;
        PanelDragPos.GetComponent(RectTransform).localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent(RectTransform).rect.width/2,TempPos.y-PanelDragPos.GetComponent(RectTransform).rect.height/2,0);
    }
}

//Dragging and dropping the quest log window:
public function DragStarted ()
{
    if(IsMovable == true && Dragging == false)
    {
        Dragging = true;
        PanelDragPos.gameObject.SetActive(true);
        var TempPos : Vector3 = Input.mousePosition - UICanvas.localPosition;
        PanelDragPos.GetComponent(RectTransform).localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent(RectTransform).rect.width/2,TempPos.y-PanelDragPos.GetComponent(RectTransform).rect.height/2,0);
        Panel.gameObject.transform.SetParent(PanelDragPos.transform, true);
    }
}

public function DragEnded ()
{
    if(IsMovable == true)
    {
        Dragging = false;
        PanelDragPos.gameObject.SetActive(false);
        Panel.gameObject.transform.SetParent(UICanvas.transform, true);
    }
}
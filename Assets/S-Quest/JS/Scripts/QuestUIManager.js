//Quest UI:
var UICanvas : RectTransform;
var Panel : RectTransform;
var QuestGiverImage : Image;
var QuestGiverName : Text;
var QuestTitleText : Text;
var QuestDescriptionText : Text;
var QuestAwardText : Text;
var QuestAwardImage : Image;
var AcceptButton : Button;
var CloseButton : Button;
var AbandonButton : Button;
var FinishButton : Button;

/*//Visual and classic 2 theme variables:
var EliminationIcon : Sprite;
var CollectionIcon : Sprite;
var MeetingIcon : Sprite;
var GotoIcon : Sprite;*/

public enum QuestPositions {TopRightCorner = 0, LowerRightCorner = 1, TopLeftCorner = 2, LowerLeftCorner = 3} 
var QuestPosition = QuestPositions.TopRightCorner & QuestPositions.LowerRightCorner & QuestPositions.TopLeftCorner & QuestPositions.LowerLeftCorner; //Quest default position.

//Drag and drop GUI:
var IsMovable : boolean = true; //Can the player change the position of the inventory GUI in game?
@HideInInspector
var Dragging : boolean = false;
var PanelDragPos : GameObject;

@HideInInspector
var ActiveQuest : Quest; //The current active quest.

function Awake () 
{
    CloseQuest();
    
    SetQuestPosition();
}

function SetQuestPosition ()
{
    Panel.anchorMax = new Vector2(0.5f,0.5f);
    Panel.anchorMin = new Vector2(0.5f,0.5f);
    Panel.pivot = new Vector2(0f,0f);
    
    var OffsetY : float;
    var OffsetX : float;
    
    OffsetY = UICanvas.rect.height/2-Panel.rect.height;
    OffsetX = UICanvas.rect.width/2-Panel.rect.width;

    switch(QuestPosition)
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

public function OpenQuest ()
{
    Panel.gameObject.SetActive(true);
    
    if(ActiveQuest.Meeting == true || ActiveQuest.ReturningToGiver == true)
    {
        FinishButton.gameObject.SetActive(true);
        AcceptButton.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(false);
        AbandonButton.gameObject.SetActive(false);
    }
    //If the quest is already finished.
    else if(ActiveQuest.QuestFinished == true)
    {
        FinishButton.gameObject.SetActive(false);
        AcceptButton.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(true);
        AbandonButton.gameObject.SetActive(false);
    }
    else if(ActiveQuest.QuestActive == false) //If the quest is still active.
    {
        FinishButton.gameObject.SetActive(false);
        AcceptButton.gameObject.SetActive(true);
        CloseButton.gameObject.SetActive(true);
        AbandonButton.gameObject.SetActive(false);
    }  
    else if(ActiveQuest.QuestActive == true) //If the quest is inactive.
    {
        FinishButton.gameObject.SetActive(false);
        AcceptButton.gameObject.SetActive(false);
        CloseButton.gameObject.SetActive(true);
        AbandonButton.gameObject.SetActive(true);
    } 
    //Quest giver name:
    QuestGiverName.text = ActiveQuest.QuestGiverName;
    //Quest giver image:
    if(ActiveQuest.QuestGiverSprite != null)
    {
        QuestGiverImage.sprite = ActiveQuest.QuestGiverSprite;
    }
    else
    {
        QuestGiverImage.gameObject.SetActive(false);
    }
    
    //Quest title:
    if(ActiveQuest.Meeting == true)
    {
        QuestTitleText.text = ActiveQuest.Sender.QuestTitle;
    }
    else
    {
        QuestTitleText.text = ActiveQuest.QuestTitle;
    }
    
    //Quest description:
    QuestDescriptionText.text = ActiveQuest.GetQuestString();
    
    SetQuestReward();
    
    if(ActiveQuest.Speech) ActiveQuest.gameObject.audio.PlayOneShot(ActiveQuest.Speech);
}

public function SetQuestReward ()
{
    //Quest reward:
    var GiveXP : int = ActiveQuest.XP;
    if(ActiveQuest.Meeting == true)
    {
       GiveXP = ActiveQuest.Sender.XP;
    }
    
    if(ActiveQuest.RewardPlayer == true && ActiveQuest.QuestFinished == false)
    {
        QuestAwardText.text = GiveXP.ToString() + " XP Points.\n";
    }
    else
    {
        QuestAwardText.gameObject.SetActive(false);
    }
    
    if(QuestAwardImage) QuestAwardImage.gameObject.SetActive(false);
    
    /*var ItemToGive : GameObject = ActiveQuest.Item;
    
    if(ActiveQuest.Meeting == true)
    {
        ItemToGive = ActiveQuest.Sender.Item;
    }
    
    if(ItemToGive != null && ActiveQuest.RewardPlayer == true && ActiveQuest.QuestFinished == false)
    {
        var ItemScript : Item = ItemToGive.gameObject.GetComponent("Item");
        QuestAwardText.text = ItemScript.Name+"("+ItemScript.Amount.ToString()+") - "+GiveXP.ToString() + " XP Points.\n";
        QuestAwardImage.sprite = ItemScript.Icon;
        QuestAwardImage.gameObject.SetActive(true);
    }*/
}

public function CloseQuest ()
{
    Panel.gameObject.SetActive(false);
    if(ActiveQuest != null) ActiveQuest.QuestOpen = false;
    ActiveQuest = null;
}

public function FinishQuest ()
{
    if(ActiveQuest.Meeting == true)
    {
        ActiveQuest.FinishMeetingQuest();
    }
    else if(ActiveQuest.ReturningToGiver == true)
    {
        ActiveQuest.FinishQuest();
    }
    CloseQuest ();
}

public function ActivateQuest ()
{
    ActiveQuest.AcceptQuest();
    CloseQuest ();
}

public function RemoveQuest ()
{
    ActiveQuest.AbandonQuest ();
    CloseQuest ();
}

function Update () 
{
    if(Dragging == true)
    {
        var TempPos : Vector3 = Input.mousePosition - UICanvas.localPosition;
        PanelDragPos.GetComponent(RectTransform).localPosition = new Vector3(TempPos.x-PanelDragPos.GetComponent(RectTransform).rect.width/2,TempPos.y-PanelDragPos.GetComponent(RectTransform).rect.height/2,0);
    }
}

//Dragging and dropping the quest window:
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
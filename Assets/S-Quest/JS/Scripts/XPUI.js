import UnityEngine.UI;

var FullBar : RectTransform;
var Empty : RectTransform;
var LevelText : Text;

@HideInInspector
var BarWidth : float = 0;
@HideInInspector
var PosX : float = 0;
@HideInInspector
var XPManager : ExperienceManager;

function SetXPBarUI () 
{
    if(XPManager == null)
    {
        XPManager = FindObjectOfType(ExperienceManager);
    
        BarWidth = FullBar.rect.width;
        PosX = FullBar.localPosition.x;
    }
    FullBar.sizeDelta.x = (XPManager.XP/(XPManager.Level*XPManager.Level1XP)) * BarWidth;
    FullBar.localPosition.x = -(BarWidth-FullBar.rect.width)/2 + PosX;
    
    LevelText.text = "Level: " + XPManager.Level.ToString();
}
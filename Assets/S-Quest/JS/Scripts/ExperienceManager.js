/*

XP and Level system Created by Oussama Bouanani (SoumiDelRio).

*/

//This is just a basic experience and level system, you can expand this however you want.
//I just created this to show you how to reward a player with experience points after completing a quest.

@HideInInspector
var Level : int = 1; //Current player's level.
@HideInInspector
var XP : float = 0.0f; //Current player's experience points.
var MaxLevel : float = 20; //Max level that the player could achieve.
var Level1XP : float = 100; //Level 1 experience points, need this to create all the other levels.

var SaveAndLoad : boolean = true; //Save and load player's level and experience points?

//Sounds:
var LevelUpSound : AudioClip;

@HideInInspector
var ExperienceUI : XPUI;

function Awake () 
{
    XP = 0;
    ExperienceUI = FindObjectOfType(XPUI);
    //Load player's level.
    if(SaveAndLoad == true)
    {
        XP = PlayerPrefs.GetFloat("XP");
        Level = PlayerPrefs.GetInt("Level");
        if(Level == 0)
        {
            Level = 1;
        }
    }
    
    ExperienceUI.SetXPBarUI ();
}

//Please use the fucntion when you want to add experience to the player.
function AddXP (Amount : int)
{
    XP += Amount;
    
    if(Level < MaxLevel) //If the player didn't reach the max level yet.
    {
        if(XP >= Level*Level1XP) //If the player's experience is high or equal to the required experience points to level up.
        {
            //Level up!
            XP -= Level*Level1XP; 
            Level++;
            
            //Play the level up sound.
            if(LevelUpSound) audio.PlayOneShot(LevelUpSound);
        }
    }
    
    //If this is the final level:
    if(Level == MaxLevel)
    {
        //Keep resetting the player's experience points.
        XP = 0;
    }
    
    //Save player's level and xp:
    if(SaveAndLoad == true)
    {
        PlayerPrefs.SetFloat("XP", XP);
        PlayerPrefs.SetInt("Level", Level);
    } 
    
    ExperienceUI.SetXPBarUI();
}
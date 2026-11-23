using UnityEngine;

// Forces the mobile game into horizontal (landscape) orientation.
public sealed class BRLandscapeMode : MonoBehaviour
{
    private void Awake()
    {
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
}

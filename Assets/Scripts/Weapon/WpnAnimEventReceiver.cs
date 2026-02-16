using UnityEngine;

public class WpnAnimEventReceiver : MonoBehaviour
{

    Animator WpnAnimator;

    void Start()
    {
        WpnAnimator = GetComponent<Animator>();
    }

    public void EventIntroEnd()
    {
        WpnAnimator.SetBool("Intro", false);

    }
    
    void Update()
    {
        
    }
}

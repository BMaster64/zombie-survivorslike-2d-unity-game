using UnityEngine;
 
public class XPLevelController : MonoBehaviour
{
    public static XPLevelController instance;

    private void Awake()
    {
        instance = this;
    }

    public int currentExperience;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetExp(int amountToGet)
    {
        currentExperience += amountToGet;
    }
    
}

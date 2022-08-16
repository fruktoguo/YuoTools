using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public bool Des;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Des)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(this);
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
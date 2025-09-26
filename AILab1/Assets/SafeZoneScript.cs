using UnityEngine;

public class SafeZoneScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.tag = "SafeZone";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = Time.timeScale + 1;
            Debug.Log("Speed increased: " + Time.timeScale);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Time.timeScale = Time.timeScale - 1;
            Debug.Log("Speed decreased: " + Time.timeScale);
        }
    }
}

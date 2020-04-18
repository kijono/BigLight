using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(scaleTime > 0){
            scaleTime -= Time.deltaTime;
            if(scaleTime <= 0){
               Time.timeScale = 1f;
            }
        }
    }
    
    private float scaleTime = 0f;
    public void SetTimeScale(float t, float duration){
        scaleTime = duration;
        Time.timeScale = t;
    }

    public void Restart(){
        Time.timeScale = 1f;
        scaleTime = 0f;

        DashShadow.Purge();
        overPanel.SetActive(false);
        winPanel.SetActive(false);
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }
    
    public GameObject overPanel;
    public void GameOver(){
        Time.timeScale = 0f;
        overPanel.SetActive(true);
    }
    
    public GameObject winPanel;
    public void Win(){
        Time.timeScale = 0f;
        winPanel.SetActive(true);
    }
}

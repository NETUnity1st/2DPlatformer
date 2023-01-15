using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int stageIndex;
    public int Bullet;
    public int health;
    public PlayerMove player;
    public GameObject[] Stages;
    public Image[] UIhealth;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;
    public void NextStage()
    {
        if (stageIndex+1 < Stages.Length)
        {
            Stages[stageIndex].SetActive(false);
            Stages[++stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex+1);
        }
        else {
            Time.timeScale = 0;
            Debug.Log("게임클리어!");

            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }
        totalPoint += stagePoint;
        stagePoint = 0;
    }
    public void healthDown()
    {

        health -= 1;
        UIhealth[health].color = new Color(1,0,0,0.4f);
        if (health <= 0)
        {
            player.OnDie();
            Debug.Log("Died");

            RestartBtn.SetActive(true);
        }
    }

    void PlayerReposition()
    {
        player.VelocityZero();
        player.transform.position = new Vector3(-10, 1.5f, 0);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            healthDown();
            if (health > 1)
                PlayerReposition();
        }
    }
    

    // Start is called before the first frame update
    void Start()
    {
        RestartBtn.SetActive(false);
        health = 3;
    }

    // Update is called once per frame
    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();

    }
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}

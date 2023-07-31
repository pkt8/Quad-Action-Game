using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject menuCam;
    public GameObject gameCam;
    public Player player;
    public Boss boss;
    public GameObject itemShop;
    public GameObject weaponShop;
    public GameObject startZone;
    public GameObject safeSpace;
    public GameObject stageSpace;
    public Vector3 limitMin;
    public Vector3 limitMax;

    public int stage;
    public float playTime;
    public bool isBattle;
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    //Menu Panel
    public AudioSource stageClearSound;
    public AudioSource menuBgm;
    public AudioSource safeBgm;
    public AudioSource stageBgm;

    public Transform bossZone;
    public GameObject[] enemies;
    public List<int> enemyList;

    //Menu Panel
    public GameObject menuPanel;
    public GameObject gamePanel;
    public GameObject overPanel;
    public Text maxScoreTxt;
    //Game Panel
    public Text scoreTxt;
    public Text stageTxt;
    public Text playTimeTxt;
    public Text playerHealthTxt;
    public Text playerAmmoTxt;
    public Text playerCoinTxt;
    public Image weapon1Img;
    public Image weapon2Img;
    public Image weapon3Img;
    public Image weaponRImg;
    public Text enemyATxt;
    public Text enemyBTxt;
    public Text enemyCTxt;
    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    public Text curScoreText;
    public Text bestText;

    void Awake()
    {
        enemyList = new List<int>();
        maxScoreTxt.text = string.Format("{0:n0}", PlayerPrefs.GetInt("MaxScore"));
        menuBgm.Play();
        if (PlayerPrefs.HasKey("MaxScore"))
            PlayerPrefs.SetInt("MaxScore", 0);
    }

    public void GameStart()
    {
        menuCam.SetActive(false);
        gameCam.SetActive(true);

        menuPanel.SetActive(false);
        gamePanel.SetActive(true);

        safeSpace.SetActive(true);
        stageSpace.SetActive(false);

        menuBgm.Stop();
        safeBgm.Play();
        

        player.gameObject.SetActive(true);
    }
    
    public void GameOver()
    {
        gamePanel.SetActive(false);
        overPanel.SetActive(true);
        curScoreText.text = scoreTxt.text;

        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if(player.score > maxScore)
        {
            bestText.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore",player.score);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    public void StageStart()
    {
        itemShop.SetActive(false);
        weaponShop.SetActive(false);
        startZone.SetActive(false);
        bossZone.gameObject.SetActive(true);
        safeSpace.SetActive(false);
        stageSpace.SetActive(true);
        isBattle = true;

        safeBgm.Stop();
        stageBgm.Play();
        stage++;
        StartCoroutine(InBattle());
    }

    public void StageEnd()
    {
        player.transform.position = Vector3.back * 25 + Vector3.up * 0.8f;

        itemShop.SetActive(true);
        weaponShop.SetActive(true);
        startZone.SetActive(true);
        bossZone.gameObject.SetActive(false);
        safeSpace.SetActive(true);
        stageSpace.SetActive(false);
        isBattle = false;
        stageClearSound.Play();
        stageBgm.Stop();
        safeBgm.Play();
    }

    IEnumerator InBattle()
    {
        if(stage % 5 == 0)
        {
            enemyCntD++;
            GameObject instantEnemy = Instantiate(enemies[3], bossZone.position, bossZone.rotation);
            Enemy enemy = instantEnemy.GetComponent<Enemy>();
            enemy.target = player.transform;
            enemy.manager = this;
            boss = instantEnemy.GetComponent<Boss>();
        }
        else
        {
            int count = 0;
            for (int index = 0; index < stage+2; index++)
            {
                int ran = Random.Range(0, 3);
                if (ran == 2)
                    count++;
                if (count > 1)
                {
                    ran = Random.Range(0, 2);
                }
                enemyList.Add(ran);

                switch (ran)
                {
                    case 0:
                        enemyCntA++;
                        break;
                    case 1:
                        enemyCntB++;
                        break;
                    case 2:
                        enemyCntC++;
                        break;
                }
            }
            while (enemyList.Count > 0)
            {
                float ranZoneX = Random.Range(limitMin.x, limitMax.x);
                float ranZoneZ = Random.Range(limitMin.z, limitMax.z);
                if (Mathf.Abs(ranZoneX) + Mathf.Abs(ranZoneZ) >= 56)
                    continue;
                Vector3 creatingPoint = new Vector3(ranZoneX, 0, ranZoneZ);
                float dist = Vector3.Distance(player.transform.position, creatingPoint);
                if (dist >= 18.0f)
                {
                    GameObject instantEnemy = Instantiate(enemies[enemyList[0]], creatingPoint, Quaternion.identity);
                    Enemy enemy = instantEnemy.GetComponent<Enemy>();
                    enemy.target = player.transform;
                    enemy.manager = this;
                    enemyList.RemoveAt(0);
                    yield return new WaitForSeconds(4f);
                }
                else continue;
            }
        }
        
        while (enemyCntA+enemyCntB+enemyCntC+enemyCntD > 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(5f);
        boss = null;
        StageEnd();
    }
    
    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
    }

    void LateUpdate()
    {
        //메인 화면 UI
        scoreTxt.text = string.Format("{0:n0}", player.score);
        stageTxt.text = "STAGE " + stage;

        //시간 계산
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour *3600) / 60);
        int second = (int)(playTime % 60);

        //게임 화면 UI
        playTimeTxt.text = string.Format("{0:00}",hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);
        playerHealthTxt.text = player.health +  " / " + player.maxHealth;
        playerCoinTxt.text = string.Format("{0:n0}", player.coin);
        if (player.equipWeapon == null)
            playerAmmoTxt.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmoTxt.text = "- / " + player.ammo;
        else
            playerAmmoTxt.text = player.equipWeapon.curAmmo + " / " + player.ammo;

        //무기 UI
        weapon1Img.color = new Color(1, 1, 1, player.hasWeapons[0] ? 1 : 0);
        weapon2Img.color = new Color(1, 1, 1, player.hasWeapons[1] ? 1 : 0);
        weapon3Img.color = new Color(1, 1, 1, player.hasWeapons[2] ? 1 : 0);
        weaponRImg.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        //몬스터 남은 숫자 UI
        enemyATxt.text = enemyCntA.ToString();
        enemyBTxt.text = enemyCntB.ToString();
        enemyCTxt.text = enemyCntC.ToString();

        //보스 체력 UI
        if (boss != null)
        {
            bossHealthGroup.anchoredPosition = Vector3.down * 30;
            bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
        }
        else
        {
            bossHealthGroup.anchoredPosition = Vector3.up * 200;
        }
            
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(limitMin, limitMax);
    }
}

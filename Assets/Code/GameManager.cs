using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    protected static GameManager Instance;
    public static GameManager GetInstance()
    {
        return Instance;
    }
    //
    public const int NULL_STATE = 0;
    public const int MENU_STATE = 1;
    public const int GAME_STATE = 2;
    public const int LOADING_STATE = 3;
    public const int END_STATE = 4;
    //
    public int State = NULL_STATE;
    public Chessboard CBorad;
    public int CurStage = 0;
    public int MaxStage = 3;

    //UI Relative
    public Transform Mask;
    public Text Stage_Label;
    public Transform LoseUI;
    public Transform WinUI;
    public Button Lose_OK_Btn;
    public Button Lose_Cancel_Btn;
    public Button Win_OK_Btn;
    public Button Win_Cancel_Btn;
    public Button Exit_Btn;
    public Image Grass_Img;
    public Image Fruit_Img;
    public Image Bark_Img;


    //
    public bool LockController;
    //
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }
    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded += OnLoadedGame;
    }
    /// ----------------
    public void StartMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("GameScene");        
    }

    public void OnLoadedGame(Scene scene, LoadSceneMode lsm)
    {
        if (scene.name == "GameScene" && scene.isLoaded)
        {
            var objs = scene.GetRootGameObjects();

            foreach (var obj in objs)
            {
                if (obj.name == "BaseAnchor")
                {
                    CBorad = obj.GetComponent<Chessboard>();
                }

                if (obj.name == "Canvas")
                {
                    //UI
                    Mask = obj.transform.Find("Mask");
                    Stage_Label = Mask.transform.Find("Stage_Label").GetComponent<Text>();
                    LoseUI = obj.transform.Find("LOSE");
                    LoseUI.gameObject.SetActive(false);
                    WinUI = obj.transform.Find("WIN");
                    WinUI.gameObject.SetActive(false);
                    //
                    Lose_OK_Btn = LoseUI.Find("OK").GetComponent<Button>();
                    Lose_OK_Btn.onClick.AddListener(Replay);

                    Lose_Cancel_Btn = LoseUI.Find("CANCEL").GetComponent<Button>();
                    Lose_Cancel_Btn.onClick.AddListener(BackToMenu);

                    Win_OK_Btn = WinUI.Find("OK").GetComponent<Button>();
                    Win_OK_Btn.onClick.AddListener(PlayNext);

                    Win_Cancel_Btn = WinUI.Find("CANCEL").GetComponent<Button>();
                    Win_Cancel_Btn.onClick.AddListener(BackToMenu);
                    //-
                    var GameUI = obj.transform.Find("GAME_UI");
                    Grass_Img = GameUI.Find("ITEM_1/ITEM_1_CD").GetComponent<Image>();
                    Fruit_Img = GameUI.Find("ITEM_2/ITEM_2_CD").GetComponent<Image>();
                    Bark_Img = GameUI.Find("ITEM_3/ITEM_3_CD").GetComponent<Image>();

                }
            }
            SetState(GAME_STATE);
        }

        if (scene.name == "MenuScene" && scene.isLoaded)
        {
            var objs = scene.GetRootGameObjects();
            
            foreach (var obj in objs)
            {
                if (obj.name == "Canvas")
                {
                   obj.transform.Find("ANYKEY").transform.DOShakePosition(20, new Vector3(10, 10, 0));
                }
            }
            SetState(MENU_STATE);

        }
    }

    public void NextStage()
    {
        if (CurStage < MaxStage)
        {
            CurStage++;
            CBorad.ResetMap();
            CBorad.LoadMap(CurStage);
            //create pawn!
            {
                GameObject go = null;
                var data = CBorad.MapData;
                go = GameObject.Instantiate(Resources.Load("Sheep")) as GameObject;
                var sheep = go.GetComponent<Sheep>();
                CBorad.RegisterPawn(sheep);
                sheep.SetToGrid(data.SheepPos);
                go = GameObject.Instantiate(Resources.Load("Dog")) as GameObject;
                var dog = go.GetComponent<Dog>();
                CBorad.RegisterPawn(dog);
                dog.SetToGrid(data.DogPos);
                for (var i = data.WolfPosAndDir.Length - 1;i>=0;--i)
                {
                    go = GameObject.Instantiate(Resources.Load("Wolf")) as GameObject;
                    var wolf = go.GetComponent<Wolf>();
                    CBorad.RegisterPawn(wolf);
                    wolf.SetToGrid(data.WolfPosAndDir[i].Pos);
                    wolf.WalkDir.Set(data.WolfPosAndDir[i].Dir.x, data.WolfPosAndDir[i].Dir.y);
                }
                
            }
            //UI 
            //clear end UI
            WinUI.gameObject.SetActive(false);
            LoseUI.gameObject.SetActive(false);
            //
            Mask.gameObject.SetActive(true);
            Stage_Label.text = "STAGE\t" + CurStage.ToString();
            LockController = true;
            Invoke("HideMask", 1.2f);
        }
    }

    public void HideMask()
    {
        Mask.gameObject.SetActive(false);
        LockController = false;
    }

    public void BackToMenu()
    {
        StartMenu();
    }

    public void Replay()
    {
        CurStage--;
        SetState(GAME_STATE);
    }

    public void PlayNext()
    {
        SetState(GAME_STATE);
    }


    /// <summary>
    /// 
    /// </summary>

    public void MissionComplete()
    {
        SetState(END_STATE);
        WinUI.gameObject.SetActive(true);
    }

    public void MissionFailed()
    {
        SetState(END_STATE);
        LoseUI.gameObject.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        switch (State)
        {
            case MENU_STATE:
                {
                    if (Input.anyKeyDown)
                    {
                        StartGame();
                    }
                }
                break;
            case GAME_STATE:
                {
                    if (!LockController)
                    {
                        //TODO CONTOALLER
                        //Here for test
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            NextStage();
                        }
                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            MissionComplete();
                        }
                        if (Input.GetKeyDown(KeyCode.DownArrow))
                        {
                            MissionFailed();
                        }
                    }
                }
                break;
            case LOADING_STATE:
                {

                }
                break;
            case END_STATE:
                {

                }
                break;
        }
    }
    //
    public void SetState(int s)
    {
        if (State != s)
        {
            EndState();
            //
            State = s;
            //
            BeginState();
        }
    }

    protected void BeginState()
    {
        switch (State)
        {
            case MENU_STATE:
                {
                    CurStage = 0;
                }
                break;
            case GAME_STATE:
                {
                    NextStage();
                }
                break;
            case LOADING_STATE:
                {

                }
                break;
            case END_STATE:
                {
                    LockController = true;
                }
                break;
        }
    }

    protected void EndState()
    {
        switch (State)
        {
            case MENU_STATE:
                {

                }
                break;
            case GAME_STATE:
                {

                }
                break;
            case LOADING_STATE:
                {

                }
                break;
            case END_STATE:
                {
                    LockController = false;
                }
                break;
        }
    }
}

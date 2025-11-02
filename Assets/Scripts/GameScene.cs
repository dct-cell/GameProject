using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour // Manages scene transitions and initialization
{
	public static GameScene instance { get; private set; }

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	[Header("Scene names")]
	public string battleSceneName = "BattleScene";
    public string mainSceneName = "MainScene";
    public string mapSceneName = "MapScene";
	public string ShopSceneName = "ShopScene";
	public string EventSceneName = "EventScene";

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == mainSceneName)
			InitializeMainScene();
		if (scene.name == battleSceneName)
			InitializeBattleScene();
		if (scene.name == mapSceneName)
			InitializeMapScene();
		if (scene.name == ShopSceneName)
			InitializeShopScene();
		if (scene.name == EventSceneName)
			InitializeEventScene();

	}

	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}


    #region Battle Scene
    public void LoadBattleScene() {
		LoadScene(battleSceneName);
	}
    private void InitializeBattleScene() {
        Debug.Log("Initialize Battle Scene");
        BattleManager.instance.BattleInit();
        StageManager.instance.StageInit();
        GridManager.instance.GridInit();
        StageLoader.instance.StageInit();
    }
    #endregion
    #region Main Scene
    public void LoadMainScene() {
        LoadScene(mainSceneName);
    }
    private void InitializeMainScene()
	{
		Debug.Log("Initialize Main Scene");
	}

    #endregion
    #region Map Scene
    private void InitializeMapScene() {
		Debug.Log("Initialize Map Scene");
		UI_MapSceneManager.instance.DrawMap();
    }

    public void LoadMapScene() {
        LoadScene(mapSceneName);
    }
	#endregion
	#region Shop Scene
	private void InitializeShopScene()
	{
		Debug.Log("Initialize Shop Scene");
		UI_ShopManager.instance.InitShopUI();
		ShopManager.instance.ShopInit();
	}

	public void LoadShopScene()
	{
		LoadScene(ShopSceneName);
	}
	#endregion
	#region Event Scene
	private void InitializeEventScene()
	{
		Debug.Log("Initialize Event Scene");
		EventManager.instance.GenerateEvent();
	}

	public void LoadEventScene()
	{
		LoadScene(EventSceneName);
	}
	#endregion


}

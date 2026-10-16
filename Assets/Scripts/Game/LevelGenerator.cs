using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : Singleton<LevelGenerator>
{
    public FoodPileSettings foodPile;
    private LevelInfo levelData; // ScriptableObject that hold current level data.
    [SerializeField]private List<GameObject> levelObjects; // Objects that will be collectable in this level.
    public List<GameObject> objectsOnScene; // All collectable objects with its count on scene.
    private void Awake()
    {
        if (ReplicaFlow.Instance == null) InitializeLevel(); // Replica launch owns stage selection.
    }
    public void InitializeLevel()
    {
        StopAllCoroutines();
        GameManager.Instance.gameStatus = GameManager.GameStatus.PREPARING;
        if (foodPile != null) { CreateFoodPile(); return; }
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        UIManager.Instance.SetLevelText("Level "+currentLevel);
        if (currentLevel > 10) // Ten authored levels are bundled; continue by replaying a random level.
        {
            currentLevel = Random.Range(1, 11);
        }
        GetLevelData(currentLevel);
    }
    private void CreateFoodPile()
    {
        if (!foodPile.IsValid) { Debug.LogError("Food pile requires eight prefabs and counts divisible by three."); return; }
        UIManager.Instance.SetLevelText("FOOD PILE");
        var random = new System.Random(foodPile.seed);
        var spawnList = new List<GameObject>();
        foreach (var prefab in foodPile.prefabs)
            for (int i = 0; i < foodPile.copiesPerType; i++) spawnList.Add(prefab);
        for (int i = spawnList.Count-1; i > 0; i--) {
            int j = random.Next(i+1); var temp = spawnList[i]; spawnList[i] = spawnList[j]; spawnList[j] = temp;
        }
        for (int i = 0; i < spawnList.Count; i++) {
            var pos = new Vector3((i%3-1f)*.50f, .55f+(i/9)*.54f, ((i/3)%3-1f)*.50f+.15f);
            var rot = Quaternion.Euler((float)random.NextDouble()*30-15, random.Next(360), (float)random.NextDouble()*30-15);
            objectsOnScene.Add(Instantiate(spawnList[i], pos, rot));
        }
        GameManager.Instance.currentLevelTime = foodPile.timeLimit;
        StartCoroutine(GameManager.Instance.DelayedGameStart(3f));
    }
    private void GetLevelData(int levelNum) // Get levelData which we created from Level Editor.
    {
        levelData = FileUtils.LoadLevel(levelNum);
        LoadLevelObject();
    }
    private void LoadLevelObject() // Loads the objects to be used in this level.
    {
        for (int i = 0; i < levelData.ObjectsOnLevel.Count; i++)
        {
            for (int j = 0; j < levelData.ObjectsOnLevel[i].objectCount; j++)
            {
                levelObjects.Add(FileUtils.LoadCollectableObjects(levelData.ObjectsOnLevel[i].objectType.ToString()));
            }
        }
        CreateLevel();
    }
    private void CreateLevel() // Instantiate level objects.
    {
        for (int i = 0; i < levelObjects.Count; i++)
        {
            objectsOnScene.Add(Instantiate(levelObjects[i], CreateRandomPosition(), levelObjects[i].transform.rotation));
        }
        GameManager.Instance.currentLevelTime = levelData.TimeToFinish;
        StartCoroutine(GameManager.Instance.DelayedGameStart(1f));
        
    }
    public Vector3 CreateRandomPosition() // Create random position in Game Area.
    {
        if (foodPile) {
            var point = Random.insideUnitCircle * .5f;
            return new Vector3(point.x, 4, point.y+.15f);
        }
        float randomX = Random.Range(-1f, 1f);
        float randomZ = Random.Range(-1f,1.5f);
        Vector3 spawnPosition = new Vector3(randomX, 2.4f, randomZ);
        return spawnPosition;
    }
    public void ClearBoard() { DestroyLevel(); }
    private void DestroyLevel()
    {
        bool sourceBoard=ReplicaFlow.Instance&& !ReplicaFlow.Instance.IsPvp && ReplicaFlow.Instance.Stage==2;
        for (int i = 0; i < objectsOnScene.Count; i++)
        {
            if (objectsOnScene[i]) {
                DG.Tweening.DOTween.Kill(objectsOnScene[i].transform);
                if(sourceBoard)ReplicaSourceObjectPool.Recycle(objectsOnScene[i]);
                else {objectsOnScene[i].SetActive(false); Destroy(objectsOnScene[i]);}
            }
        }
        InputController.Instance.ResetStack();
        BoosterManager.Instance.ResetBoosters();
        levelData = null;
        objectsOnScene.Clear();
        levelObjects.Clear();
        InputController.Instance.arrivedObject.Clear();
        InputController.Instance.objectsOnStack.Clear();
        UIManager.Instance.timerText.text = "_____";
    }
    public void RestartLevel()
    {
        DestroyLevel();
        InitializeLevel();
    }
}

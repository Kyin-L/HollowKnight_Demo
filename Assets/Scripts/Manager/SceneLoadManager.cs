using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonBase<SceneLoadManager>, ISceneLoadManager
{
    private SceneLoadManager() { }

    public float m_transitionTime = 1f;
    private string m_targetSceneName;
    private string m_targetEntranceID;

    private Animator m_sceneCross;

    private int m_animatorFadeInHash;
    private int m_animatorFadeOutHash;

    private bool m_isLoading;

    private IResourceManager m_resourceManager;
    private IMonoManager m_monoManager;
    private DataManager m_dataManager;

    protected override void Initialize()
    {
        m_resourceManager = ManagerLocator.Get<IResourceManager>();
        m_monoManager = ManagerLocator.Get<IMonoManager>();
        m_dataManager = ManagerLocator.Get<DataManager>();

        GameObject sceneCross = GameObject.Find("SceneCross");
        if (sceneCross == null)
        {
            sceneCross = GameObject.Instantiate(m_resourceManager.Load<GameObject>("Prefabs/Loading/SceneCross"));
            sceneCross.transform.SetParent(GameObject.Find("GameManager").transform);
            m_sceneCross = sceneCross.GetComponentInChildren<Animator>();
        }

        m_animatorFadeInHash = Animator.StringToHash("CrossfadeIn");
        m_animatorFadeOutHash = Animator.StringToHash("CrossfadeOut");

        m_isLoading = false;
    }

    public void DelayLoadScene(string sceneName)
    {
        if (m_isLoading)
            return;

        m_monoManager.StartCoroutine(LoadScene(sceneName));
    }

    public void DelayLoadScene(LevelData data)
    {
        if (m_isLoading)
            return;

        m_dataManager.levelData = data;

        m_monoManager.StartCoroutine(LoadScene(data.sceneNane));
    }

    IEnumerator LoadScene(string sceneName)
    {
        m_isLoading = true;

        m_sceneCross.Play(m_animatorFadeOutHash);
        // Wait
        yield return new WaitForSeconds(m_transitionTime);
        // Load Scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        m_sceneCross.Play(m_animatorFadeInHash);

        m_isLoading = false;
    }
}

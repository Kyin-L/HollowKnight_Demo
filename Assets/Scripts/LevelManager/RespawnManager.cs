using UnityEngine;
using UnityEngine.Playables;
using EventHandler.Respawn;
using UnityEngine.Timeline;

public class RespawnManager : MonoBehaviour
{
    public GameObject player;
    public PlayableDirector director;
    public RespawnPoint replacePoint;
    public RespawnPoint respawnPoint;

    private IEventManager eventManager;
    private DataManager dataManager;
    private LevelManager levelManager;

    private readonly string playerTrackName = "Knight";

    void Start()
    {
        eventManager = ManagerLocator.Get<IEventManager>();
        eventManager.AddListener<ReplaceEventHandler>(Replace);
        eventManager.AddListener<RespawnEventHandler>(Respawn);

        dataManager = ManagerLocator.Get<DataManager>();
        levelManager = LevelManager.Instance;
        player = levelManager.player;
        Enter(dataManager.levelData.enterpointID);
    }

    void OnDestroy()
    {
        eventManager.RemoveListener<ReplaceEventHandler>(Replace);
        eventManager.RemoveListener<RespawnEventHandler>(Respawn);
    }

    public void Enter(int id)
    {
        RespawnPoint[] points = GetComponentsInChildren<RespawnPoint>();
        RespawnPoint point = null;
        foreach (RespawnPoint p in points)
        {
            if (p.id == id)
            {
                point = p;
                break;
            }
        }

        if(point != null)
        {
            player.transform.position = point.position;
            director.playableAsset = point.enterTimeline;
            director.Play();
        }
    }

    public void SetReplacePoint(RespawnPoint point)
    {
        replacePoint = point;
    }

    public void Replace(ReplaceEventHandler handler)
    {
        director.playableAsset = replacePoint.enterTimeline;
        director.Play();
    }

    public void ReplacePlayer()
    {
        player.transform.position = replacePoint.position;
    }

    public void Respawn(RespawnEventHandler handler)
    {
        director.playableAsset = respawnPoint.enterTimeline;
        director.Play();
    }

    public void RespawnPlayer()
    {
        player.transform.position = respawnPoint.position;
    }

    public void ReleaseCharacter()
    {
        foreach (var output in director.playableAsset.outputs)
        {
            if (output.streamName == playerTrackName)
            {
                if (output.sourceObject is AnimationTrack)
                {
                    director.SetGenericBinding(output.sourceObject, null);
                }
            }
        }
    }

    public void BindCharacterToTimeline()
    {
        foreach (var output in director.playableAsset.outputs)
        {
            if (output.streamName == playerTrackName)
            {
                if (output.sourceObject is AnimationTrack)
                {
                    director.SetGenericBinding(output.sourceObject, player.GetComponent<Animator>());
                }
                if (output.sourceObject is SignalTrack)
                {
                    director.SetGenericBinding(output.sourceObject, player.GetComponent<SignalReceiver>());
                }
            }
        }
    }
}

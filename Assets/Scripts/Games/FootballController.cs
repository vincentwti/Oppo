using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using WTI.NetCode;

public class FootballPlayerData
{
    public bool isStrikerSelected;
    public bool isGoalKeeperSelected;
    public float shootTime;
    public int p1Score; //STRIKER
    public int p2Score; //GK
    public List<MatchData> matchDataList;
}

public class MatchData
{
    public int match;
    public int winnerId; //based on ScoreController.Player ( -1 for none )
}

public class FootballController : MonoBehaviour
{
    public Button strikerButton;
    public Button goalKeeperButton;

    public Camera strikerCamera;
    public Camera goalKeeperCamera;

    public GoalKeeper goalKeeper;
    public Striker striker;
    public Ball ball;
    public Transform goal;
    public SwipeController swipeController;

    public Weather weather;
    public Locator locator;

    public SkyController skyController;
    public ScoreController scoreController;
    public TweenFade transitionAnim;
    public List<MatchData> matchDataList = new List<MatchData>();

    public enum PlayerType
    {
        None,
        Striker,
        GoalKeeper
    }

    public PlayerType playerType = PlayerType.None;

    private float shootTimer = 4f;
    private float elapsedTime;

    public bool isStarted = false;

    public Dictionary<ulong, int> clientsRoleDict = new Dictionary<ulong, int>();

    public static FootballController Instance { get; private set; }

    private void Start()
    {
        Instance = this;

        strikerButton.onClick.AddListener(OnStrikerSelected);
        goalKeeperButton.onClick.AddListener(OnGoalKeeperSelected);

        elapsedTime = 0;
        //scoreController.time.SetTime(10, ForwardShoot);

        EventManager.onStrikerSelected += OnStrikerSelected;
        EventManager.onGoalKeeperSelected += OnGoalKeeperSelected;
        EventManager.onNetworkConnected += ApplyRole;
        //if (NetworkController.Instance.GetClientId() == 1)
        //{
        //    StartCoroutine(GetWeatherData());
        //}

        //playerType = PlayerType.GoalKeeper;
        //OnGoalKeeperSelected();
    }

    public void ApplyRole()
    {
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            return;
        }
#if !UNITY_EDITOR
        Debug.Log("AplyRole");
        OnGoalKeeperSelected();
        //OnStrikerSelected();
#endif
    }

    private IEnumerator GetWeatherData()
    {
        float lati = 0f;
        float longi = 0f;
        Weather.WeatherType weatherType = Weather.WeatherType.Clear;
        yield return locator.GetLatitudeLongitude((latitude, longitude) => { lati = latitude; longi = longitude; });
        yield return weather.RequestWeather(lati, longi, (weather) => { weatherType = weather; });
        skyController.CheckCurrentTimeWeather(weatherType);
    }

    private void OnDestroy()
    {
        EventManager.onStrikerSelected -= OnStrikerSelected;
        EventManager.onGoalKeeperSelected -= OnGoalKeeperSelected;
    }

    public void StartMatch()
    {
        isStarted = true;
        if (NetworkController.Instance.GetClientId() == 1)
        {
            StartCoroutine(GetWeatherData());
        }
        if (GameManager.Instance.IsServer)
        {
            Debug.Log("Start Match");
            scoreController.time.SetTime(10, () =>
            {
                ForwardShoot();
                EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
            });
            EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsServer && ball.isShooting)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= shootTimer)
            {
                if (!CheckCurrentMatch())
                {
                    scoreController.AddScore(ScoreController.Player.player2);
                    matchDataList.Add(new MatchData { match = scoreController.GetRound(), winnerId = (int)ScoreController.Player.player2 });
                    FootballPlayerData data = new FootballPlayerData
                    {
                        isStrikerSelected = clientsRoleDict.ContainsValue((int)PlayerType.Striker),
                        isGoalKeeperSelected = clientsRoleDict.ContainsValue((int)PlayerType.GoalKeeper),
                        p1Score = scoreController.GetPlayer1Score(),
                        p2Score = scoreController.GetPlayer2Score(),
                        matchDataList = matchDataList
                    };
                    EventManager.onFootballDataSent?.Invoke(data);
                    //EventManager.onScoreUpdated?.Invoke(GameManager.Instance.GetClientId(), scoreController.GetPlayer1Score(), scoreController.GetPlayer2Score());
                }
                NextRound();
                ball.isShooting = false;
            }
        }
    }

    private ulong GetClientIdByRole(PlayerType playerType)
    {
        if (clientsRoleDict.ContainsValue((int)playerType))
        {
            return clientsRoleDict.Where(x => x.Value == (int)playerType).FirstOrDefault().Key;
        }
        return 0;
    }

    public void CheckState(bool isStrikerSelected, bool isGoalKeeperSelected, float shootTime, int p1Score, int p2Score)
    {
        OnStrikerSelected(GetClientIdByRole(PlayerType.Striker), isStrikerSelected);
        OnGoalKeeperSelected(GetClientIdByRole(PlayerType.GoalKeeper), isGoalKeeperSelected);
        //scoreController.time.SetTime();
    }

    public void CheckState()
    {
        if (GameManager.Instance.IsServer)
        {
            CheckState(!strikerButton.interactable, !goalKeeperButton.interactable, scoreController.time.GetCurrentTime(), scoreController.GetPlayer1Score(), scoreController.GetPlayer2Score());
        }
    }

    private void OnStrikerSelected()
    {
        playerType = PlayerType.Striker;
        strikerCamera.gameObject.SetActive(true);
        goalKeeperCamera.gameObject.SetActive(false);

        EventManager.onStrikerSelected?.Invoke(GameManager.Instance.GetClientId(), true);
    }

    private void OnGoalKeeperSelected()
    {
        playerType = PlayerType.GoalKeeper;
        strikerCamera.gameObject.SetActive(false);
        goalKeeperCamera.gameObject.SetActive(true);

        EventManager.onGoalKeeperSelected?.Invoke(GameManager.Instance.GetClientId(), true);
    }

    public void UpdateGoalKeeperPosition(Vector3 position, Vector3 handPosition)
    {
        goalKeeper.UpdatePosition(position, handPosition);
    }

    public void UpdateFootballPosition(Vector3 position)
    {
        ball.UpdatePosition(position);
    }

    public void Shoot()
    {
        //swipeController.CanSwipe(false);
        //scoreController.time.Pause(true);
        striker.PlayShootAnimation();
    }

    public void ForwardShoot()
    {
        if (!GameManager.Instance.IsServer)
        {
            scoreController.time.SetElapsedTime(0);
        }
        if (playerType == PlayerType.Striker)
        {
            striker.PlayShootAnimation(Vector3.forward * 25);

            NetworkController.Instance.statusText.text = "Forward Shoot";
        }
    }

    public void StartShootTimer()
    {
        Debug.Log("StartShootTimer");
        scoreController.time.SetTime(10, null);
    }

    public void NextRound()
    {
        Debug.Log("NextRound");
        NetworkController.Instance.errorMessage = "NextRound";
        elapsedTime = 0;
        PlayTransitionAnim();
        scoreController.NextRound();
        if (playerType == PlayerType.Striker)
        {
            ball.Reset();
            swipeController.CanSwipe(true);
            swipeController.ClearLine();
        }
        if (GameManager.Instance.IsServer)
        {
            scoreController.time.SetTime(10, () =>
            {
                EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
            });
            EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
            EventManager.onNextRoundStarted?.Invoke(GameManager.Instance.GetClientId());
        }
        goal.gameObject.SetActive(true);
    }

    public void ResetMatch()
    {
        Debug.Log("Reset Match");
        if (NetworkController.Instance.GetClientId() == 1)
        {
            StartCoroutine(GetWeatherData());
        }
        elapsedTime = 0;
        PlayTransitionAnim();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.isShooting = false;
        if (playerType == PlayerType.Striker)
        {
            ball.Reset();
            swipeController.CanSwipe(true);
            swipeController.ClearLine();
        }
        gameObject.SetActive(true);
    }

    public IEnumerator WaitForResetMatch()
    {
        Debug.Log("Reset Match");
        elapsedTime = 0;
        ball.isShooting = false;
        yield return new WaitForSeconds(3f);
        PlayTransitionAnim();
        matchDataList.Clear();
        scoreController.ResetMatch();
        ball.Reset();
        if (playerType == PlayerType.Striker)
        {
            swipeController.CanSwipe(true);
            swipeController.ClearLine();
        }

        scoreController.time.SetTime(10, () =>
        {
            EventManager.onShootTimerEnded?.Invoke(GameManager.Instance.GetClientId());
        });
        EventManager.onShootTimerStarted?.Invoke(GameManager.Instance.GetClientId());
        EventManager.onResetMatchStarted?.Invoke(GameManager.Instance.GetClientId());                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                   
    }

    public void PlayTransitionAnim()
    {
        Debug.Log("PlayTransitionAnim : " + transitionAnim);
        transitionAnim.Play();
    }

    public bool CheckCurrentMatch()
    {
        Debug.Log("match count : " + matchDataList.Count + " " + scoreController.GetRound());
        if (matchDataList.Count - 1 == scoreController.GetRound())
        {
            return true;
        }
        else
        {

            return false;
        }
    }

    private void StopMatch()
    {
        ball.isShooting = false;
    }

    public void OnStrikerSelected(ulong clientId, bool isSelected)
    {
        strikerButton.interactable = !isSelected;
        if (isSelected)
        {
            clientsRoleDict.Add(clientId, (int)PlayerType.Striker);
        }
        else
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public void OnGoalKeeperSelected(ulong clientId, bool isSelected)
    {
        goalKeeperButton.interactable = !isSelected;
        if (isSelected)
        {
            clientsRoleDict.Add(clientId, (int)PlayerType.GoalKeeper);
        }
        else
        {
            clientsRoleDict.Remove(clientId);
        }
    }

    public void OnPartOfDayChanged(int part)
    {
        skyController.OnPartOfDayChanged(part);
    }

    public void OnWeatherChanged(int weather)
    {
        skyController.OnWeatherChanged(weather);
    }

    public void OnDisconnected(ulong clientId)
    {
        if (clientsRoleDict.ContainsKey(clientId))
        {
            if (clientsRoleDict[clientId] == (int)PlayerType.Striker)
            {
                EventManager.onStrikerSelected.Invoke(clientId, false);
            }
            else if (clientsRoleDict[clientId] == (int)PlayerType.GoalKeeper)
            {
                EventManager.onGoalKeeperSelected.Invoke(clientId, false);
            }

            clientsRoleDict.Remove(clientId);
        }

        //if (playerType == PlayerType.Striker)
        //{
        //    EventManager.onStrikerSelected?.Invoke(false);
        //}
        //else
        //{
        //    EventManager.onGoalKeeperSelected?.Invoke(false);
        //}
    }

    public void SendPlayerData()
    {
        EventManager.onFootballDataSent?.Invoke(new FootballPlayerData
        {
            isStrikerSelected = !strikerButton.interactable,
            isGoalKeeperSelected = !goalKeeperButton.interactable,
            shootTime = scoreController.time.GetCurrentTime(),
            p1Score = scoreController.GetPlayer1Score(),
            p2Score = scoreController.GetPlayer2Score(),
            matchDataList = new List<MatchData>()
        });
    }

    //public void SendResetMatch(ulong clientId)
    //{
    //    EventManager.onResetMatchStarted?.Invoke(clientId);
    //}

    public void UpdatePlayerData(FootballPlayerData data)
    {
        strikerButton.interactable = !data.isStrikerSelected;
        goalKeeperButton.interactable = !data.isGoalKeeperSelected;
        scoreController.SetScore(data.matchDataList, data.p1Score, data.p2Score);
    }
}

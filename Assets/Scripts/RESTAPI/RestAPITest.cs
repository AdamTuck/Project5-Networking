using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class RestAPITest : MonoBehaviour
{
    [SerializeField] TMP_InputField inputUsername;
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] TMP_InputField inputScore;

    [SerializeField] private TMP_Text txtUsername;
    [SerializeField] private GameObject highscoreElementObj;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private Transform scoreBoard;

    [SerializeField] private GameObject btnRegister;
    [SerializeField] private GameObject btnLogin;

    IEnumerator RegisterUser(string _username, string _password)
    {
        User user = new User();

        user.username = _username;
        user.password = _password;

        string dataToUpload = JsonUtility.ToJson(user);

        UnityWebRequest registerUserRequest = UnityWebRequest.Post(
            "https://bootcamp-restapi-practice.xrcourse.com/register", dataToUpload, "application/json");

        yield return registerUserRequest.SendWebRequest();

        Debug.Log($"Response code: {registerUserRequest.responseCode}");
        Debug.Log($"Response error: {registerUserRequest.error}");
        Debug.Log($"Response handler: {registerUserRequest.downloadHandler.text}");

        // Login the user immediately
        StartCoroutine(LoginUser(_username, _password));
    }

    IEnumerator LoginUser(string _username, string _password)
    {
        User user = new User();

        user.username = _username;
        user.password = _password;

        string dataToUpload = JsonUtility.ToJson(user);

        UnityWebRequest loginUserRequest = UnityWebRequest.Post(
            "https://bootcamp-restapi-practice.xrcourse.com/login", dataToUpload, "application/json");

        yield return loginUserRequest.SendWebRequest();

        Debug.Log($"Response code: {loginUserRequest.responseCode}");
        Debug.Log($"Response error: {loginUserRequest.error}");
        Debug.Log($"Response handler: {loginUserRequest.downloadHandler.text}");

        // Save token if the login is successful
        Login loginData = JsonUtility.FromJson<Login>(loginUserRequest.downloadHandler.text);

        PlayerPrefs.SetString("token", loginData.token);
        txtUsername.text = _username;

        registerPanel.SetActive(false);
    }

    IEnumerator SubmitScore(int _score)
    {
        Score scoreData = new Score();
        scoreData.score = _score;

        string dataToUpload = JsonUtility.ToJson(scoreData);

        UnityWebRequest submitScoreRequest = UnityWebRequest.Post(
            "https://bootcamp-restapi-practice.xrcourse.com/submit-score", dataToUpload, "application/json");

        submitScoreRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("token"));

        yield return submitScoreRequest.SendWebRequest();

        Debug.Log($"Response code: {submitScoreRequest.responseCode}");
        Debug.Log($"Response error: {submitScoreRequest.error}");
        Debug.Log($"Response handler: {submitScoreRequest.downloadHandler.text}");

        // update the scoreboard
        StartCoroutine(UpdateScoreboard());
    }

    IEnumerator UpdateScoreboard ()
    {
        UnityWebRequest webRequest = UnityWebRequest.Get("https://bootcamp-restapi-practice.xrcourse.com/top-scores");

        yield return webRequest.SendWebRequest();

        Debug.Log($"Response code: {webRequest.responseCode}");
        Debug.Log($"Response error: {webRequest.error}");
        Debug.Log($"Response handler: {webRequest.downloadHandler.text}");

        string highScores = $"{{\"highScores\":{webRequest.downloadHandler.text}}}";

        HighScores topScores = JsonUtility.FromJson<HighScores>(highScores);

        for (int i=0; i<topScores.highScores.Length; i++)
        {
            Transform highScoreElement;

            if (i<scoreBoard.childCount)
                highScoreElement = scoreBoard.GetChild(i);
            else
                highScoreElement = Instantiate(highscoreElementObj, scoreBoard).transform;

            highScoreElement.GetChild(0).GetComponent<TMP_Text>().text = (i+1).ToString();
            highScoreElement.GetChild(1).GetComponent<TMP_Text>().text = topScores.highScores[i].username;
            highScoreElement.GetChild(2).GetComponent<TMP_Text>().text = topScores.highScores[i].highScore.ToString();
        }
    }

    public void RegisterUserButton ()
    {
        StartCoroutine(RegisterUser(inputUsername.text, inputPassword.text));
    }

    public void LoginUserCall ()
    {
        StartCoroutine(LoginUser(inputUsername.text, inputPassword.text));
    }

    public void SubmitScoreCall ()
    {
        StartCoroutine(SubmitScore(int.Parse(inputScore.text)));
    }

    public void UpdateScoreBoard()
    {
        StartCoroutine(UpdateScoreboard());
    }
}
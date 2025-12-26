using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class APIService
{
    private static string baseUrl = "https://your-backend-api.com"; // change later

    public static async Task<PlayerData> GetPlayerData(string playerId)
    {
        string url = $"{baseUrl}/player/{playerId}";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
                return JsonUtility.FromJson<PlayerData>(request.downloadHandler.text);
            else
                Debug.LogError("GetPlayerData failed: " + request.error);
        }
        return null;
    }

    public static async Task<bool> UpdatePlayerScore(string playerId, int newScore)
    {
        string url = $"{baseUrl}/player/{playerId}/updateScore";
        WWWForm form = new WWWForm();
        form.AddField("score", newScore);

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            await request.SendWebRequest();
            return request.result == UnityWebRequest.Result.Success;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class ChatMessage
{
    public string role;
    public string content;
}

[Serializable]
public class ChatRequest
{
    public string model;
    public ChatMessage[] messages;
    public bool stream;
    public int seed;
}

// Response DTOs for parsing API response
[Serializable]
public class ChatChoice
{
    public string finish_reason;
    public int index;
    public ChatMessage message;
}

[Serializable]
public class ChatCompletionResponse
{
    public string id;
    public long created;
    public string model;
    public string @object; // not required but harmless; can be ignored
    public ChatChoice[] choices;
}

public class LLM : MonoBehaviour
{
    public static LLM instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [Header("API settings")]
    public string apiKey = "sk-zM-NQtQ8f4qe7mhYag60lQ";
    private const string apiUrl = "https://llmapi.paratera.com/v1/chat/completions";
    public string modelId = "Qwen3-Next-80B-A3B-Instruct";

    public async Task<string> Chat(ChatMessage[] messages)
    {
        int randomSeed = UnityEngine.Random.Range(0, int.MaxValue);
        
        var requestBody = new ChatRequest
        {
            model = modelId,
            messages = messages,
            stream = false,
            seed = randomSeed
		};

        string json = JsonUtility.ToJson(requestBody);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);
            request.SetRequestHeader("Accept", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            string responseText = request.downloadHandler.text;

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"failed: {request.responseCode} {request.error}\nRequest: {json}\nResponse: {responseText}");
                return null;
            }

            // Parse the wrapper response and return just the assistant message content
            ChatCompletionResponse parsed = null;
            try
            {
                parsed = JsonUtility.FromJson<ChatCompletionResponse>(responseText);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to parse response JSON: {ex.Message}\nResponse: {responseText}");
                return null;
            }

            string content = null;
            if (parsed != null && parsed.choices != null && parsed.choices.Length > 0 && parsed.choices[0].message != null)
            {
                content = parsed.choices[0].message.content;
            }

            if (string.IsNullOrEmpty(content))
            {
                Debug.LogError($"Response did not contain choices[0].message.content. Full response: {responseText}");
                return null;
            }

            Debug.Log($"result (seed: {randomSeed})£º" + content);
            return content;
        }
    }
}


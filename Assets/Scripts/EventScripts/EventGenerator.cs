using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EventGenerator : MonoBehaviour
{
	public static EventGenerator instance { get; private set; }
	public string currentEvent;
	private object lockObj = new object();
	private List<ChatMessage> chatHistory = new List<ChatMessage>();
	
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		instance = this;
		DontDestroyOnLoad(gameObject);
	}
	
	private void Start()
	{
		chatHistory.Add(new ChatMessage
		{
			role = "system",
			content = @"
请生成一个奇幻游戏事件，该事件需要让玩家做出行动，要求如下：
- 事件包含一个简短的描述。
- 事件描述应富有想象力且引人入胜。
- 事件应包含一个明确的情境或挑战，促使玩家做出选择或采取行动。
- 事件应该选取不同的主题和场景，避免重复之前的事件。
返回格式：直接返回一个字符串描述事件"
		});
		
		GenerateEvent();
	}
	
	private async void GenerateEvent()
	{
		chatHistory.Add(new ChatMessage
		{
			role = "user",
			content = "请按照系统提示词生成事件"
		});
		
		Debug.Log("generating...");
		
		var ev = await LLM.instance.Chat(chatHistory.ToArray());
		
		if (!string.IsNullOrEmpty(ev))
		{
			chatHistory.Add(new ChatMessage
			{
				role = "assistant",
				content = ev
			});
			
			int maxMessages = 1 + (3 * 2);
			if (chatHistory.Count > maxMessages)
			{
				var systemMsg = chatHistory[0];
				var recentMsgs = chatHistory.GetRange(chatHistory.Count - (maxMessages - 1), maxMessages - 1);
				chatHistory.Clear();
				chatHistory.Add(systemMsg);
				chatHistory.AddRange(recentMsgs);
			}
		}
		
		lock (lockObj)
		{
			currentEvent = ev;
		}
		Debug.Log("generation done");
	}
	
	public async Task<string> GetEvent()
	{
		while (true)
		{
			lock (lockObj)
			{
				if (!string.IsNullOrEmpty(currentEvent))
				{
					string ev = currentEvent;
					currentEvent = "";
					Debug.Log("Event: " + ev);
					GenerateEvent();
					return ev;
				}
			}
			await Task.Delay(100);
		}
	}
}

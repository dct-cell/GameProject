using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

public class EventGenerator : MonoBehaviour
{
	public static EventGenerator instance { get; private set; }
	public string currentEvent;
	private object lockObj = new object();
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
		GenerateEvent();
	}
	private async void GenerateEvent()
	{
		string userPrompt = @"
请按照系统提示词生成事件
";
		string systemPrompt = @"
请生成一个奇幻游戏事件，该事件需要让玩家做出行动，要求如下：
- 事件包含一个简短的描述。
- 事件描述应富有想象力且引人入胜。
返回格式：直接返回一个字符串描述事件
";
		Debug.Log("generating...");
		var ev = await LLM.instance.Chat(userPrompt, systemPrompt);
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
					break;
			}
			await Task.Delay(100);
		}

		string ev;
		lock (lockObj)
		{
			ev = currentEvent;
			currentEvent = "";
			Debug.Log("Event: " + ev);
		}
		return ev;
	}

}

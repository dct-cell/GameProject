using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class EventOutcome
{
	public int gold;
	public int hp;
}

[Serializable]
public class EventReply
{
	public string narrative;
	public EventOutcome outcome;
}

public class ReplyGenerator : MonoBehaviour
{
	public static ReplyGenerator instance { get; private set; }
	private List<ChatMessage> chatHistory = new List<ChatMessage>();
	string eventDescription;
	
	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject);
		}
		instance = this;
		chatHistory.Add(new ChatMessage
		{
			role = "system",
			content = @"
你是一个游戏事件叙事引擎，负责根据当前事件和玩家行为，
生成生动的剧情描述，以及可供游戏逻辑解析的结构化反馈。

【你的任务目标】
1. 你需要根据输入的“事件信息”和“玩家输入”生成一个完整事件的响应。
2. 你的输出分为两个部分：
   - narrative（文本叙述）：自然语言，描述事件发生的过程和结果，语气贴合游戏风格。
   - outcome（结构化数据）：一个JSON对象，用于告诉游戏逻辑发生了什么数值变化。

【输出格式（必须严格遵守）】
输出请使用以下JSON结构，由大括号包裹，不包含额外字符：
{
  ""narrative"": ""（这里是一段描述事件的自然语言文本）"",
  ""outcome"": {
    ""gold"": （一个整数，表示金币变化，可以是正数或者负数）,
    ""hp"": （一个整数，表示血量变化，可以是正数或者负数）,
  }
}

【编写规则】
- narrative 要和 outcome 对应，不得矛盾。
- 不得输出除JSON之外的任何额外文字。
- 不得输出解释、注释或前后缀。
- narrative 用简体中文书写，只使用正常的中文文字和标点符号。

【风格建议】
- 应根据事件类型自动调整语气。
- 生成结果要有逻辑感，不应随机无关。
"
		});
	}

	public void InitEvent(string ev)
	{
		eventDescription = ev;
	}

	public async Task<string> GenerateReply(string playerAction)
	{
		chatHistory.Add(new ChatMessage
		{
			role = "user",
			content = $"事件描述：{eventDescription}\n玩家行动：{playerAction}\n请根据系统提示词的要求生成回复。"
		});
		
		var reply = await LLM.instance.Chat(chatHistory.ToArray());
		
		if (string.IsNullOrEmpty(reply))
		{
			Debug.LogError("LLM returned null or empty reply");
			return null;
		}
		
		chatHistory.Add(new ChatMessage
		{
			role = "assistant",
			content = reply
		});
		
		EventReply eventReply = null;
		try
		{
			string cleanedReply = reply.Trim();
			Debug.Log($"Parsing reply: {cleanedReply}");
			eventReply = JsonUtility.FromJson<EventReply>(cleanedReply);
			
			if (eventReply == null)
			{
				Debug.LogError("Failed to parse EventReply: result is null");
				return null;
			}
			
		}
		catch (Exception ex)
		{
			Debug.LogError($"Failed to parse JSON reply: {ex.Message}\nReply content: {reply}");
			return null;
		}
		
		return eventReply.narrative;
	}
}

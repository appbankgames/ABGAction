using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Base class for action
/// </summary>

public class ABGAction : MonoBehaviour
{
	static public GameObject container = null;
	public GameObject target = null;
	
	protected GameObject callbackTarget;
	protected string callbackMessage;
	protected object callbackArgument;
	protected System.Action didPlayActionDelegate;
	protected bool ignoresCallbackWhenActionFinishedIncompletely = false;
	protected bool hasCallbackBeenSent = false;
	
	
	static public GameObject GetContainer()
	{
		if(container == null)
		{
			string name = typeof(ABGAction).FullName;
			container = GameObject.Find(name);
			if(container == null)
			{
				container = new GameObject();
				container.name = name;
			}
		}
		
		return container;
	}
	
	
	static public Color GetColor(GameObject target)
	{
		if(target != null)
		{
			UIWidget widget = target.GetComponentInChildren<UIWidget>();
			if(widget != null)
			{
				return widget.color;
			}

			SysFontText sysFontText = target.GetComponentInChildren<SysFontText>();
			if(sysFontText != null)
			{
				return sysFontText.FontColor;
			}
			
			if(target.renderer != null)
			{
				if(target.renderer.material != null && target.renderer.material.HasProperty("_Color"))
				{
					return target.renderer.material.color;
				}
			}
		}
		
		return new Color(1.0f, 1.0f, 1.0f, 1.0f);
	}


	[System.Obsolete]
	static public void Play(ABGAction action, GameObject callbackTarget, string callbackMessage, object callbackArgument)
	{
		action.Play(callbackTarget, callbackMessage, callbackArgument);
	}
	
	
	[System.Obsolete]
	static public void Play(ABGAction action, GameObject callbackTarget, string callbackMessage)
	{
		Play(action, callbackTarget, callbackMessage, null);
	}


	[System.Obsolete]
	static public void Play(ABGAction action)
	{
		Play(action, null, "");
	}
	
	
	void Awake()
	{
		gameObject.transform.parent = ABGAction.GetContainer().transform;
		gameObject.name = GetType().FullName;
	}


	void OnDestroy()
	{
		Stop();
	}
	
	
	public void SetParent(GameObject parent)
	{
		if(parent != null)
		{
			gameObject.transform.parent = parent.transform;
		}
	}


	public virtual void DidPlayABGAction()
	{
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - this=" + this + ", target=" + target + ", callbackTarget=" + callbackTarget + ", callbackMessage=" + callbackMessage + ", callbackArgument=" + callbackArgument);
		
		if(!hasCallbackBeenSent)
		{
			if(callbackTarget != null && callbackMessage != "")
			{
				callbackTarget.SendMessage(callbackMessage, callbackArgument, SendMessageOptions.DontRequireReceiver);
			}
			else if(didPlayActionDelegate != null)
			{
				didPlayActionDelegate();
			}
			hasCallbackBeenSent = true;
		}

		Stop();
	}


	public virtual void DidPlayABGActionIncompletely()
	{
//		Debug.LogWarning(System.Reflection.MethodBase.GetCurrentMethod().Name + " - this=" + this + ", target=" + target + ", callbackTarget=" + callbackTarget + ", callbackMessage=" + callbackMessage + ", callbackArgument=" + callbackArgument);

		if(!hasCallbackBeenSent)
		{
			if(ignoresCallbackWhenActionFinishedIncompletely)
			{
				callbackTarget.SendMessage("DidPlayABGActionIncompletely", SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				if(callbackTarget != null && callbackMessage != "")
				{
					callbackTarget.SendMessage(callbackMessage, callbackArgument, SendMessageOptions.DontRequireReceiver);
				}
				else if(didPlayActionDelegate != null)
				{
					didPlayActionDelegate();
				}
			}
			hasCallbackBeenSent = true;
		}

		Stop();
	}
	

	public virtual void Play(GameObject parent, GameObject callbackTarget, string callbackMessage, object callbackArgument, System.Action didPlayActionDelegate)
	{
		SetParent(parent);

		this.callbackTarget = callbackTarget;
		this.callbackMessage = callbackMessage;
		this.callbackArgument = callbackArgument;
		this.didPlayActionDelegate = didPlayActionDelegate;
		hasCallbackBeenSent = false;
	}


	public virtual void Play(GameObject parent, GameObject callbackTarget, string callbackMessage, object callbackArgument)
	{
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - this=" + this + ", target=" + target + ", callbackTarget=" + callbackTarget + ", callbackMessage=" + callbackMessage + ", callbackArgument=" + callbackArgument);

		Play(parent, callbackTarget, callbackMessage, callbackArgument, null);
	}
	
	
	public virtual void Play(GameObject parent, GameObject callbackTarget, string callbackMessage)
	{
		Play(parent, callbackTarget, callbackMessage, null, null);
	}


	public virtual void Play(GameObject parent, System.Action didPlayActionDelegate)
	{
		Play(parent, null, "", null, didPlayActionDelegate);
	}


	public virtual void Play(GameObject parent)
	{
		Play(parent, null, "", null, null);
	}


	public virtual void Play(GameObject callbackTarget, string callbackMessage, object callbackArgument)
	{
		Play((GameObject)null, callbackTarget, callbackMessage, callbackArgument);
	}
	
	
	public virtual void Play(GameObject callbackTarget, string callbackMessage)
	{
		Play((GameObject)null, callbackTarget, callbackMessage);
	}


	public virtual void Play(System.Action didPlayActionDelegate)
	{
		Play((GameObject)null, didPlayActionDelegate);
	}


	public virtual void Play()
	{
		Play((GameObject)null);
	}


	public virtual void Stop()
	{
		if(gameObject != null)
		{
			Destroy(gameObject);
		}
	}
}


/// <summary>
/// Run some actions at a time
/// </summary>

public class ABGActionSet : ABGAction
{
	private List<ABGAction> actions = new List<ABGAction>();
	private int countOfDoneAction;
	
	
	static public ABGActionSet Action(params ABGAction[] actions)
	{
		GameObject obj = new GameObject();
		ABGActionSet action = obj.AddComponent<ABGActionSet>();
		return action.Init(actions);
	}


	public ABGActionSet Init(params ABGAction[] actions)
	{
		AddAction(actions);
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		countOfDoneAction = 0;
		for(int i=0; i<actions.Count; i++)
		{
			ABGAction action = actions[i];
			action.Play(gameObject, "DidPlayABGActionSet", i);
		}
	}


	public override void Stop()
	{
		for(int i=0; i<actions.Count; i++)
		{
			if(actions[i] != null)
			{
				actions[i].Stop();
			}
		}

		base.Stop();
	}


	public void DidPlayABGActionSet(int index)
	{
		if(index >= 0 && index <= actions.Count-1)
		{
			actions[index] = null;
			if(++countOfDoneAction == actions.Count)
			{
				DidPlayABGAction();
			}
		}
	}


	public void AddAction(params ABGAction[] actions)
	{
		this.actions.AddRange(actions);

		foreach(ABGAction action in actions)
		{
			action.SetParent(gameObject);
		}
	}
}


/// <summary>
/// Run actions sequentially
/// </summary>

public class ABGActionSequential : ABGAction
{
	private List<ABGAction> actions = new List<ABGAction>();
	private int index;
	
	
	static public ABGActionSequential Action(params ABGAction[] actions)
	{
		GameObject obj = new GameObject();
		ABGActionSequential action = obj.AddComponent<ABGActionSequential>();
		return action.Init(actions);
	}
	
	
	public ABGActionSequential Init(params ABGAction[] actions)
	{
		AddAction(actions);
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		index = 0;
		PlayNextAction();
	}


	public override void Stop()
	{
		for(int i=0; i<actions.Count; i++)
		{
			if(actions[i] != null)
			{
				actions[i].Stop();
			}
		}

		base.Stop();
	}


	public void DidPlayABGActionSequential()
	{
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name);

		actions[index] = null;

		index++;
		PlayNextAction();
	}

	public void PlayNextAction()
	{
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - index=" + index + "/actions.Count=" + actions.Count);
		
		if(index >= 0 && index <= actions.Count-1)
		{
			ABGAction action = actions[index];
			if(index == actions.Count-1)
			{
				action.Play(gameObject, "DidPlayABGAction");
			}
			else
			{
				action.Play(gameObject, "DidPlayABGActionSequential");
			}
		}
	}
	
	
	public void AddAction(params ABGAction[] actions)
	{
		this.actions.AddRange(actions);

		foreach(ABGAction action in actions)
		{
			action.SetParent(gameObject);
		}
	}
}


/// <summary>
/// Action to send message
/// </summary>

public class ABGActionMessage : ABGAction
{
	public string message;
	public object arg;
	
	
	static public ABGActionMessage Action(GameObject target, string message, object arg)
	{
		GameObject obj = new GameObject();
		ABGActionMessage action = obj.AddComponent<ABGActionMessage>();
		return action.Init(target, message, arg);
	}
	
	
	static public ABGActionMessage Action(GameObject target, string message)
	{
		return Action(target, message, null);
	}
	
	
	public ABGActionMessage Init(GameObject target, string message, object arg)
	{
		this.target = target;
		this.message = message;
		this.arg = arg;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		if(!target){
			return;
		}

		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		target.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
		DidPlayABGAction();
	}
}


/// <summary>
/// Action to change GameObject activation
/// </summary>

public class ABGActionActivate : ABGAction
{
	public bool activated;
	public bool recursively;
	
	
	static public ABGActionActivate Action(GameObject target, bool activated, bool recursively)
	{
		GameObject obj = new GameObject();
		ABGActionActivate action = obj.AddComponent<ABGActionActivate>();
		return action.Init(target, activated, recursively);
	}
	
	
	static public ABGActionActivate Action(GameObject target, bool activated)
	{
		return Action(target, activated, false);
	}
	
	
	public ABGActionActivate Init(GameObject target, bool activated, bool recursively)
	{
		this.target = target;
		this.activated = activated;
		this.recursively = recursively;
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		if(target == null)
		{
			return;
		}

		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(recursively)
		{
			target.SetActiveRecursively(activated);
		}
		else
		{
			target.active = activated;
		}

		DidPlayABGAction();
	}
}


/// <summary>
/// Base class for finite time action
/// </summary>

public class ABGActionFiniteTime : ABGAction
{
	public float duration;


	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		StartCoroutine("WaitCompletion");
	}


	public override void DidPlayABGAction()
	{
		StopCoroutine("WaitCompletion");

		base.DidPlayABGAction();
	}


	public IEnumerator WaitCompletion()
	{
		yield return new WaitForSeconds(duration + 60.0f / 60.0f);
//		Debug.LogWarning(System.Reflection.MethodBase.GetCurrentMethod().Name + " - this=" + this + ", target=" + target + ", callbackTarget=" + callbackTarget + ", callbackMessage=" + callbackMessage + ", callbackArgument=" + callbackArgument);
		DidPlayABGActionIncompletely();
	}
}


/// <summary>
/// Action to just delay
/// </summary>

public class ABGActionDelay : ABGActionFiniteTime
{
	private bool isFinishedIncompletely;
	
	
	static public ABGActionDelay Action(float duration)
	{
		GameObject obj = new GameObject();
		ABGActionDelay action = obj.AddComponent<ABGActionDelay>();
		return action.Init(duration);
	}
	
	
	public ABGActionDelay Init(float duration)
	{
		this.duration = duration;
		return this;
	}


	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		StartCoroutine("Delay");
	}


	public override void Stop()
	{
		StopCoroutine("Delay");

		base.Stop();
	}
	
	
	public IEnumerator Delay()
	{
		yield return new WaitForSeconds(duration);
		DidPlayABGAction();
	}
}


/// <summary>
/// Base class for NGUI tween
/// </summary>

public class ABGActionNGUITween : ABGActionFiniteTime
{
	public UITweener.Method method = UITweener.Method.Linear;
}


/// <summary>
/// Action using NGUI's TweenColor
/// </summary>

public class ABGActionNGUITweenColor : ABGActionNGUITween
{
	public Color fromColor;
	public Color toColor;
	
	
	static public ABGActionNGUITweenColor Action(GameObject target, float duration, Color fromColor, Color toColor, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenColor action = obj.AddComponent<ABGActionNGUITweenColor>();
		return action.Init(target, duration, fromColor, toColor, method);
	}
	
	
	static public ABGActionNGUITweenColor Action(GameObject target, float duration, Color fromColor, Color toColor)
	{
		return Action(target, duration, fromColor, toColor, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColor ActionTo(GameObject target, float duration, Color toColor, UITweener.Method method)
	{
		Color fromColor = GetColor(target.gameObject);
		return Action(target, duration, fromColor, toColor, method);
	}
	
	
	static public ABGActionNGUITweenColor ActionTo(GameObject target, float duration, Color toColor)
	{
		return ActionTo(target, duration, toColor, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColor ActionFrom(GameObject target, float duration, Color fromColor, UITweener.Method method)
	{
		Color toColor = GetColor(target.gameObject);
		return Action(target, duration, fromColor, toColor, method);
	}
	
	
	static public ABGActionNGUITweenColor ActionFrom(GameObject target, float duration, Color fromColor)
	{
		return ActionFrom(target, duration, fromColor, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenColor Init(GameObject target, float duration, Color fromColor, Color toColor, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromColor = fromColor;
		this.toColor = toColor;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		ABGTweenColor tween = ABGTweenColor.Begin(target, duration, toColor);
		if(tween == null)
		{
			return;
		}

		tween.from = fromColor;
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using NGUI's TweenColor (alpha only)
/// </summary>

public class ABGActionNGUITweenColorAlpha : ABGActionNGUITween
{
	public float fromAlpha;
	public float toAlpha;
	
	
	static public ABGActionNGUITweenColorAlpha Action(GameObject target, float duration, float fromAlpha, float toAlpha, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenColorAlpha action = obj.AddComponent<ABGActionNGUITweenColorAlpha>();
		return action.Init(target, duration, fromAlpha, toAlpha, method);
	}
	
	
	static public ABGActionNGUITweenColorAlpha Action(GameObject target, float duration, float fromAlpha, float toAlpha)
	{
		return Action(target, duration, fromAlpha, toAlpha, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorAlpha ActionTo(GameObject target, float duration, float toAlpha, UITweener.Method method)
	{
		float fromAlpha = 1.0f;
		Color color = GetColor(target.gameObject);
		fromAlpha = color.a;
		return Action(target, duration, fromAlpha, toAlpha, method);
	}
	
	
	static public ABGActionNGUITweenColorAlpha ActionTo(GameObject target, float duration, float toAlpha)
	{
		return ActionTo(target, duration, toAlpha, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorAlpha ActionFrom(GameObject target, float duration, float fromAlpha, UITweener.Method method)
	{
		float toAlpha = 1.0f;
		Color color = GetColor(target.gameObject);
		toAlpha = color.a;
		return Action(target, duration, fromAlpha, toAlpha, method);
	}
	
	
	static public ABGActionNGUITweenColorAlpha ActionFrom(GameObject target, float duration, float fromAlpha)
	{
		return ActionFrom(target, duration, fromAlpha, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenColorAlpha Init(GameObject target, float duration, float fromAlpha, float toAlpha, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromAlpha = fromAlpha;
		this.toAlpha = toAlpha;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		Color color = GetColor(target.gameObject);

		ABGTweenColor tween = ABGTweenColor.Begin(target, duration, new Color(color.r, color.g, color.b, toAlpha));
		if(tween == null)
		{
			return;
		}

		tween.from = new Color(color.r, color.g, color.b, fromAlpha);
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using NGUI's TweenColor recursively
/// </summary>

public class ABGActionNGUITweenColorRecursive : ABGActionNGUITween
{
	public Color fromColor;
	public Color toColor;
	private bool isFromColorUndefined = false;
	private bool isToColorUndefined = false;

	
	static public ABGActionNGUITweenColorRecursive Action(GameObject target, float duration, Color fromColor, Color toColor, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenColorRecursive action = obj.AddComponent<ABGActionNGUITweenColorRecursive>();
		return action.Init(target, duration, fromColor, toColor, method);
	}
	
	
	static public ABGActionNGUITweenColorRecursive Action(GameObject target, float duration, Color fromColor, Color toColor)
	{
		return Action(target, duration, fromColor, toColor, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorRecursive ActionTo(GameObject target, float duration, Color toColor, UITweener.Method method)
	{
		ABGActionNGUITweenColorRecursive action = Action(target, duration, new Color(1.0f, 1.0f, 1.0f, 1.0f), toColor, method);
		action.isFromColorUndefined = true;
		return action;
	}
	
	
	static public ABGActionNGUITweenColorRecursive ActionTo(GameObject target, float duration, Color toColor)
	{
		return ActionTo(target, duration, toColor, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorRecursive ActionFrom(GameObject target, float duration, Color fromColor, UITweener.Method method)
	{
		ABGActionNGUITweenColorRecursive action = Action(target, duration, fromColor, new Color(1.0f, 1.0f, 1.0f, 1.0f), method);
		action.isToColorUndefined = true;
		return action;
	}
	
	
	static public ABGActionNGUITweenColorRecursive ActionFrom(GameObject target, float duration, Color fromColor)
	{
		return ActionFrom(target, duration, fromColor, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenColorRecursive Init(GameObject target, float duration, Color fromColor, Color toColor, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromColor = fromColor;
		this.toColor = toColor;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		Transform[] children = target.GetComponentsInChildren<Transform>();
		for(int i=0; i<children.Length; i++)
		{
			Transform child = children[i];
			ABGTweenColor tween = ABGTweenColor.Begin(child.gameObject, duration, (!isToColorUndefined? toColor: GetColor(child.gameObject)));
			if(tween == null)
			{
				continue;
			}
			
			tween.from = (!isFromColorUndefined? fromColor: GetColor(child.gameObject));
			tween.method = method;
			if(i == children.Length-1)
			{
				tween.eventReceiver = gameObject;
				tween.callWhenFinished = "DidPlayABGAction";
			}
		}
	}
}


/// <summary>
/// Action using NGUI's TweenColor recursively (alpha only)
/// </summary>

public class ABGActionNGUITweenColorAlphaRecursive : ABGActionNGUITween
{
	public float fromAlpha;
	public float toAlpha;
	private bool isFromAlphaUndefined = false;
	private bool isToAlphaUndefined = false;
	
	
	static public ABGActionNGUITweenColorAlphaRecursive Action(GameObject target, float duration, float fromAlpha, float toAlpha, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenColorAlphaRecursive action = obj.AddComponent<ABGActionNGUITweenColorAlphaRecursive>();
		return action.Init(target, duration, fromAlpha, toAlpha, method);
	}
	
	
	static public ABGActionNGUITweenColorAlphaRecursive Action(GameObject target, float duration, float fromAlpha, float toAlpha)
	{
		return Action(target, duration, fromAlpha, toAlpha, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorAlphaRecursive ActionTo(GameObject target, float duration, float toAlpha, UITweener.Method method)
	{
		ABGActionNGUITweenColorAlphaRecursive action = Action(target, duration, 1.0f, toAlpha, method);
		action.isFromAlphaUndefined = true;
		return action;
	}
	
	
	static public ABGActionNGUITweenColorAlphaRecursive ActionTo(GameObject target, float duration, float toAlpha)
	{
		return ActionTo(target, duration, toAlpha, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenColorAlphaRecursive ActionFrom(GameObject target, float duration, float fromAlpha, UITweener.Method method)
	{
		ABGActionNGUITweenColorAlphaRecursive action = Action(target, duration, fromAlpha, 1.0f, method);
		action.isToAlphaUndefined = true;
		return action;
	}
	
	
	static public ABGActionNGUITweenColorAlphaRecursive ActionFrom(GameObject target, float duration, float fromAlpha)
	{
		return ActionFrom(target, duration, fromAlpha, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenColorAlphaRecursive Init(GameObject target, float duration, float fromAlpha, float toAlpha, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromAlpha = fromAlpha;
		this.toAlpha = toAlpha;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		if(!target){
			return;	
		}
		
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		Transform[] children = target.GetComponentsInChildren<Transform>();
		for(int i=0; i<children.Length; i++)
		{
			Transform child = children[i];
			Color color = GetColor(child.gameObject);
			ABGTweenColor tween = ABGTweenColor.Begin(child.gameObject, duration, new Color(color.r, color.g, color.b, (!isToAlphaUndefined? toAlpha: color.a)));
			if(tween == null)
			{
				continue;
			}
			
			tween.from = new Color(color.r, color.g, color.b, (!isFromAlphaUndefined? fromAlpha: color.a));
			tween.method = method;
			if(i == children.Length-1)
			{
				tween.eventReceiver = gameObject;
				tween.callWhenFinished = "DidPlayABGAction";
			}
		}
	}
}


/// <summary>
/// Action using NGUI's TweenScale
/// </summary>

public class ABGActionNGUITweenPosition : ABGActionNGUITween
{
	public Vector3 fromPos;
	public Vector3 toPos;
	
	
	static public ABGActionNGUITweenPosition Action(GameObject target, float duration, Vector3 fromPos, Vector3 toPos, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenPosition action = obj.AddComponent<ABGActionNGUITweenPosition>();
		return action.Init(target, duration, fromPos, toPos, method);
	}
	
	
	static public ABGActionNGUITweenPosition Action(GameObject target, float duration, Vector3 fromPos, Vector3 toPos)
	{
		return Action(target, duration, fromPos, toPos, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenPosition ActionTo(GameObject target, float duration, Vector3 toPos, UITweener.Method method)
	{
		return Action(target, duration, target.transform.localPosition, toPos, method);
	}
	
	
	static public ABGActionNGUITweenPosition ActionTo(GameObject target, float duration, Vector3 toPos)
	{
		return ActionTo(target, duration, toPos, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenPosition ActionFrom(GameObject target, float duration, Vector3 fromPos, UITweener.Method method)
	{
		return Action(target, duration, fromPos, target.transform.localPosition, method);
	}
	
	
	static public ABGActionNGUITweenPosition ActionFrom(GameObject target, float duration, Vector3 fromPos)
	{
		return ActionFrom(target, duration, fromPos, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenPosition Init(GameObject target, float duration, Vector3 fromPos, Vector3 toPos, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromPos = fromPos;
		this.toPos = toPos;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		TweenPosition tween = TweenPosition.Begin(target, duration, toPos);
		tween.from = fromPos;
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using NGUI's TweenRotation
/// </summary>

public class ABGActionNGUITweenRotation : ABGActionNGUITween
{
	public Vector3 fromRotation;
	public Vector3 toRotation;
	
	
	static public ABGActionNGUITweenRotation Action(GameObject target, float duration, Vector3 fromRotation, Vector3 toRotation, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenRotation action = obj.AddComponent<ABGActionNGUITweenRotation>();
		return action.Init(target, duration, fromRotation, toRotation, method);
	}
	
	
	static public ABGActionNGUITweenRotation Action(GameObject target, float duration, Vector3 fromRotation, Vector3 toRotation)
	{
		return Action(target, duration, fromRotation, toRotation, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenRotation ActionTo(GameObject target, float duration, Vector3 toRotation, UITweener.Method method)
	{
		return Action(target, duration, target.transform.localRotation.eulerAngles, toRotation, method);
	}
	
	
	static public ABGActionNGUITweenRotation ActionTo(GameObject target, float duration, Vector3 toRotation)
	{
		return ActionTo(target, duration, toRotation, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenRotation ActionFrom(GameObject target, float duration, Vector3 fromRotation, UITweener.Method method)
	{
		return Action(target, duration, fromRotation, target.transform.localRotation.eulerAngles, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenRotation ActionFrom(GameObject target, float duration, Vector3 fromRotation)
	{
		return ActionFrom(target, duration, fromRotation, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenRotation Init(GameObject target, float duration, Vector3 fromRotation, Vector3 toRotation, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromRotation = fromRotation;
		this.toRotation = toRotation;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		TweenRotation tween = TweenRotation.Begin(target, duration, Quaternion.Euler(toRotation));
		tween.from = fromRotation;
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using NGUI's TweenScale
/// </summary>

public class ABGActionNGUITweenScale : ABGActionNGUITween
{
	public Vector3 fromScale;
	public Vector3 toScale;
	
	
	static public ABGActionNGUITweenScale Action(GameObject target, float duration, Vector3 fromScale, Vector3 toScale, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenScale action = obj.AddComponent<ABGActionNGUITweenScale>();
		return action.Init(target, duration, fromScale, toScale, method);
	}
	
	
	static public ABGActionNGUITweenScale Action(GameObject target, float duration, Vector3 fromScale, Vector3 toScale)
	{
		return Action(target, duration, fromScale, toScale, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenScale ActionTo(GameObject target, float duration, Vector3 toScale, UITweener.Method method)
	{
		return Action(target, duration, target.transform.localScale, toScale, method);
	}
	
	
	static public ABGActionNGUITweenScale ActionTo(GameObject target, float duration, Vector3 toScale)
	{
		return ActionTo(target, duration, toScale, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenScale ActionFrom(GameObject target, float duration, Vector3 fromScale, UITweener.Method method)
	{
		return Action(target, duration, fromScale, target.transform.localScale, method);
	}
	
	
	static public ABGActionNGUITweenScale ActionFrom(GameObject target, float duration, Vector3 fromScale)
	{
		return ActionFrom(target, duration, fromScale, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenScale Init(GameObject target, float duration, Vector3 fromScale, Vector3 toScale, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromScale = fromScale;
		this.toScale = toScale;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		if(!target){
			return;	
		}
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		TweenScale tween = TweenScale.Begin(target, duration, toScale);
		tween.from = fromScale;
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using NGUI's TweenTransform
/// </summary>

public class ABGActionNGUITweenTransform : ABGActionNGUITween
{
	public Transform fromTransform;
	public Transform toTransform;
	
	
	static public ABGActionNGUITweenTransform Action(GameObject target, float duration, Transform fromTransform, Transform toTransform, UITweener.Method method)
	{
		GameObject obj = new GameObject();
		ABGActionNGUITweenTransform action = obj.AddComponent<ABGActionNGUITweenTransform>();
		return action.Init(target, duration, fromTransform, toTransform, method);
	}
	
	
	static public ABGActionNGUITweenTransform Action(GameObject target, float duration, Transform fromTransform, Transform toTransform)
	{
		return Action(target, duration, fromTransform, toTransform, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenTransform ActionTo(GameObject target, float duration, Transform toTransform, UITweener.Method method)
	{
		return Action(target, duration, target.transform, toTransform, method);
	}
	
	
	static public ABGActionNGUITweenTransform ActionTo(GameObject target, float duration, Transform toTransform)
	{
		return ActionTo(target, duration, toTransform, UITweener.Method.Linear);
	}
	
	
	static public ABGActionNGUITweenTransform ActionFrom(GameObject target, float duration, Transform fromTransform, UITweener.Method method)
	{
		return Action(target, duration, fromTransform, target.transform, method);
	}
	
	
	static public ABGActionNGUITweenTransform ActionFrom(GameObject target, float duration, Transform fromTransform)
	{
		return ActionFrom(target, duration, fromTransform, UITweener.Method.Linear);
	}
	
	
	public ABGActionNGUITweenTransform Init(GameObject target, float duration, Transform fromTransform, Transform toTransform, UITweener.Method method)
	{
		this.target = target;
		this.duration = duration;
		this.fromTransform = fromTransform;
		this.toTransform = toTransform;
		this.method = method;
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		TweenTransform tween = TweenTransform.Begin(target, duration, fromTransform, toTransform);
		tween.method = method;
		tween.eventReceiver = gameObject;
		tween.callWhenFinished = "DidPlayABGAction";
	}
}


/// <summary>
/// Action using iTween
/// </summary>

public class ABGActioniTween : ABGActionFiniteTime
{
	public string tween;
	public Hashtable args;
	public string tweenName;
	
	static public int sequentialNumber = 0;
	
	
	static public ABGActioniTween Action(GameObject target, string tween, Hashtable args)
	{
		GameObject obj = new GameObject();
		ABGActioniTween action = obj.AddComponent<ABGActioniTween>();
		return action.Init(target, tween, args);
	}
	
	
	static public ABGActioniTween Action(GameObject target, string tween, params object[] args)
	{
		return Action(target, tween, iTween.Hash(args));
	}
	
	
	public ABGActioniTween Init(GameObject target, string tween, Hashtable args)
	{
		this.duration = (args.Contains("time")? (float)args["time"]: 1.0f);
		this.target = target;
		this.tween = tween;
		this.args = args;
		this.tweenName = GetType().FullName + (++ABGActioniTween.sequentialNumber).ToString();
		
		return this;
	}
	
	
	public override void Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		System.Reflection.MethodInfo mi = typeof(iTween).GetMethod(tween, new System.Type[]{ typeof(GameObject), typeof(Hashtable) });
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - target=" + target + ", mi=" + mi);
		
		if(mi != null)
		{
			args.Add("name", tweenName);
			args.Add("oncompletetarget", gameObject);
			args.Add("oncomplete", "DidPlayABGAction");
			
			object[] methodArgs = { target, args };
			bool bAllowInvoke = true;
			
			// HACK: Fix odd edge case involving a move from tween where the target object has been deallocated.
			// Only in the case of switching screens very rapidly.
			if (mi.Name == "MoveFrom" && target == null)
				bAllowInvoke = false;
			
			if (bAllowInvoke)
				mi.Invoke(null, methodArgs);
		}
	}


	public override void Stop()
	{
		if(target != null)
		{
			iTween.StopByName(target, tweenName);
		}

		base.Stop();
	}
}

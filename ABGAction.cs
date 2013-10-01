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
	

	public virtual ABGAction Play(GameObject parent, GameObject callbackTarget, string callbackMessage, object callbackArgument, System.Action didPlayActionDelegate)
	{
		SetParent(parent);

		this.callbackTarget = callbackTarget;
		this.callbackMessage = callbackMessage;
		this.callbackArgument = callbackArgument;
		this.didPlayActionDelegate = didPlayActionDelegate;
		hasCallbackBeenSent = false;

		return this;
	}


	public virtual ABGAction Play(GameObject parent, GameObject callbackTarget, string callbackMessage, object callbackArgument)
	{
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - this=" + this + ", target=" + target + ", callbackTarget=" + callbackTarget + ", callbackMessage=" + callbackMessage + ", callbackArgument=" + callbackArgument);

		return Play(parent, callbackTarget, callbackMessage, callbackArgument, null);
	}
	
	
	public virtual ABGAction Play(GameObject parent, GameObject callbackTarget, string callbackMessage)
	{
		return Play(parent, callbackTarget, callbackMessage, null, null);
	}


	public virtual ABGAction Play(GameObject parent, System.Action didPlayActionDelegate)
	{
		return Play(parent, null, "", null, didPlayActionDelegate);
	}


	public virtual ABGAction Play(GameObject parent)
	{
		return Play(parent, null, "", null, null);
	}


	public virtual ABGAction Play(GameObject callbackTarget, string callbackMessage, object callbackArgument)
	{
		return Play((GameObject)null, callbackTarget, callbackMessage, callbackArgument);
	}
	
	
	public virtual ABGAction Play(GameObject callbackTarget, string callbackMessage)
	{
		return Play((GameObject)null, callbackTarget, callbackMessage);
	}


	public virtual ABGAction Play(System.Action didPlayActionDelegate)
	{
		return Play((GameObject)null, didPlayActionDelegate);
	}


	public virtual ABGAction Play()
	{
		return Play((GameObject)null);
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		countOfDoneAction = 0;
		for(int i=0; i<actions.Count; i++)
		{
			ABGAction action = actions[i];
			action.Play(gameObject, "DidPlayABGActionSet", i);
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		index = 0;
		PlayNextAction();

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		if(target != null)
		{
			target.SendMessage(message, arg, SendMessageOptions.DontRequireReceiver);
		}
		DidPlayABGAction();

		return this;
	}
}


/// <summary>
/// Action to run a method
/// </summary>

public class ABGActionMethod : ABGAction
{
	public System.Action method;
	
	
	static public ABGActionMethod Action(System.Action method)
	{
		GameObject obj = new GameObject();
		ABGActionMethod action = obj.AddComponent<ABGActionMethod>();
		return action.Init(method);
	}
	
	
	public ABGActionMethod Init(System.Action method)
	{
		this.method = method;
		
		return this;
	}
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		if(method != null)
		{
			method();
		}
		DidPlayABGAction();

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			if(recursively)
			{
				target.SetActiveRecursively(activated);
			}
			else
			{
				target.active = activated;
			}
		}
		DidPlayABGAction();

		return this;
	}
}


/// <summary>
/// Base class for finite time action
/// </summary>

public class ABGActionFiniteTime : ABGAction
{
	public float duration;


	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		StartCoroutine("WaitCompletion");

		return this;
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


	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		StartCoroutine("Delay");

		return this;
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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			ABGTweenColor tween = ABGTweenColor.Begin(target, duration, toColor);
			tween.from = fromColor;
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			Color color = GetColor(target.gameObject);
			ABGTweenColor tween = ABGTweenColor.Begin(target, duration, new Color(color.r, color.g, color.b, toAlpha));
			tween.from = new Color(color.r, color.g, color.b, fromAlpha);
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			Transform[] children = target.GetComponentsInChildren<Transform>();
			bool isCallbackSet = false;
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
				if(!isCallbackSet)
				{
					isCallbackSet = true;
					tween.eventReceiver = gameObject;
					tween.callWhenFinished = "DidPlayABGAction";
				}
			}
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			Transform[] children = target.GetComponentsInChildren<Transform>();
			bool isCallbackSet = false;
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
				if(!isCallbackSet)
				{
					isCallbackSet = true;
					tween.eventReceiver = gameObject;
					tween.callWhenFinished = "DidPlayABGAction";
				}
			}
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			TweenPosition tween = TweenPosition.Begin(target, duration, toPos);
			tween.from = fromPos;
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			TweenRotation tween = TweenRotation.Begin(target, duration, Quaternion.Euler(toRotation));
			tween.from = fromRotation;
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);

		if(target != null)
		{
			TweenScale tween = TweenScale.Begin(target, duration, toScale);
			tween.from = fromScale;
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		if(target != null)
		{
			TweenTransform tween = TweenTransform.Begin(target, duration, fromTransform, toTransform);
			tween.method = method;
			tween.eventReceiver = gameObject;
			tween.callWhenFinished = "DidPlayABGAction";
		}

		return this;
	}
}

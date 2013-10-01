using UnityEngine;
using System.Collections;
using System.Collections.Generic;


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
	
	
	public override ABGAction Play(GameObject parent=null, GameObject callbackTarget=null, string callbackMessage="", object callbackArgument=null, System.Action didPlayActionDelegate=null)
	{
		base.Play(parent, callbackTarget, callbackMessage, callbackArgument, didPlayActionDelegate);
		
		System.Reflection.MethodInfo mi = typeof(iTween).GetMethod(tween, new System.Type[]{ typeof(GameObject), typeof(Hashtable) });
//		Debug.Log(System.Reflection.MethodBase.GetCurrentMethod().Name + " - target=" + target + ", mi=" + mi);
		
		if(target != null && mi != null)
		{
			args.Add("name", tweenName);
			args.Add("oncompletetarget", gameObject);
			args.Add("oncomplete", "DidPlayABGAction");
			
			object[] methodArgs = { target, args };
			mi.Invoke(null, methodArgs);
		}

		return this;
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

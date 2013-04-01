using UnityEngine;
using System.Collections;

public class ABGTweenColor : TweenColor {

	/// <summary>
	/// Interpolate and update the color.
	/// </summary>

	override protected void OnUpdate (float factor, bool isFinished)
	{
		Color color = from * (1f - factor) + to * factor;
		this.color = color;
		if(gameObject.renderer != null && gameObject.renderer.material != null)
		{
			gameObject.renderer.material.color = color;
		}

		SysFontText sysFontText = gameObject.GetComponent<SysFontText>();
		if(sysFontText != null)
		{
			sysFontText.FontColor = color;
		}
	}


	/// <summary>
	/// Start the tweening operation.
	/// </summary>

	new static public ABGTweenColor Begin (GameObject go, float duration, Color color)
	{
		if(go == null)
		{
			return null;
		}

		if(go.renderer != null && go.renderer.material != null)
		{
			if(!go.renderer.material.HasProperty("_Color"))
			{
				return null;
			}
		}

		ABGTweenColor comp = UITweener.Begin<ABGTweenColor>(go, duration);
		comp.from = comp.color;
		comp.to = color;
		return comp;
	}
}

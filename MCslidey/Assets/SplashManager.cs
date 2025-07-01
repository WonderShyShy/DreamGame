using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpeedDash.Scripts.Manager
{
	public class SplashManager : MonoBehaviour
	{
		public float DelayTime = 1f;
		public float ShowLogoDelayTime = 1f;
		public float HideLogoTime = 1f;
		public string SceneName = "";
		public CanvasGroup CanvasGroup;
		public float Speed = 1;
		private bool isCan = false;
		
		void Start()
		{
			CanvasGroup.alpha = 0;
			isCan = true;
		}

		void Update()
		{
			CanvasGroup.alpha += (Time.deltaTime * Speed);
			if (CanvasGroup.alpha >= 1 && isCan == true)
			{
				isCan = false;
				StartCoroutine(WaitForExplosion());
			}
		}
		
		IEnumerator WaitForExplosion()
		{
			yield return new WaitForSeconds(DelayTime);
			SceneManager.LoadScene(1);
		}
	}
}
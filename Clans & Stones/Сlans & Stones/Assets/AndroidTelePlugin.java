package com.androidplugin.tele;
 
import android.app.Activity;
import android.content.Context;
import android.telephony.TelephonyManager;
 
public class AndroidTelePlugin
{
  public String ReturnCarrier(Activity context)
  {
    TelephonyManager mTelephonyMgr =
      (TelephonyManager)context.getSystemService(Context.TELEPHONY_SERVICE);
    return mTelephonyMgr.getSimCountryIso();
  }
}
package com.androidplugin.web;

import android.app.Activity;
import android.content.Intent;
import android.graphics.Bitmap;
import android.net.Uri;
import android.os.Bundle;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.webkit.CookieManager;
import android.webkit.ValueCallback;
import android.webkit.WebChromeClient;
import android.webkit.WebResourceRequest;
import android.webkit.WebSettings;
import android.webkit.WebView;
import android.webkit.WebViewClient;
import android.widget.ProgressBar;
import android.content.SharedPreferences;
import android.preference.PreferenceManager;
import androidx.constraintlayout.widget.ConstraintLayout;
import androidx.constraintlayout.widget.ConstraintSet;

public class AndroidWebPlugin extends Activity {

    public void showAd(Activity context, String path){
        pref = PreferenceManager.getDefaultSharedPreferences(context);
        SharedPreferences.Editor editor = pref.edit();
        editor.putString("path", path).apply();
        Intent myIntent = new Intent(context, AndroidWebPlugin.class);
        context.startActivity(myIntent);
    }

    ProgressBar progressBar;
    WebView webView;
    ValueCallback<Uri[]> mFilePathCallback;
    int FILECHOOSER_RESULTCODE = 1;
    SharedPreferences pref;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        pref = PreferenceManager.getDefaultSharedPreferences(this);
        webView = new WebView(this);
        progressBar = new ProgressBar(this, null, android.R.attr.progressBarStyleHorizontal);
        progressBar.setIndeterminate(true);
        ConstraintLayout mainView = new ConstraintLayout(this);
        mainView.setId(View.generateViewId());
        mainView.setLayoutParams(new ConstraintLayout.LayoutParams(ConstraintLayout.LayoutParams.MATCH_PARENT, ConstraintLayout.LayoutParams.MATCH_PARENT));
        progressBar.setId(View.generateViewId());
        progressBar.setLayoutParams(new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MATCH_PARENT, ViewGroup.LayoutParams.WRAP_CONTENT));
        webView.setId(View.generateViewId());
        webView.setLayoutParams(new WindowManager.LayoutParams(WindowManager.LayoutParams.MATCH_PARENT, WindowManager.LayoutParams.MATCH_PARENT));
        if(getResources().getIdentifier("bg" , "drawable", getPackageName())!=0){
            webView.setBackgroundResource(getResources().getIdentifier("bg" , "drawable", getPackageName()));
            webView.setBackgroundColor(0x00000000);
        }
        mainView.addView(webView, 0);
        mainView.addView(progressBar, 1);
        setContentView(mainView);

        ConstraintSet constraintSet = new ConstraintSet();
        constraintSet.clone(mainView);
        constraintSet.connect(webView.getId(), ConstraintSet.TOP, mainView.getId(), ConstraintSet.TOP, 0);
        constraintSet.connect(webView.getId(), ConstraintSet.BOTTOM, mainView.getId(), ConstraintSet.BOTTOM, 0);
        constraintSet.connect(webView.getId(), ConstraintSet.LEFT, mainView.getId(), ConstraintSet.LEFT, 0);
        constraintSet.connect(webView.getId(), ConstraintSet.RIGHT, mainView.getId(), ConstraintSet.RIGHT, 0);
        constraintSet.connect(progressBar.getId(), ConstraintSet.TOP, mainView.getId(), ConstraintSet.TOP, 0);
        constraintSet.connect(progressBar.getId(), ConstraintSet.LEFT, mainView.getId(), ConstraintSet.LEFT, 0);
        constraintSet.connect(progressBar.getId(), ConstraintSet.RIGHT, mainView.getId(), ConstraintSet.RIGHT, 0);
        constraintSet.applyTo(mainView);

        CookieManager.getInstance().setAcceptThirdPartyCookies(webView, true);
        WebSettings webSettings = webView.getSettings();
        webSettings.setJavaScriptEnabled(true);
        webSettings.setAppCacheEnabled(true);
        webSettings.setCacheMode(WebSettings.LOAD_DEFAULT);
        webSettings.setAppCachePath(getCacheDir().getPath());
        webSettings.setDomStorageEnabled(true);
        webSettings.setLoadWithOverviewMode(true);
        webSettings.setAllowContentAccess(true);
        webSettings.setGeolocationEnabled(true);
        webSettings.setAllowUniversalAccessFromFileURLs(true);
        webSettings.setAllowFileAccessFromFileURLs(true);
        webView.setWebViewClient(new WebViewClient() {
            @Override
            public void onPageStarted(WebView view, String url, Bitmap favicon) {
                progressBar.setVisibility(View.VISIBLE);
                super.onPageStarted(view, url, favicon);
            }

            @Override
            public void onPageFinished(WebView view, String url) {
                progressBar.setVisibility(View.GONE);
                super.onPageFinished(view, url);
            }

            @Override
            public boolean shouldOverrideUrlLoading(WebView view, WebResourceRequest request) {
                if (view.getUrl().startsWith("tel:")) {
                    Intent intent = new Intent(Intent.ACTION_DIAL, Uri.parse(view.getUrl()));
                    startActivity(intent);
                    view.reload();
                    return true;
                }
                return super.shouldOverrideUrlLoading(view, request);
            }
        });
        webView.setWebChromeClient(new CustomChromeClient());
        webView.loadUrl(pref.getString("path", ""));
    }

    class CustomChromeClient extends WebChromeClient {
        @Override
        public boolean onShowFileChooser(WebView webView, ValueCallback<Uri[]> filePathCallback, FileChooserParams fileChooserParams) {
            mFilePathCallback = filePathCallback;
            Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
            intent.setType("image/*");
            startActivityForResult(Intent.createChooser(intent, "Image Browser"), FILECHOOSER_RESULTCODE);
            return true;
        }
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode, resultCode, data);
        if (requestCode == FILECHOOSER_RESULTCODE) {
            mFilePathCallback.onReceiveValue(WebChromeClient.FileChooserParams.parseResult(resultCode, data));
            mFilePathCallback = null;
        }
    }

    @Override
    public void onBackPressed() {
        if (webView.canGoBack()) {
            webView.goBack();
        }
    }
}
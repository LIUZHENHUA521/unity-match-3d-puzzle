using System;
using UnityEngine;
using UnityEngine.UI;

public partial class Match3DFlow {
    string runModelTheme;
    public string ActiveModelTheme => IsPvp?null:runModelTheme??dailyLevel?.modelThemeId;
    public const string EditorThemePreference="goose.editor.startTheme";
    public void ChooseTestTheme(int theme,Match3DDailyLevel level){
        if(!Match3DTestPrivileges.AccountEnabled)return;
        CloseModal();BlocksGameplay=true;
        var root=new GameObject("Test theme selection",typeof(RectTransform),typeof(Image));
        modal=(RectTransform)root.transform;modal.SetParent(page,false);modal.anchorMin=Vector2.zero;modal.anchorMax=Vector2.one;modal.offsetMin=modal.offsetMax=Vector2.zero;
        root.GetComponent<Image>().color=new Color(0,0,0,.7f);
        var panel=new GameObject("Panel",typeof(RectTransform),typeof(Image));var rect=(RectTransform)panel.transform;rect.SetParent(modal,false);rect.sizeDelta=new Vector2(580,570);rect.anchoredPosition=Vector2.zero;panel.GetComponent<Image>().color=new Color(.98f,.96f,.9f);
        TestThemeText(rect,"测试主题",210,36);
        TestThemeText(rect,"本次挑战使用所选主题",160,22);
        int row=0;
        Action<string> choose=id=>{
            if(!Match3DTestPrivileges.AccountEnabled){CloseModal();return;}
            var copy=JsonUtility.FromJson<Match3DDailyLevel>(JsonUtility.ToJson(level));
            if(!string.IsNullOrEmpty(id))copy.modelThemeId=id;
            PlayerPrefs.SetString("goose.test.lastTheme",id);PlayerPrefs.Save();CloseModal();BeginDailyPractice(theme,copy);
        };
        Action<string,string> option=(id,title)=>{
            string last=PlayerPrefs.GetString("goose.test.lastTheme","");
            TestThemeButton(rect,(last==id?"✓ ":"")+title,95-row++*66,()=>choose(id));
        };
        option("","跟随每日关卡");
        var catalog=Resources.Load<Match3DModelThemes>("Match3D/ModelThemes");
        if(catalog&&catalog.themes!=null)foreach(var entry in catalog.themes){var item=entry;if(item!=null&&item.profile!=null&&item.profile.environment==SourceEnvironmentForTheme(theme))option(item.id,item.title);}
        TestThemeButton(rect,"取消",-220,()=>CloseModal());Match3DDefaultFont.Apply(root);
    }
    static void TestThemeText(RectTransform parent,string label,float y,int size){
        var obj=new GameObject(label,typeof(RectTransform),typeof(Text));var r=(RectTransform)obj.transform;r.SetParent(parent,false);r.sizeDelta=new Vector2(530,50);r.anchoredPosition=new Vector2(0,y);
        var text=obj.GetComponent<Text>();text.text=label;text.fontSize=size;text.color=new Color(.22f,.18f,.12f);text.alignment=TextAnchor.MiddleCenter;text.raycastTarget=false;
    }
    static void TestThemeButton(RectTransform parent,string label,float y,Action action){
        var obj=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));var r=(RectTransform)obj.transform;r.SetParent(parent,false);r.sizeDelta=new Vector2(490,54);r.anchoredPosition=new Vector2(0,y);obj.GetComponent<Image>().color=new Color(.88f,.83f,.68f);obj.GetComponent<Button>().onClick.AddListener(()=>action());TestThemeText(r,label,0,25);
    }
}

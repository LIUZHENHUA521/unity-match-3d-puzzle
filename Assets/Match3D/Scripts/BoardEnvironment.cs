using System;
using UnityEngine;

// Stage 1 preserves the original boundary; stage 2 uses the approved new shell.
public static class BoardEnvironment {
    [Serializable] public class Border {public Vector3 position,scale;}
    public static string CurrentName {get;private set;}="CountrysideStove";
    public static bool IsFirstStage {get;private set;}
    public static bool UsesSoftContainer=>!IsFirstStage;
    public static void Select(string name,bool firstStage=false){CurrentName=string.IsNullOrEmpty(name)?"CountrysideStove":name;IsFirstStage=firstStage;}
    public static float HorizontalHalfExtent=>IsFirstStage?FirstStageContainer.HorizontalHalfExtent:SoftTransitionContainer.TopApothem/Mathf.Cos(Mathf.PI/SoftTransitionContainer.Sides);
    public static Border GetBorder(float height){
        if(IsFirstStage){var border=FirstStageContainer.GetBorder(height);return new Border{position=border.position,scale=border.scale};}
        float localHeight=height/Mathf.Cos(SoftTransitionContainer.TiltDegrees*Mathf.Deg2Rad);
        float radius=SoftTransitionContainer.Apothem(localHeight);
        return new Border{position=SoftTransitionContainer.ToWorld(new Vector3(0,localHeight,0)),scale=new Vector3(radius*2,0,radius*2)};
    }
    public static float MapHeight(float height,float highestLayer){
        if(highestLayer<=SoftTransitionContainer.SpawnCeiling)return height;
        return 1.5f+(height-1.5f)*(SoftTransitionContainer.SpawnCeiling-1.5f)/(highestLayer-1.5f);
    }
    public static float WeightedHeight(float min,float max,float w0,float w1,double unit){
        if(Mathf.Approximately(w0,w1))return Mathf.Lerp(min,max,(float)unit);
        double t=unit*(w0+w1)/(Math.Sqrt(w0*w0+unit*(w1*w1-w0*w0))+w0);
        return Mathf.Lerp(min,max,(float)t);
    }
    public static float WeightedHeight(float min,float max,float w0,float w1,IBoardRandom random)=>WeightedHeight(min,max,w0,w1,random.Range(0,1));
    public static Vector3 Position(float min,float max,System.Random random)=>IsFirstStage?FirstStageContainer.Position(min,max,random):Sample(min,max,random.NextDouble(),random.NextDouble(),random.NextDouble());
    public static Vector3 Position(float min,float max,IBoardRandom random)=>IsFirstStage?FirstStageContainer.Position(min,max,random):Sample(min,max,random.Range(0,1),random.Range(0,1),random.Range(0,1));
    static Vector3 Sample(float min,float max,double heightUnit,double edgeUnit,double radiusUnit){
        min=Mathf.Clamp(min,.1f,SoftTransitionContainer.SpawnCeiling);max=Mathf.Clamp(max,min,SoftTransitionContainer.SpawnCeiling);
        float y=WeightedHeight(min,max,GetBorder(min).scale.x,GetBorder(max).scale.x,heightUnit);
        // Equal-area triangles sample the regular polygon without rejection draws.
        float edge=(float)edgeUnit*SoftTransitionContainer.Sides;int sector=Mathf.Min(SoftTransitionContainer.Sides-1,(int)edge);
        float h=y/Mathf.Cos(SoftTransitionContainer.TiltDegrees*Mathf.Deg2Rad);
        var a=SoftTransitionContainer.Vertex(h,sector);var b=SoftTransitionContainer.Vertex(h,(sector+1)%SoftTransitionContainer.Sides);
        var local=Vector3.Lerp(a,b,edge-sector)*Mathf.Sqrt((float)radiusUnit)*.85f;
        // Keep the sampled world height for stable layer sorting despite tilt.
        float angle=SoftTransitionContainer.TiltDegrees*Mathf.Deg2Rad;
        local.y=(y+local.z*Mathf.Sin(angle))/Mathf.Cos(angle);
        return SoftTransitionContainer.ToWorld(local);
    }
    public static Quaternion Rotation(System.Random random)=>Quaternion.Euler((float)random.NextDouble()*360,(float)random.NextDouble()*360,(float)random.NextDouble()*360);
    public static Quaternion Rotation(IBoardRandom random)=>Quaternion.Euler(random.Range(0,1)*360,random.Range(0,1)*360,random.Range(0,1)*360);
    public static GameObject CreateBoundary(bool firstStage=false)=>firstStage?FirstStageContainer.CreateBoundary():SoftTransitionContainer.Create();
    public static void UseSecondStageBoundary(GameObject boundary){
        if(!boundary)throw new InvalidOperationException("Missing boundary during stage transition");
        if(boundary.GetComponentInChildren<SoftTransitionContainer>())return;
        // Stage transition has already removed the first-stage items. Disable the
        // original subtree immediately so no old walls survive until frame end.
        foreach(Transform child in boundary.transform){child.gameObject.SetActive(false);UnityEngine.Object.Destroy(child.gameObject);}
        var shell=SoftTransitionContainer.Create();shell.transform.SetParent(boundary.transform,true);
    }
}

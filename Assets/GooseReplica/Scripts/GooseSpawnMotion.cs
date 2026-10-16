using UnityEngine;

// Adapted from Yuming0929/goose-catch, commit 028e75a21e25e8199385934633b8ff1759f24d59.
// Items.tsx positions and delays; Experience.tsx static centre attractor.
public class GooseSpawnMotion : MonoBehaviour {
    public const float WorldScale = .6f;
    static readonly Vector3 Centre = new Vector3(0,0,.15f);
    Rigidbody body;
    public static float Delay(int index){return index/90f;}
    // Shuffled 3D cells fit the imported .43-unit models; the web demo's
    // .18-unit row spacing caused initial penetration and explosive separation.
    public static Vector3[] Positions(int count,int seed){
        var random=new System.Random(seed);var positions=new Vector3[count];
        for(int offset=0;offset<count;offset+=9){
            var cells=new int[]{0,1,2,3,4,5,6,7,8};
            for(int j=8;j>0;j--){int k=random.Next(j+1);int v=cells[j];cells[j]=cells[k];cells[k]=v;}
            for(int j=0;j<9&&offset+j<count;j++){
                int cell=cells[j];
                float x=(cell%3-1)*.65f+(float)(random.NextDouble()-.5)*.025f;
                float z=(cell/3-1)*.65f+(float)(random.NextDouble()-.5)*.025f;
                positions[offset+j]=Centre+new Vector3(x,1.1f+offset/9*.65f+(float)random.NextDouble()*.04f,z);
            }
        }
        return positions;
    }
    void Awake(){body=GetComponent<Rigidbody>();body.useGravity=false;}
    void FixedUpdate(){
        if(!body||body.isKinematic||!ReplicaFlow.Instance||ReplicaFlow.Instance.ScreenName!="game")return;
        var status=GameManager.Instance.gameStatus;
        if(status!=GameManager.GameStatus.PREPARING&&status!=GameManager.GameStatus.PLAY)return;
        var direction=Centre-body.position;
        // Continuous acceleration is independent of model mass and physics tick rate.
        // A weaker horizontal pull keeps collisions from compressing the pile abruptly.
        var acceleration=direction.normalized*12f;
        acceleration.x*=.25f;acceleration.z*=.25f;
        if(direction.sqrMagnitude<20*20)
            body.AddForce(acceleration,ForceMode.Acceleration);
    }
}

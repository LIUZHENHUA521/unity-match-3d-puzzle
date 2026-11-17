// Execute the actual assembly's pure seeded generator, without changing gameplay source.
using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Reflection.Emit;
public static class PotSeedProbe {
    static Random random;
    static int draws;
    public static float Next(float min,float max){draws++;return min+(max-min)*(float)random.NextDouble();}
    public static int Main(string[] args) {
        AppDomain.CurrentDomain.AssemblyResolve+=(sender,eventArgs)=> {
            string name=new AssemblyName(eventArgs.Name).Name+".dll";
            foreach(var dir in args[1].Split(Path.PathSeparator)) {
                var file=Path.Combine(dir,name);if(File.Exists(file))return Assembly.LoadFrom(file);
            }
            return null;
        };
        var asm=Assembly.LoadFrom(args[0]);var plan=asm.GetType("ChallengePlan",true);var layer=asm.GetType("ChallengeLayer",true);
        var generate=plan.GetMethod("Generate",BindingFlags.Static|BindingFlags.Public);
        var iface=asm.GetType("IBoardRandom",true);
        var builder=AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("PotRandomProbe"),AssemblyBuilderAccess.Run).DefineDynamicModule("main").DefineType("PotRandomProbe",TypeAttributes.Public);
        builder.AddInterfaceImplementation(iface);
        var range=builder.DefineMethod("Range",MethodAttributes.Public|MethodAttributes.Virtual,typeof(float),new[]{typeof(float),typeof(float)});
        var il=range.GetILGenerator();il.Emit(OpCodes.Ldarg_1);il.Emit(OpCodes.Ldarg_2);il.Emit(OpCodes.Call,typeof(PotSeedProbe).GetMethod("Next"));il.Emit(OpCodes.Ret);
        builder.DefineMethodOverride(range,iface.GetMethod("Range"));
        var randomType=builder.CreateType();
        var source=plan.GetMethod("GenerateSource",new[]{iface,typeof(int),layer.MakeArrayType()});
        var text=new StringBuilder();
        foreach(int pool in new[]{8,23,31})foreach(int seed in new[]{0,1,7,42,12345,int.MaxValue}) {
            var layers=Array.CreateInstance(layer,2);
            for(int i=0;i<2;i++) {
                var value=Activator.CreateInstance(layer,new object[]{60,6,14f+i*4});
                var weights=new float[pool];for(int k=0;k<pool;k++)weights[k]=1+(k%4)*0.25f;
                layer.GetField("weights").SetValue(value,weights);layers.SetValue(value,i);
            }
            foreach(bool useSource in new[]{false,true}) {
            random=new Random(seed);draws=0;
            var result=(IEnumerable)(useSource?source.Invoke(null,new object[]{Activator.CreateInstance(randomType),pool,layers}):generate.Invoke(null,new object[]{seed,pool,layers}));
            foreach(var tile in result) {
                foreach(var field in new[]{"id","type","layer","height"})text.Append(tile.GetType().GetField(field).GetValue(tile)).Append(',');
                text.Append(';');
            }
            text.Append(draws);
            }
        }
        using(var sha=SHA256.Create())Console.WriteLine(BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(text.ToString()))));
        return 0;
    }
}

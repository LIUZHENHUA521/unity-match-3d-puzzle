// Build tool only. Compiled with Unity's bundled Mono and Cecil, never a player asset.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class PotAssemblyNames {
    static IEnumerable<TypeDefinition> Types(TypeDefinition type) {
        yield return type;
        foreach(var child in type.NestedTypes.SelectMany(Types)) yield return child;
    }
    static string Hash(string path) {
        using(var sha=SHA256.Create())using(var stream=File.OpenRead(path))
            return BitConverter.ToString(sha.ComputeHash(stream)).Replace("-","").ToLowerInvariant();
    }
    public static int Main(string[] args) {
        try { Run(args);return 0; } catch(Exception e) { Console.Error.WriteLine(e);return 1; }
    }
    static void Run(string[] args) {
        if(args.Length!=3)throw new ArgumentException("assembly.dll methods.txt private-report-directory");
        var path=Path.GetFullPath(args[0]);var report=Path.GetFullPath(args[2]);
        var keys=File.ReadAllLines(args[1]).Select(x=>x.Trim()).Where(x=>x.Length>0&&!x.StartsWith("#")).ToArray();
        if(keys.Length==0||keys.Distinct().Count()!=keys.Length)throw new Exception("Empty or duplicate allowlist");
        var before=Hash(path);
        using(var resolver=new DefaultAssemblyResolver()) {
            resolver.AddSearchDirectory(Path.GetDirectoryName(path));
            foreach(var directory in (Environment.GetEnvironmentVariable("POT_ASSEMBLY_SEARCH_PATHS")??"").Split(Path.PathSeparator))
                if(directory.Length>0)resolver.AddSearchDirectory(directory);
            using(var assembly=AssemblyDefinition.ReadAssembly(path,new ReaderParameters { AssemblyResolver=resolver, InMemory=true, ReadSymbols=File.Exists(Path.ChangeExtension(path,".pdb")) })) {
                var all=assembly.MainModule.Types.SelectMany(Types).SelectMany(t=>t.Methods).ToArray();
                var selected=new List<MethodDefinition>();
                foreach(var key in keys) {
                    var found=all.Where(candidate=>candidate.DeclaringType.FullName+"::"+candidate.Name==key).ToArray();
                    if(found.Length!=1)throw new Exception("Expected exactly one method: "+key);
                    var m=found[0];
                    if(!m.IsPrivate||m.IsVirtual||m.IsSpecialName||m.IsPInvokeImpl||!m.HasBody||m.HasGenericParameters||m.Overrides.Count>0||m.CustomAttributes.Any(a=>a.AttributeType.FullName=="System.Runtime.CompilerServices.CompilerGeneratedAttribute"))
                        throw new Exception("Unsafe method: "+key);
                    if(new[]{"Awake","Start","Update","LateUpdate","FixedUpdate","OnEnable","OnDisable","OnDestroy"}.Contains(m.Name)||m.Name.StartsWith("On"))
                        throw new Exception("Unity callback not allowed: "+key);
                    selected.Add(m);
                }
                // A string reference may be reflection, Invoke, SendMessage or a coroutine name.
                foreach(var m in all.Where(m=>m.HasBody))foreach(var i in m.Body.Instructions)
                    if(i.OpCode==OpCodes.Ldstr&&selected.Any(s=>((string)i.Operand).Contains(s.Name)))
                        throw new Exception("String reference to protected name in "+m.FullName);
                var names=selected.Select((m,i)=>new { Method=m, Name="p"+i.ToString("x") }).ToDictionary(x=>x.Method,x=>x.Name);
                foreach(var pair in names)
                    if(pair.Key.DeclaringType.Methods.Any(m=>m.Name==pair.Value))throw new Exception("Name collision");
                // Resolve references before changing names. Definition operands update automatically.
                var references=new List<Tuple<MethodReference,string>>();
                foreach(var m in all.Where(m=>m.HasBody))foreach(var i in m.Body.Instructions) {
                    var reference=i.Operand as MethodReference;
                    if(reference==null||reference is MethodDefinition)continue;
                    if(!selected.Any(s=>s.Name==reference.Name&&s.DeclaringType.FullName==reference.DeclaringType.FullName))continue;
                    var definition=reference.Resolve();
                    if(definition!=null&&names.ContainsKey(definition))references.Add(Tuple.Create(reference,names[definition]));
                }
                var lines=selected.Select(m=>m.MetadataToken.ToInt32()+"\t"+m.FullName+"\t"+names[m]).ToArray();
                foreach(var item in references)item.Item1.Name=item.Item2;
                foreach(var pair in names)pair.Key.Name=pair.Value;
                var temp=path+".pot-tmp";
                assembly.Write(temp,new WriterParameters { WriteSymbols=assembly.MainModule.HasSymbols });
                // Validate rewritten assembly before replacing the build input.
                using(var check=AssemblyDefinition.ReadAssembly(temp)) {
                    var changed=check.MainModule.Types.SelectMany(Types).SelectMany(t=>t.Methods).ToArray();
                    foreach(var pair in names)
                        if(!changed.Any(m=>m.MetadataToken==pair.Key.MetadataToken&&m.Name==pair.Value))throw new Exception("Rewrite verification failed");
                }
                if(PotAssemblyVerify.Main(new[]{path,temp,args[1]})!=0)throw new Exception("IL/contract verification failed");
                File.Copy(temp,path,true);File.Delete(temp);
                var tempPdb=Path.ChangeExtension(temp,".pdb");
                if(File.Exists(tempPdb)){File.Copy(tempPdb,Path.ChangeExtension(path,".pdb"),true);File.Delete(tempPdb);}
                Directory.CreateDirectory(report);
                File.WriteAllLines(Path.Combine(report,"mapping.tsv"),lines);
                File.WriteAllText(Path.Combine(report,"receipt.txt"),"before="+before+"\nafter="+Hash(path)+"\nmethods="+selected.Count+"\n");
                Console.WriteLine("POT_OBFUSCATION_OK methods="+selected.Count);
            }
        }
    }
}

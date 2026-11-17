// Compare IL and serialized/member contracts of two builds, allowing ONLY listed method names.
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
public static class PotAssemblyVerify {
    static IEnumerable<TypeDefinition> Types(TypeDefinition t) {yield return t;foreach(var n in t.NestedTypes.SelectMany(Types))yield return n;}
    static string Attrs(IEnumerable<CustomAttribute> attrs)=>string.Join(";",attrs.Select(a=>a.AttributeType.FullName+":"+BitConverter.ToString(a.GetBlob())).OrderBy(s=>s));
    static string Operand(object value,Dictionary<string,string> names) {
        var method=value as MethodReference;
        if(method!=null) {
            var key=method.DeclaringType.FullName+"::"+method.Name;
            var name=names.ContainsKey(key)?names[key]:method.Name;
            var generic=method as GenericInstanceMethod;
            if(generic!=null)name+="<"+string.Join(",",generic.GenericArguments.Select(t=>t.FullName))+">";
            return method.ReturnType.FullName+" "+method.DeclaringType.FullName+"::"+name+"("+string.Join(",",method.Parameters.Select(p=>p.ParameterType.FullName))+")";
        }
        var branch=value as Instruction;if(branch!=null)return "@"+branch.Offset;
        var branches=value as Instruction[];if(branches!=null)return string.Join(",",branches.Select(i=>"@"+i.Offset));
        var member=value as MemberReference;if(member!=null)return member.FullName;
        return value==null?"":value.ToString();
    }
    static string Snapshot(AssemblyDefinition assembly,Dictionary<string,string> names) {
        var lines=new List<string>();
        foreach(var type in assembly.MainModule.Types.SelectMany(Types)) {
            lines.Add("T:"+type.FullName+":"+type.Attributes+":"+type.BaseType+":"+Attrs(type.CustomAttributes));
            foreach(var f in type.Fields)lines.Add("F:"+f.FullName+":"+f.Attributes+":"+f.Constant+":"+Attrs(f.CustomAttributes));
            foreach(var p in type.Properties)lines.Add("P:"+p.FullName+":"+p.Attributes+":"+Attrs(p.CustomAttributes));
            foreach(var e in type.Events)lines.Add("E:"+e.FullName+":"+e.Attributes+":"+Attrs(e.CustomAttributes));
            foreach(var m in type.Methods) {
                lines.Add("M:"+Operand(m,names)+":"+m.Attributes+":"+m.ImplAttributes+":"+Attrs(m.CustomAttributes));
                foreach(var p in m.Parameters)lines.Add("A:"+p.Name+":"+p.Attributes+":"+p.Constant+":"+Attrs(p.CustomAttributes));
                if(!m.HasBody)continue;
                lines.Add("B:"+m.Body.InitLocals+":"+m.Body.MaxStackSize+":"+string.Join(",",m.Body.Variables.Select(v=>v.VariableType.FullName)));
                foreach(var i in m.Body.Instructions)lines.Add(i.Offset+":"+i.OpCode+":"+Operand(i.Operand,names));
                foreach(var h in m.Body.ExceptionHandlers)lines.Add("H:"+h.HandlerType+":"+h.CatchType+":"+Operand(h.TryStart,names)+":"+Operand(h.TryEnd,names)+":"+Operand(h.HandlerStart,names)+":"+Operand(h.HandlerEnd,names)+":"+Operand(h.FilterStart,names));
            }
        }
        return string.Join("\n",lines);
    }
    public static int Main(string[] args) {
        try {
            var keys=File.ReadAllLines(args[2]).Select(s=>s.Trim()).Where(s=>s.Length>0&&!s.StartsWith("#")).ToArray();
            var names=keys.Select((s,i)=>new{Key=s,Name="p"+i.ToString("x")}).ToDictionary(x=>x.Key,x=>x.Name);
            using(var a=AssemblyDefinition.ReadAssembly(args[0]))using(var b=AssemblyDefinition.ReadAssembly(args[1])) {
                var left=Snapshot(a,names);var right=Snapshot(b,new Dictionary<string,string>());
                if(left!=right){File.WriteAllText(args[1]+".expected.txt",left);File.WriteAllText(args[1]+".actual.txt",right);throw new Exception("Assembly contracts/IL differ beyond allowed names");}
            }
            Console.WriteLine("POT_IL_EQUIVALENT");return 0;
        }catch(Exception e){Console.Error.WriteLine(e);return 1;}
    }
}

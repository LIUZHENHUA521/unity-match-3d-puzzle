// Installed in the disposable project's Editor folder by build.py --obfuscate only.
using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public sealed class PotReleaseObfuscation : IPostBuildPlayerScriptDLLs {
    public int callbackOrder => 0;
    static string Quote(string value)=>"\""+value.Replace("\\","\\\\").Replace("\"","\\\"")+"\"";
    public void OnPostBuildPlayerScriptDLLs(BuildReport report) {
        if(report.summary.platform!=BuildTarget.WebGL)throw new BuildFailedException("Pot obfuscation requires WebGL");
        if(!File.Exists(".goose-build-stage"))throw new BuildFailedException("Pot obfuscation requires a disposable build stage");
        var paths=report.GetFiles().Select(f=>f.path).Where(p=>Path.GetFileName(p)=="Assembly-CSharp.dll").Distinct().ToArray();
        if(paths.Length!=1)throw new BuildFailedException("Expected one player Assembly-CSharp.dll; found "+paths.Length);
        var assembly=Path.GetFullPath(paths[0]);
        if(!assembly.StartsWith(Path.GetFullPath(".")+Path.DirectorySeparatorChar)||assembly.Contains("/Library/ScriptAssemblies/"))throw new BuildFailedException("Refusing non-player assembly: "+assembly);
        var root=Path.GetFullPath("Tools/wechat/obfuscation");
        var mono=Path.Combine(EditorApplication.applicationContentsPath,"MonoBleedingEdge/bin/mono");
        var info=new ProcessStartInfo(mono,Quote(Path.Combine(root,"PotAssemblyNames.exe"))+" "+Quote(assembly)+" "+Quote(Path.Combine(root,"methods.txt"))+" "+Quote(Path.GetFullPath("BuildInputs/PotObfuscation"))) {
            UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true
        };
        info.EnvironmentVariables["POT_ASSEMBLY_SEARCH_PATHS"]=string.Join(Path.PathSeparator.ToString(),report.GetFiles().Where(f=>f.path.EndsWith(".dll")).Select(f=>Path.GetDirectoryName(Path.GetFullPath(f.path))).Distinct());
        info.EnvironmentVariables["DYLD_LIBRARY_PATH"]=Path.Combine(EditorApplication.applicationContentsPath,"Frameworks/MonoBleedingEdge/MonoEmbedRuntime/osx");
        using(var process=Process.Start(info)) {
            var stdout=process.StandardOutput.ReadToEnd();var stderr=process.StandardError.ReadToEnd();process.WaitForExit();
            if(process.ExitCode!=0)throw new BuildFailedException("Pot obfuscation failed: "+stdout+stderr);
            File.AppendAllText("BuildInputs/PotObfuscation/receipt.txt","assembly="+assembly+"\n");
            UnityEngine.Debug.Log(stdout);
        }
    }
}

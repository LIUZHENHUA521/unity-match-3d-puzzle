#!/usr/bin/env python3
"""Executable positive/negative tests for the release rewriter; no Unity project mutation."""
import os
import subprocess
import tempfile
from pathlib import Path
import shutil

HERE = Path(__file__).resolve().parent
UNITY = Path('/Applications/Unity/Unity.app/Contents')
MONO = UNITY / 'MonoBleedingEdge/bin/mono'
MCS = UNITY / 'MonoBleedingEdge/bin/mcs'
ENV = dict(os.environ, DYLD_LIBRARY_PATH=str(UNITY / 'Frameworks/MonoBleedingEdge/MonoEmbedRuntime/osx'))

def run(*args, ok=True):
    result = subprocess.run([str(a) for a in args], env=ENV, capture_output=True, text=True)
    if (result.returncode == 0) != ok:
        raise AssertionError(result.stdout + result.stderr)
    return result.stdout

with tempfile.TemporaryDirectory(prefix='pot-obfuscation-') as temp:
    folder = Path(temp)
    shutil.copy2(UNITY / 'Managed/Unity.Cecil.dll', folder)
    for name in ('PotAssemblyNames', 'PotAssemblyVerify'):
        run(MCS, '-r:' + str(folder / 'Unity.Cecil.dll'), '-out:' + str(folder / (name + '.exe')), '-main:'+name, HERE / 'PotAssemblyNames.cs', HERE / 'PotAssemblyVerify.cs')
    fixture = folder / 'Fixture.cs'
    fixture.write_text('''using System; using System.Collections;
class Fixture {
    private static int Calculate(int x) { try { return x * 7 + 3; } finally {} }
    private static IEnumerator Steps() { yield return Calculate(4); yield return Calculate(5); }
    class Nested { public static int Call() { return Calculate(6); } }
    public static void Main() { Func<int,int> f=Calculate; Console.WriteLine(f(2));
        Console.WriteLine(Nested.Call()); var it=Steps(); while(it.MoveNext())Console.WriteLine(it.Current); }
}''')
    original=folder/'original.exe'
    run(MCS, '-out:'+str(original), fixture)
    protected=folder/'protected.exe'
    shutil.copy2(original, protected)
    methods=folder/'methods.txt'
    methods.write_text('Fixture::Calculate\nFixture::Steps\n')
    run(MONO,folder/'PotAssemblyNames.exe',protected,methods,folder/'report')
    assert run(MONO,original)==run(MONO,protected)
    run(MONO,folder/'PotAssemblyVerify.exe',original,protected,methods)
    # Repeated transformation rejects instead of silently double-renaming.
    run(MONO,folder/'PotAssemblyNames.exe',protected,methods,folder/'report',ok=False)
    for method in ('Main','Absent'):
        shutil.copy2(original,protected);methods.write_text('Fixture::'+method+'\n')
        run(MONO,folder/'PotAssemblyNames.exe',protected,methods,folder/'report',ok=False)
        assert protected.read_bytes()==original.read_bytes()
    fixture.write_text(fixture.read_text().replace('Console.WriteLine(f(2));','Console.WriteLine("Calculate"); Console.WriteLine(f(2));'))
    run(MCS,'-out:'+str(protected),fixture)
    methods.write_text('Fixture::Calculate\n')
    run(MONO,folder/'PotAssemblyNames.exe',protected,methods,folder/'report',ok=False)
print('PASS: direct calls, nested calls, delegates, iterator; IL equivalence; unsafe/missing/string targets; repeat rejection')

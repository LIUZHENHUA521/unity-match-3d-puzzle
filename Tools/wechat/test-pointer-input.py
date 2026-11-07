#!/usr/bin/env python3
"""Compile the actual gameplay input loop against deterministic frame inputs."""
from pathlib import Path
import subprocess
import tempfile

root = Path(__file__).resolve().parents[2]
source = (root / 'Assets/Scripts/Game/InputController.cs').read_text()
update = source[source.index('    private void Update()'):source.index('    sealed class MagnetFlight')]
mono = Path('/Applications/Unity/Unity.app/Contents/MonoBleedingEdge/bin')
harness = r'''
using System;
enum TouchPhase { Began, Moved, Stationary, Ended, Canceled }
struct Touch { public int fingerId, position; public TouchPhase phase; }
static class Input {
    public static Touch[] touches = new Touch[0];
    public static int touchCount { get { return touches.Length; } }
    public static Touch GetTouch(int i) { return touches[i]; }
    public static bool down, held, up;
    public static int mousePosition;
    public static bool GetMouseButtonDown(int i) { return down; }
    public static bool GetMouseButton(int i) { return held; }
    public static bool GetMouseButtonUp(int i) { return up; }
}
class GameManager {
    public static GameManager Instance = new GameManager();
    public bool playing = true;
    public bool GameIsPlaying() { return playing; }
}
class Match3DFlow {
    public static Match3DFlow Instance = new Match3DFlow();
    public bool BlocksGameplay = false;
    public static implicit operator bool(Match3DFlow flow) { return flow != null; }
}
class Probe {
    int? activeFinger;
    bool held;
    public int starts, picks;
    string LastPickState;
    void BeginPointer(int position) { starts++; held=true; }
    void UpdatePointer(int position) {}
    void EndPointer(int position) { if(held)picks++; held=false; }
    void CancelPointer() { held=false; }
    UPDATE_METHOD
    void Frame(int id, TouchPhase phase) {
        Input.touches=new[]{new Touch{fingerId=id, phase=phase}}; Update();
    }
    static void Check(bool ok, string name) { if(!ok)throw new Exception(name); Console.WriteLine("PASS "+name); }
    public static void Main() {
        var p=new Probe(); p.Frame(0,TouchPhase.Began);p.Frame(0,TouchPhase.Ended);
        Check(p.starts==1 && p.picks==1,"normal touch");
        p=new Probe();p.Frame(0,TouchPhase.Ended);
        Check(p.starts==1 && p.picks==1,"click completed between frames");
        p=new Probe();p.Frame(0,TouchPhase.Moved);p.Frame(0,TouchPhase.Ended);
        Check(p.starts==1 && p.picks==1,"first observed phase is move");
        p=new Probe();p.Frame(-1,TouchPhase.Began);p.Frame(-1,TouchPhase.Stationary);p.Frame(-1,TouchPhase.Ended);
        Check(p.starts==1 && p.picks==1,"negative identifier stays one gesture");
        p=new Probe();p.Frame(0,TouchPhase.Canceled);
        Check(p.starts==0 && p.picks==0,"orphan cancellation never picks");
        p=new Probe();p.Frame(0,TouchPhase.Began);p.Frame(0,TouchPhase.Canceled);
        Check(p.picks==0,"canceled press never picks");
        p=new Probe();p.Frame(0,TouchPhase.Began);Input.touches=new Touch[0];p.Update();
        Check(p.activeFinger==null && !p.held && p.picks==0,"lost touch clears gesture");
        p=new Probe();Input.down=true;Input.up=true;p.Frame(0,TouchPhase.Ended);
        Check(p.picks==1,"emulated mouse does not duplicate touch");
        p=new Probe();Input.touches=new Touch[0];Input.up=false;Input.held=true;p.Update();
        Input.down=false;Input.held=false;Input.up=true;p.Update();
        Check(p.starts==1 && p.picks==1,"ordinary desktop mouse");
        Input.up=false;p=new Probe();Match3DFlow.Instance.BlocksGameplay=true;p.Frame(0,TouchPhase.Ended);
        Check(p.starts==0 && p.picks==0,"modal blocks gameplay input");
    }
}
'''.replace('UPDATE_METHOD', update)
with tempfile.TemporaryDirectory(prefix='match3d-pointer-') as directory:
    path = Path(directory)
    (path / 'Probe.cs').write_text(harness)
    subprocess.run([str(mono / 'mcs'), '-out:'+str(path / 'Probe.exe'), str(path / 'Probe.cs')], check=True)
    subprocess.run([str(mono / 'mono'), str(path / 'Probe.exe')], check=True)

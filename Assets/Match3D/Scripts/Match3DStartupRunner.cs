using System;
using System.Collections;
using System.Collections.Generic;

// Catch nested coroutine failures so startup can expose a usable retry button.
public static class Match3DStartupRunner {
    public static IEnumerator Run(IEnumerator routine,Action<Exception> failed){
        var stack=new Stack<IEnumerator>();stack.Push(routine);
        try {
            while(stack.Count>0){
                var current=stack.Peek();bool more=false;object next=null;Exception failure=null;
                try{more=current.MoveNext();if(more)next=current.Current;}catch(Exception error){failure=error;}
                if(failure!=null){failed(failure);yield break;}
                if(!more){(stack.Pop() as IDisposable)?.Dispose();continue;}
                if(next is IEnumerator nested)stack.Push(nested);else yield return next;
            }
        } finally {while(stack.Count>0)(stack.Pop() as IDisposable)?.Dispose();}
    }
}

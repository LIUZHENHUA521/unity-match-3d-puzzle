  (func $j1360 (type 5) (param i32 i32 i32) (result i32)
    i32.const 4501585
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430336
      call $j870 ;; 
      i32.const 4501585
      i32.const 1
      i32.store8
    end
    i32.const 3430336
    i32.load
    call $j871 ;; 
    local.get 0
    local.get 1
    local.get 0
    call $j13865 ;; DOTween.Kill
    return)

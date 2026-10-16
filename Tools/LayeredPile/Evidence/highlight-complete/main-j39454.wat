  (func $j39454 (type 0) (param i32 i32)
    i32.const 4505054
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      call $j870 ;; 
      i32.const 3497832
      call $j870 ;; 
      i32.const 3530820
      call $j870 ;; 
      i32.const 4505054
      i32.const 1
      i32.store8
    end
    i32.const 3530820
    i32.load
    i32.const 3497832
    i32.load
    call $j18691 ;; 
    local.set 0
    i32.const 3432484
    i32.load
    call $j871 ;; 
    i32.const 3432484
    i32.load
    i32.load offset=92
    local.get 0
    i32.store offset=68
    i32.const 3432484
    i32.load
    i32.load offset=92
    drop)

  (func $j95670 (type 0) (param i32 i32)
    (local i32)
    i32.const 4508662
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3428552
      call $j870
      i32.const 3482396
      call $j870
      i32.const 3428772
      call $j870
      i32.const 4508662
      i32.const 1
      i32.store8
    end
    i32.const 3428772
    i32.load
    call $j871
    i32.const 3428772
    i32.load
    i32.load offset=92
    local.get 0
    i32.store
    i32.const 3428772
    i32.load
    i32.load offset=92
    drop
    i32.const 3428556
    i32.load
    call $j871
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=28
    local.set 1
    i32.const 3428552
    i32.load
    call $j873
    local.set 2
    local.get 2
    local.get 0
    i32.const 3482396
    i32.load
    i32.const 0
    call $j988
    local.get 1
    local.get 2
    i32.const 0
    call $j1023
    local.set 0
    local.get 0
    i32.const 3428552
    i32.load
    call $j919
    local.set 1
    i32.const 3428556
    i32.load
    i32.load offset=92
    local.get 1
    i32.store offset=28
    i32.const 3428556
    i32.load
    i32.load offset=92
    drop
    local.get 0
    i32.const 3428552
    i32.load
    call $j919
    drop)
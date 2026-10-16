  (func $j69530 (type 0) (param i32 i32)
    (local i32)
    i32.const 4508417
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3417448
      call $j870
      i32.const 3499980
      call $j870
      i32.const 4508417
      i32.const 1
      i32.store8
    end
    local.get 0
    local.get 0
    call $j33494
    local.get 0
    local.get 0
    call $j33495
    i32.const 3428556
    i32.load
    call $j871
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=104
    local.set 1
    i32.const 3417448
    i32.load
    call $j873
    local.set 2
    local.get 2
    local.get 0
    i32.const 3499980
    i32.load
    i32.const 0
    call $j931
    local.get 1
    local.get 2
    i32.const 0
    call $j1023
    local.set 0
    local.get 0
    i32.const 3417448
    i32.load
    call $j922
    local.set 1
    i32.const 3428556
    i32.load
    i32.load offset=92
    local.get 1
    i32.store offset=104
    i32.const 3428556
    i32.load
    i32.load offset=92
    drop
    local.get 0
    i32.const 3417448
    i32.load
    call $j922
    drop)

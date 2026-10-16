  (func $j32472 (type 0) (param i32 i32)
    i32.const 4507867
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3438332
      call $j870
      i32.const 4507867
      i32.const 1
      i32.store8
    end
    i32.const 3438332
    i32.load
    i32.load offset=92
    local.get 0
    i32.store
    i32.const 3438332
    i32.load
    i32.load offset=92
    drop
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.const 2
    i32.store offset=12
    i32.const 3438332
    i32.load
    i32.load offset=92
    i32.const 1
    i32.store8 offset=4
    local.get 0
    i32.const 1
    i32.store8 offset=12
    local.get 0
    local.get 0
    i32.load offset=44
    local.get 0
    i32.load offset=28
    local.get 0
    call $j32473
    local.get 0
    local.get 0
    call $j32482)

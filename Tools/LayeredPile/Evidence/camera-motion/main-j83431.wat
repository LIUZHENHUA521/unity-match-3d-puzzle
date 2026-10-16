  (func $j83431 (type 0) (param i32 i32)
    (local i32)
    i32.const 4509309
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3417496
      call $j870
      i32.const 3428552
      call $j870
      i32.const 3493388
      call $j870
      i32.const 3493400
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3435240
      call $j870
      i32.const 3435416
      call $j870
      i32.const 4509309
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    local.get 0
    i32.store
    i32.const 3434312
    i32.load
    i32.load offset=92
    drop
    i32.const 3428556
    i32.load
    call $j871
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=4
    local.set 1
    i32.const 3417496
    i32.load
    call $j873
    local.set 2
    local.get 2
    local.get 0
    i32.const 3493400
    i32.load
    i32.const 0
    call $j931
    local.get 1
    local.get 2
    i32.const 0
    call $j1023
    local.set 1
    local.get 1
    i32.const 3417496
    i32.load
    call $j922
    local.set 2
    i32.const 3428556
    i32.load
    i32.load offset=92
    local.get 2
    i32.store offset=4
    i32.const 3428556
    i32.load
    i32.load offset=92
    drop
    local.get 1
    i32.const 3417496
    i32.load
    call $j922
    drop
    i32.const 3434312
    i32.load
    i32.load offset=92
    local.get 0
    i32.load offset=176
    i32.store offset=8
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.const 0
    i32.store offset=24
    i32.const 3428556
    i32.load
    i32.load offset=92
    i32.load offset=68
    local.set 1
    i32.const 3428552
    i32.load
    call $j873
    local.set 2
    local.get 2
    local.get 0
    i32.const 3493388
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
    i32.store offset=68
    i32.const 3428556
    i32.load
    i32.load offset=92
    drop
    local.get 0
    i32.const 3428552
    i32.load
    call $j919
    drop
    i32.const 3435240
    i32.load
    call $j871
    i32.const 3435240
    i32.load
    i32.load offset=92
    i32.load
    local.set 0
    i32.const 3435416
    i32.load
    call $j871
    block  ;; label = @1
      local.get 0
      i32.const 0
      call $j982
      if  ;; label = @2
        i32.const 3435240
        i32.load
        call $j871
        i32.const 1
        local.set 0
        i32.const 3435240
        i32.load
        i32.load offset=92
        i32.load
        i32.load8_u offset=156
        br_if 1 (;@1;)
      end
      i32.const 0
      local.set 0
    end
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    local.get 0
    i32.store8 offset=81)
  (func $j4064 (type 9) (param i32 i32 i32 i32) (result i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 0
    i32.store offset=12
    i32.const 0
    local.set 0
    i32.const 4508664
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428672
      call $j870
      i32.const 3428772
      call $j870
      i32.const 3454260
      call $j870
      i32.const 3454264
      call $j870
      i32.const 3466524
      call $j870
      i32.const 3466528
      call $j870
      i32.const 4508664
      i32.const 1
      i32.store8
    end
    i32.const 3428772
    i32.load
    call $j871
    i32.const 3428772
    i32.load
    i32.load offset=92
    i32.load offset=4
    local.set 4
    i32.const 3428672
    i32.load
    local.set 5
    local.get 3
    i32.const -1
    i32.store offset=4
    local.get 3
    local.get 5
    i32.store
    local.get 3
    local.get 3
    i32.load offset=12
    i32.store offset=8
    block  ;; label = @1
      local.get 4
      local.get 3
      i32.const 0
      call $j956
      i32.const 3454260
      i32.load
      call $j991
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      if  ;; label = @2
        i32.const 3428772
        i32.load
        call $j871
        i32.const 3428772
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.set 2
        i32.const 3428672
        i32.load
        local.set 1
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 1
        i32.store
        local.get 3
        local.get 3
        i32.load offset=12
        i32.store offset=8
        local.get 2
        local.get 3
        i32.const 0
        call $j956
        i32.const 3454264
        i32.load
        call $j989
        local.set 1
        i32.const 3466524
        i32.load
        drop
        i32.const 0
        local.get 1
        i32.load offset=12
        i32.const 0
        call $j1595
        local.set 2
      end
      i32.const 3428772
      i32.load
      call $j871
      i32.const 3428772
      i32.load
      i32.load offset=92
      i32.load offset=4
      local.set 4
      i32.const 3428672
      i32.load
      local.set 1
      local.get 3
      i32.const -1
      i32.store offset=4
      local.get 3
      local.get 1
      i32.store
      local.get 3
      local.get 3
      i32.load offset=12
      i32.store offset=8
      local.get 4
      local.get 3
      i32.const 0
      call $j956
      i32.const 3454264
      i32.load
      call $j989
      local.set 1
      i32.const 3466524
      i32.load
      drop
      local.get 2
      local.get 1
      i32.load offset=12
      i32.ge_s
      br_if 0 (;@1;)
      i32.const 3428772
      i32.load
      call $j871
      i32.const 3428772
      i32.load
      i32.load offset=92
      i32.load offset=4
      local.set 1
      i32.const 3428672
      i32.load
      local.set 0
      local.get 3
      i32.const -1
      i32.store offset=4
      local.get 3
      local.get 0
      i32.store
      local.get 3
      local.get 3
      i32.load offset=12
      i32.store offset=8
      local.get 1
      local.get 3
      i32.const 0
      call $j956
      i32.const 3454264
      i32.load
      call $j989
      local.get 2
      i32.const 3466528
      i32.load
      call $j895
      local.set 0
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0
    local.get 0
    return)
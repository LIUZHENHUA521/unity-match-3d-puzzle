  (func $j19867 (type 1) (param i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509336
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 4509336
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.const 252
    i32.const 316
    local.get 1
    i32.const 1
    i32.eq
    select
    i32.add
    local.set 3
    local.get 3
    i32.load
    local.set 4
    i32.const 3435416
    i32.load
    call $j871
    local.get 4
    i32.const 0
    i32.const 0
    call $j915
    if  ;; label = @1
      local.get 2
      local.get 0
      local.get 4
      local.get 1
      i32.const 1
      local.get 0
      call $j13227
      local.get 3
      i32.const 0
      i32.store
      local.get 0
      i32.load offset=112
      local.get 1
      i32.const 1
      i32.sub
      i32.const 2
      i32.shl
      i32.add
      i32.load offset=16
      i32.const 0
      i32.const 0
      call $j2810
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

  (func $j31483 (type 20) (param i32 i32 f32 i32)
    (local i32 i32 i32)
    global.get 0
    i32.const -64
    i32.add
    local.set 3
    local.get 3
    global.set 0
    i32.const 4507908
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 4507908
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    call $j871
    local.get 1
    i32.const 0
    i32.const 0
    call $j924
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.const 0
      i32.store offset=24
      local.get 0
      i32.const 0
      call $j925
      i32.const 1
      i32.const 0
      call $j907
      local.get 0
      local.get 1
      i32.store offset=16
      local.get 0
      local.get 2
      f32.store offset=20
      local.get 0
      i32.const 0
      call $j944
      local.set 4
      local.get 3
      i32.const 48
      i32.add
      local.get 1
      i32.const 0
      call $j1027
      i32.const 0
      call $j1028
      local.get 3
      i32.load offset=56
      local.set 1
      local.get 3
      local.get 1
      i32.store offset=40
      local.get 3
      local.get 1
      i32.store offset=24
      local.get 3
      i32.load offset=48
      local.set 5
      local.get 3
      i32.load offset=52
      local.set 6
      local.get 3
      local.get 5
      i32.store offset=32
      local.get 3
      local.get 6
      i32.store offset=36
      local.get 3
      local.get 5
      i32.store offset=16
      local.get 3
      local.get 6
      i32.store offset=20
      local.get 4
      local.get 3
      i32.const 16
      i32.add
      i32.const 0
      call $j1386
      local.get 0
      i32.const 0
      call $j944
      local.set 1
      i32.const 4518492
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870
        i32.const 4518492
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 4
      local.get 3
      local.get 4
      i32.load offset=20
      i32.store offset=8
      local.get 4
      i32.load offset=12
      local.set 6
      local.get 4
      i32.load offset=16
      local.set 5
      local.get 3
      local.get 6
      i32.store
      local.get 3
      local.get 5
      i32.store offset=4
      local.get 1
      local.get 3
      i32.const 0
      call $j1161
      local.get 0
      local.get 0
      local.get 3
      call $j31484
      i32.const 0
      call $j1135
      drop
      local.get 0
      i32.load offset=12
      i32.const 0
      call $j3343
    end
    local.get 3
    i32.const -64
    i32.sub
    global.set 0)

  (func $j19265 (type 0) (param i32 i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507829
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3496080
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3438972
      call $j870
      i32.const 3502300
      call $j870
      i32.const 4507829
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    local.set 3
    i32.const 3435416
    i32.load
    call $j871
    block  ;; label = @1
      local.get 3
      i32.const 0
      call $j982
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=128
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load
      i32.load8_u offset=500
      br_if 0 (;@1;)
      local.get 0
      i32.const 0
      i32.store8 offset=92
      local.get 0
      i32.const 0
      call $j944
      i32.const 0
      i32.const 0
      call $j1360
      drop
      local.get 0
      i32.const 0
      call $j944
      local.set 3
      i32.const 4525920
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870
        i32.const 4525920
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 2
      local.get 2
      i32.load
      local.set 4
      local.get 2
      i32.load offset=4
      local.set 5
      local.get 1
      local.get 2
      i32.load offset=8
      i32.store offset=24
      local.get 1
      local.get 1
      i32.load offset=24
      i32.store offset=8
      local.get 1
      local.get 4
      i32.store offset=16
      local.get 1
      local.get 5
      i32.store offset=20
      local.get 1
      i32.load offset=16
      local.set 5
      local.get 1
      i32.load offset=20
      local.set 4
      local.get 1
      local.get 5
      i32.store
      local.get 1
      local.get 4
      i32.store offset=4
      local.get 3
      local.get 1
      i32.const 0
      call $j1161
      local.get 0
      i32.const 0
      call $j944
      f32.const 0x1p+0 (;=1;)
      f32.const 0x1.8p-1 (;=0.75;)
      i32.const 0
      call $j1727
      local.set 3
      i32.const 3438972
      i32.load
      call $j873
      local.set 2
      local.get 2
      local.get 0
      i32.const 3496080
      i32.load
      i32.const 0
      call $j1486
      local.get 3
      local.get 2
      i32.const 3502300
      i32.load
      call $j1889
      drop
    end
    local.get 1
    i32.const 32
    i32.add
    global.set 0)

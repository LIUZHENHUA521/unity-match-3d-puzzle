  (func $j9876 (type 7) (param i32 i32 i32 i32)
    (local i32 f32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 0
    i32.load offset=664
    i32.const 0
    i32.const 0
    call $j1360
    drop
    local.get 0
    i32.load offset=664
    local.set 4
    block  ;; label = @1
      local.get 2
      if  ;; label = @2
        local.get 4
        local.get 0
        i32.const 672
        i32.const 668
        local.get 1
        select
        i32.add
        f32.load
        local.get 0
        f32.load offset=704
        i32.const 0
        i32.const 0
        call $j33898
        drop
        br 1 (;@1;)
      end
      local.get 3
      i32.const 32
      i32.add
      local.get 4
      i32.const 0
      call $j1340
      local.get 3
      f32.load offset=32
      local.set 5
      local.get 3
      i32.const 32
      i32.add
      local.get 0
      i32.load offset=664
      i32.const 0
      call $j1340
      local.get 3
      f32.load offset=36
      local.set 6
      local.get 3
      local.get 0
      i32.const 672
      i32.const 668
      local.get 1
      select
      i32.add
      f32.load
      f32.store offset=24
      local.get 3
      local.get 3
      i32.load offset=24
      i32.store offset=8
      local.get 3
      local.get 6
      f32.store offset=20
      local.get 3
      local.get 5
      f32.store offset=16
      local.get 3
      i32.load offset=20
      local.set 0
      local.get 3
      local.get 3
      i32.load offset=16
      i32.store
      local.get 3
      local.get 0
      i32.store offset=4
      local.get 4
      local.get 3
      i32.const 0
      call $j1271
    end
    local.get 3
    i32.const 48
    i32.add
    global.set 0)

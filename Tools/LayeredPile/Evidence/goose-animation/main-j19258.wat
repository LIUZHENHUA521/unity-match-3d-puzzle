  (func $j19258 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 i32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const -64
    i32.add
    local.set 3
    local.get 3
    global.set 0
    i32.const 4507835
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3502396
      call $j870
      i32.const 4507835
      i32.const 1
      i32.store8
    end
    local.get 1
    f32.load offset=8
    local.set 7
    local.get 1
    f32.load offset=4
    drop
    local.get 1
    f32.load
    local.set 8
    local.get 3
    i32.const 48
    i32.add
    local.get 0
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 8
    local.get 3
    f32.load offset=48
    f32.sub
    local.set 8
    local.get 3
    f32.load offset=52
    drop
    local.get 7
    local.get 3
    f32.load offset=56
    f32.sub
    local.set 7
    i32.const 4523252
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4523252
      i32.const 1
      i32.store8
    end
    i32.const 3434688
    i32.load
    call $j871
    local.get 8
    local.get 8
    f32.mul
    f32.const 0x0p+0 (;=0;)
    f32.add
    local.get 7
    local.get 7
    f32.mul
    f32.add
    local.set 11
    i32.const 4523251
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4523251
      i32.const 1
      i32.store8
    end
    i32.const 3434688
    i32.load
    call $j871
    local.get 8
    local.get 8
    f32.mul
    f32.const 0x0p+0 (;=0;)
    f32.add
    local.get 7
    local.get 7
    f32.mul
    f32.add
    f32.sqrt
    local.set 9
    block  ;; label = @1
      local.get 9
      f32.const 0x1.4f8b58p-17 (;=1e-05;)
      f32.gt
      if  ;; label = @2
        local.get 7
        local.get 9
        f32.div
        local.set 7
        f32.const 0x0p+0 (;=0;)
        local.get 9
        f32.div
        local.set 10
        local.get 8
        local.get 9
        f32.div
        local.set 8
        br 1 (;@1;)
      end
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
      local.set 4
      local.get 4
      f32.load offset=8
      local.set 7
      local.get 4
      f32.load offset=4
      local.set 10
      local.get 4
      f32.load
      local.set 8
    end
    i32.const 8
    i32.const 14
    i32.const 0
    call $j1595
    local.set 4
    local.get 0
    i32.const 0
    call $j944
    local.set 6
    local.get 3
    local.get 1
    i32.load offset=8
    i32.store offset=24
    local.get 1
    i32.load
    local.set 5
    local.get 1
    i32.load offset=4
    local.set 1
    local.get 3
    local.get 5
    i32.store offset=16
    local.get 3
    local.get 1
    i32.store offset=20
    local.get 4
    f32.convert_i32_s
    local.set 9
    local.get 6
    local.get 3
    i32.const 16
    i32.add
    local.get 11
    f32.sqrt
    local.get 9
    f32.div
    i32.const 0
    i32.const 0
    call $j3031
    i32.const 1
    i32.const 3502396
    i32.load
    call $j1183
    drop
    local.get 0
    i32.const 0
    call $j944
    local.get 2
    i32.const 0
    call $j1628
    local.get 0
    i32.const 0
    call $j944
    f32.const 0x1p+0 (;=1;)
    f32.const 0x1.99999ap-3 (;=0.2;)
    i32.const 0
    call $j1727
    i32.const 26
    i32.const 3502396
    i32.load
    call $j1183
    drop
    local.get 3
    local.get 7
    f32.store offset=40
    local.get 3
    local.get 3
    i32.load offset=40
    i32.store offset=8
    local.get 3
    local.get 10
    f32.store offset=36
    local.get 3
    local.get 8
    f32.store offset=32
    local.get 3
    i32.load offset=32
    local.set 1
    local.get 3
    i32.load offset=36
    local.set 5
    local.get 3
    local.get 1
    i32.store
    local.get 3
    local.get 5
    i32.store offset=4
    local.get 0
    local.get 0
    local.get 3
    local.get 9
    local.get 3
    call $j36722
    i32.const 0
    call $j1135
    drop
    local.get 0
    i32.const 0
    i32.store8 offset=93
    local.get 3
    i32.const -64
    i32.sub
    global.set 0)
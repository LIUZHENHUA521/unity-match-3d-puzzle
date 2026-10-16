  (func $j13230 (type 9) (param i32 i32 i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32)
    global.get 0
    i32.const 96
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509356
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3491012
      call $j870
      i32.const 3435480
      call $j870
      i32.const 4509356
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.load offset=8
    local.set 7
    i32.const 3435480
    i32.load
    call $j871
    local.get 7
    i32.const 0
    i32.const 0
    call $j5150
    local.set 7
    local.get 7
    i32.const 0
    call $j1027
    local.set 8
    local.get 3
    local.get 1
    i32.load offset=20
    i32.store offset=56
    local.get 1
    local.set 4
    local.get 4
    i32.load offset=12
    local.set 5
    local.get 4
    i32.load offset=16
    local.set 6
    local.get 3
    local.get 5
    i32.store offset=48
    local.get 3
    local.get 6
    i32.store offset=52
    local.get 8
    local.get 3
    i32.const 48
    i32.add
    i32.const 0
    call $j1386
    local.get 7
    i32.const 0
    call $j1027
    local.set 8
    f32.const 0x0p+0 (;=0;)
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j1345
    local.set 9
    f32.const 0x0p+0 (;=0;)
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j1345
    local.set 10
    local.get 3
    f32.const 0x0p+0 (;=0;)
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j1345
    f32.const 0x1.68p+8 (;=360;)
    f32.mul
    f32.store offset=88
    local.get 3
    local.get 3
    i32.load offset=88
    i32.store offset=40
    local.get 3
    local.get 10
    f32.const 0x1.68p+8 (;=360;)
    f32.mul
    f32.store offset=84
    local.get 3
    local.get 9
    f32.const 0x1.68p+8 (;=360;)
    f32.mul
    f32.store offset=80
    local.get 3
    i32.load offset=80
    local.set 6
    local.get 3
    i32.load offset=84
    local.set 5
    local.get 3
    local.get 6
    i32.store offset=32
    local.get 3
    local.get 5
    i32.store offset=36
    local.get 8
    local.get 3
    i32.const 32
    i32.add
    i32.const 0
    call $j1725
    local.get 7
    i32.const 0
    call $j1027
    local.get 0
    i32.const 128
    i32.const 156
    local.get 2
    i32.const 1
    i32.eq
    select
    i32.add
    i32.load
    i32.const 0
    call $j1628
    block  ;; label = @1
      block  ;; label = @2
        local.get 0
        i32.load8_u offset=552
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        i32.load8_u offset=24
        i32.eqz
        br_if 0 (;@2;)
        local.get 7
        i32.const 0
        call $j1027
        local.set 0
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
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
        local.set 4
        local.get 4
        i32.load
        local.set 5
        local.get 4
        i32.load offset=4
        local.set 6
        local.get 3
        local.get 4
        i32.load offset=8
        i32.store offset=72
        local.get 3
        local.get 3
        i32.load offset=72
        i32.store offset=24
        local.get 3
        local.get 5
        i32.store offset=64
        local.get 3
        local.get 6
        i32.store offset=68
        local.get 3
        i32.load offset=64
        local.set 6
        local.get 3
        i32.load offset=68
        local.set 5
        local.get 3
        local.get 6
        i32.store offset=16
        local.get 3
        local.get 5
        i32.store offset=20
        local.get 0
        local.get 3
        i32.const 16
        i32.add
        i32.const 0
        call $j1161
        br 1 (;@1;)
      end
      local.get 7
      i32.const 0
      call $j1027
      local.set 0
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
      local.set 2
      local.get 3
      local.get 2
      i32.load offset=20
      i32.store offset=8
      local.get 2
      local.set 4
      local.get 4
      i32.load offset=12
      local.set 5
      local.get 4
      i32.load offset=16
      local.set 6
      local.get 3
      local.get 5
      i32.store
      local.get 3
      local.get 6
      i32.store offset=4
      local.get 0
      local.get 3
      i32.const 0
      call $j1161
    end
    local.get 7
    i32.const 3491012
    i32.load
    call $j1083
    local.set 0
    local.get 0
    local.get 1
    i32.load offset=8
    i32.const 0
    call $j19263
    local.get 0
    local.get 1
    i32.load8_u offset=24
    i32.const 0
    call $j6602
    local.get 3
    i32.const 96
    i32.add
    global.set 0
    local.get 0
    return)

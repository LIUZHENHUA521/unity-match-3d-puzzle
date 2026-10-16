  (func $j9875 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 i32 f32 f32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509395
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 4509395
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    local.get 3
    local.set 5
    block  ;; label = @1
      local.get 3
      call $j1923
      i32.eqz
      if  ;; label = @2
        local.get 1
        i32.load offset=52
        local.set 4
        br 1 (;@1;)
      end
      local.get 1
      i32.load offset=56
      local.get 2
      i32.const 1
      i32.sub
      i32.const 2
      i32.shl
      i32.add
      i32.load offset=16
      local.set 4
    end
    local.get 4
    i32.load offset=20
    local.set 4
    local.get 5
    local.get 4
    i32.const 0
    call $j1028
    local.get 3
    f32.load offset=8
    local.set 8
    local.get 3
    f32.load offset=4
    local.set 9
    local.get 3
    f32.load
    local.set 10
    local.get 3
    local.get 4
    i32.const 0
    call $j1387
    local.get 3
    f32.load
    local.set 7
    local.get 3
    local.get 4
    i32.const 0
    call $j1387
    local.get 7
    f32.neg
    local.get 3
    f32.load
    i32.const 0
    call $j1345
    local.set 7
    f32.const 0x0p+0 (;=0;)
    f32.const 0x1p+1 (;=2;)
    i32.const 0
    call $j1345
    local.set 12
    local.get 3
    local.get 4
    i32.const 0
    call $j1387
    local.get 3
    f32.load offset=8
    local.set 11
    local.get 3
    local.get 4
    i32.const 0
    call $j1387
    local.get 11
    f32.neg
    local.get 3
    f32.load offset=8
    i32.const 0
    call $j1345
    local.set 11
    local.get 10
    local.get 7
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    f32.add
    local.set 10
    local.get 9
    local.get 12
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    f32.add
    local.set 9
    local.get 8
    local.get 11
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    f32.add
    local.set 8
    local.get 3
    local.get 4
    i32.const 0
    call $j1028
    local.get 0
    local.set 5
    local.get 3
    f32.load offset=4
    local.get 1
    i32.const 300
    i32.const 364
    local.get 2
    i32.const 1
    i32.eq
    select
    i32.add
    f32.load
    f32.const 0x1p+1 (;=2;)
    f32.add
    f32.lt
    if  ;; label = @1
      i32.const 4524773
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3439908
        call $j870
        i32.const 4524773
        i32.const 1
        i32.store8
      end
      i32.const 3439908
      i32.load
      i32.load offset=92
      local.set 6
      local.get 6
      f32.load offset=32
      local.set 12
      local.get 6
      f32.load offset=28
      local.set 11
      local.get 6
      f32.load offset=24
      local.set 7
      local.get 1
      i32.const 300
      i32.const 364
      local.get 2
      i32.const 1
      i32.eq
      select
      i32.add
      f32.load
      local.set 13
      local.get 3
      local.get 4
      i32.const 0
      call $j1028
      local.get 7
      local.set 14
      local.get 13
      f32.const 0x1p+1 (;=2;)
      f32.add
      local.get 3
      f32.load offset=4
      f32.sub
      local.set 7
      local.get 10
      local.get 14
      local.get 7
      f32.mul
      f32.add
      local.set 10
      local.get 9
      local.get 11
      local.get 7
      f32.mul
      f32.add
      local.set 9
      local.get 8
      local.get 12
      local.get 7
      f32.mul
      f32.add
      local.set 8
    end
    local.get 5
    local.get 8
    f32.store offset=8
    local.get 0
    local.get 9
    f32.store offset=4
    local.get 0
    local.get 10
    f32.store
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

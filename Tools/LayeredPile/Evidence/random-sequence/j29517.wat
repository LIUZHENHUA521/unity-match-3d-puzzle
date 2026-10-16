  (func $j29517 (type 44) (param i32 i32 i32 i32) (result f32)
    (local i32 f32 f32 f32 f32)
    i32.const 4507302
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3464556
      call $j870
      i32.const 3464568
      call $j870
      i32.const 3464552
      call $j870
      i32.const 3423828
      call $j870
      i32.const 3439848
      call $j870
      i32.const 4507302
      i32.const 1
      i32.store8
    end
    i32.const 3423828
    i32.load
    call $j873
    local.set 4
    local.get 4
    i32.const 3464552
    i32.load
    call $j1967
    i32.const 0
    local.set 3
    loop  ;; label = @1
      local.get 0
      i32.load offset=12
      local.get 3
      i32.gt_s
      if  ;; label = @2
        local.get 4
        local.get 2
        local.get 3
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=16
        local.get 2
        local.get 3
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=20
        f32.add
        local.get 2
        local.get 3
        i32.const 12
        i32.mul
        i32.add
        f32.load offset=24
        f32.mul
        local.get 3
        i32.const 3464556
        i32.load
        call $j1709
        local.get 3
        i32.const 1
        i32.add
        local.set 3
        br 1 (;@1;)
      end
    end
    local.get 4
    i32.const 3464568
    i32.load
    call $j1898
    local.set 3
    local.get 0
    local.get 3
    i32.const 2
    i32.shl
    i32.add
    f32.load offset=16
    local.set 5
    local.get 1
    local.get 3
    i32.const 2
    i32.shl
    i32.add
    f32.load offset=16
    local.set 6
    local.get 2
    local.get 3
    i32.const 12
    i32.mul
    i32.add
    f32.load offset=16
    local.set 7
    local.get 2
    local.get 3
    i32.const 12
    i32.mul
    i32.add
    f32.load offset=20
    local.set 8
    i32.const 3439848
    i32.load
    call $j871
    local.get 5
    local.get 6
    local.get 7
    local.get 8
    local.get 3
    call $j16590
    return)
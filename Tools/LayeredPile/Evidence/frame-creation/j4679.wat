  (func $j4679 (type 7) (param i32 i32 i32 i32)
    (local f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4507828
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 4507828
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load offset=20
      i32.const 0
      call $j7845
      local.get 1
      i32.eq
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=19
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=16
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.load8_u offset=18
        br_if 1 (;@1;)
      end
      block  ;; label = @2
        local.get 2
        br_if 0 (;@2;)
        local.get 3
        local.get 0
        i32.load offset=20
        i32.const 0
        call $j35148
        local.get 3
        f32.load offset=8
        local.set 4
        local.get 3
        f32.load offset=4
        local.set 5
        local.get 3
        f32.load
        local.set 6
        i32.const 4523252
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          call $j870
          i32.const 4523252
          i32.const 1
          i32.store8
        end
        i32.const 3434688
        i32.load
        call $j871
        local.get 6
        local.get 6
        f32.mul
        local.get 5
        local.get 5
        f32.mul
        f32.add
        local.get 4
        local.get 4
        f32.mul
        f32.add
        f32.sqrt
        f32.const 0x1.99999ap-3 (;=0.2;)
        f32.gt
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        br_if 1 (;@1;)
      end
      local.get 0
      local.get 1
      i32.const 1
      i32.xor
      local.get 0
      call $j7954
      local.get 1
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load8_u offset=4
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=92
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      local.get 0
      call $j19265
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

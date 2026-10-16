  (func $j16590 (type 467) (param f32 f32 f32 f32 i32) (result f32)
    (local f32 f32)
    i32.const 4518487
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434696
      call $j870
      i32.const 4518487
      i32.const 1
      i32.store8
    end
    local.get 2
    f32.abs
    local.set 5
    local.get 3
    f32.abs
    local.set 6
    local.get 5
    local.get 6
    local.get 5
    local.get 6
    f32.gt
    select
    f32.const 0x1.0c6f7ap-20 (;=1e-06;)
    f32.mul
    local.set 5
    i32.const 3434696
    i32.load
    i32.load offset=92
    f32.load
    f32.const 0x1p+3 (;=8;)
    f32.mul
    local.set 6
    local.get 3
    local.get 2
    f32.sub
    f32.abs
    local.get 5
    local.get 6
    local.get 5
    local.get 6
    f32.gt
    select
    f32.lt
    if  ;; label = @1
      local.get 0
      local.get 1
      i32.const 0
      call $j1345
      return
    end
    local.get 0
    local.set 5
    local.get 1
    local.get 0
    f32.sub
    local.set 1
    f32.const 0x0p+0 (;=0;)
    local.get 2
    local.get 3
    f32.add
    f32.const 0x1p+0 (;=1;)
    f32.mul
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    i32.const 0
    call $j1345
    local.set 0
    local.get 0
    local.get 0
    f32.add
    local.get 3
    local.get 2
    f32.sub
    f32.div
    local.set 6
    local.get 2
    local.get 2
    f32.add
    local.get 3
    local.get 2
    f32.sub
    f32.div
    local.set 0
    local.get 5
    local.get 1
    local.get 6
    local.get 0
    f32.const 0x1p-2 (;=0.25;)
    f32.mul
    local.get 0
    f32.mul
    f32.add
    f32.sqrt
    local.get 0
    f32.const 0x1p-1 (;=0.5;)
    f32.mul
    f32.sub
    f32.mul
    f32.add
    return)
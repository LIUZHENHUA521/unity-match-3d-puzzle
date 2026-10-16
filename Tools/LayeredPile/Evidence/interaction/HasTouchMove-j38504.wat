  (func $j38504 (type 2) (param i32 i32) (result i32)
    (local i32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 1
    local.set 2
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=72
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      i32.const 0
      call $j1860
      local.get 1
      f32.load
      local.set 3
      local.get 0
      f32.load offset=212
      local.set 4
      i32.const 4518487
      i32.load8_u
      i32.eqz
      if  ;; label = @2
        i32.const 3434696
        call $j870
        i32.const 4518487
        i32.const 1
        i32.store8
      end
      local.get 4
      local.get 3
      f32.sub
      f32.abs
      local.set 5
      local.get 3
      f32.abs
      local.set 3
      local.get 4
      f32.abs
      local.set 4
      local.get 3
      local.get 4
      local.get 3
      local.get 4
      f32.gt
      select
      f32.const 0x1.0c6f7ap-20 (;=1e-06;)
      f32.mul
      local.set 3
      i32.const 3434696
      i32.load
      i32.load offset=92
      f32.load
      f32.const 0x1p+3 (;=8;)
      f32.mul
      local.set 4
      local.get 5
      local.get 3
      local.get 4
      local.get 3
      local.get 4
      f32.gt
      select
      f32.lt
      if  ;; label = @2
        i32.const 0
        local.set 2
        local.get 1
        i32.const 0
        call $j1860
        local.get 1
        f32.load offset=4
        local.set 3
        local.get 0
        f32.load offset=216
        local.set 4
        i32.const 4518487
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434696
          call $j870
          i32.const 4518487
          i32.const 1
          i32.store8
        end
        local.get 4
        local.get 3
        f32.sub
        f32.abs
        local.set 5
        local.get 3
        f32.abs
        local.set 3
        local.get 4
        f32.abs
        local.set 4
        local.get 3
        local.get 4
        local.get 3
        local.get 4
        f32.gt
        select
        f32.const 0x1.0c6f7ap-20 (;=1e-06;)
        f32.mul
        local.set 3
        i32.const 3434696
        i32.load
        i32.load offset=92
        f32.load
        f32.const 0x1p+3 (;=8;)
        f32.mul
        local.set 4
        local.get 5
        local.get 3
        local.get 4
        local.get 3
        local.get 4
        f32.gt
        select
        f32.lt
        br_if 1 (;@1;)
      end
      i32.const 1
      local.set 2
    end
    local.get 1
    i32.const 16
    i32.add
    global.set 0
    local.get 2
    return)

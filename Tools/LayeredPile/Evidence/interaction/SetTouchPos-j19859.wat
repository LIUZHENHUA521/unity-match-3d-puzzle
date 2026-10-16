  (func $j19859 (type 1) (param i32 i32 i32)
    (local i32 i32 i32 i32 i32 i32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    block  ;; label = @1
      local.get 1
      if  ;; label = @2
        i32.const 4524770
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439888
          call $j870
          i32.const 4524770
          i32.const 1
          i32.store8
        end
        i32.const 3439888
        i32.load
        i32.load offset=92
        local.set 1
        local.get 1
        f32.load offset=12
        local.set 9
        local.get 1
        f32.load offset=8
        f32.const -0x1.f4p+9 (;=-1000;)
        f32.mul
        i32.reinterpret_f32
        local.set 1
        local.get 9
        f32.const -0x1.f4p+9 (;=-1000;)
        f32.mul
        i32.reinterpret_f32
        local.set 3
        br 1 (;@1;)
      end
      local.get 2
      i32.const 0
      call $j1860
      local.get 2
      i32.load
      local.set 1
      local.get 2
      i32.load offset=4
      local.set 3
    end
    local.get 0
    local.set 7
    i32.const 0
    local.set 0
    local.get 1
    local.set 8
    local.get 0
    local.set 6
    i32.const 0
    local.set 0
    local.get 3
    local.set 1
    i32.const 32
    local.set 4
    local.get 4
    i32.const 31
    i32.and
    local.set 5
    i32.const 32
    local.get 4
    i32.const 63
    i32.and
    i32.le_u
    if  ;; label = @1
      local.get 1
      local.get 5
      i32.shl
      local.set 3
      i32.const 0
      local.set 4
    else
      i32.const 1
      local.get 5
      i32.shl
      i32.const 1
      i32.sub
      local.get 1
      i32.const 32
      local.get 5
      i32.sub
      i32.shr_u
      i32.and
      local.get 0
      local.get 5
      i32.shl
      i32.or
      local.set 3
      local.get 1
      local.get 5
      i32.shl
      local.set 4
    end
    local.get 3
    local.set 1
    local.get 6
    local.set 3
    local.get 8
    local.set 0
    local.get 3
    local.get 1
    i32.or
    local.set 1
    local.get 0
    local.get 4
    i32.or
    local.set 6
    local.get 7
    local.set 0
    local.get 0
    local.get 6
    i32.store offset=212
    local.get 0
    local.get 1
    i32.store offset=216
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

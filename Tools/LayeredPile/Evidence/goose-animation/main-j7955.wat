  (func $j7955 (type 1) (param i32 i32 i32)
    i32.const 4507823
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870
      i32.const 3541696
      call $j870
      i32.const 4507823
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=44
    local.set 2
    i32.const 3435416
    i32.load
    call $j871
    local.get 2
    i32.const 0
    i32.const 0
    call $j924
    i32.eqz
    if  ;; label = @1
      local.get 1
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.load offset=44
        i32.const 0
        call $j8464
        local.get 0
        i32.const 3541696
        i32.load
        i32.const 0
        call $j1446
        return
      end
      local.get 0
      local.get 0
      i32.load8_u offset=16
      i32.const 1
      f32.const 0x1.99999ap-3 (;=0.2;)
      local.get 0
      call $j7956
      drop
      local.get 0
      i32.const 3541696
      i32.load
      i32.const 0
      call $j1446
      local.get 0
      i32.const 3541696
      i32.load
      i32.const 0
      call $j1535
      drop
    end)
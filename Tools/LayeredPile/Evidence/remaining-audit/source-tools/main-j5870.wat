  (func $j5870 (type 0) (param i32 i32)
    i32.const 4509338
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 4509338
      i32.const 1
      i32.store8
    end
    local.get 0
    local.get 0
    i32.load offset=192
    local.get 0
    f32.load offset=300
    i32.const 1
    local.get 0
    call $j19885
    i32.const 3434312
    i32.load
    call $j871
    local.get 0
    call $j1923
    if  ;; label = @1
      local.get 0
      local.get 0
      i32.load offset=208
      local.get 0
      f32.load offset=364
      i32.const 2
      local.get 0
      call $j19885
    end)

  (func $j83394 (type 0) (param i32 i32)
    (local f32)
    i32.const 4509314
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3563188
      call $j870
      i32.const 4509314
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=436
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 424
      i32.add
      local.set 1
      local.get 1
      i32.const 0
      call $j2631
      if  ;; label = @2
        local.get 1
        i32.const 0
        call $j8421
        local.set 2
        i32.const 3434312
        i32.load
        call $j871
        i32.const 3434312
        i32.load
        i32.load offset=92
        local.get 2
        f32.store offset=52
        br 1 (;@1;)
      end
      i32.const 3563188
      i32.load
      i32.const 0
      call $j1547
    end
    local.get 0
    local.get 0
    call $j19884
    if  ;; label = @1
      local.get 0
      local.get 0
      call $j38498
    end)
  (func $j85374 (type 6) (param i32 i32 i32 i32)
    i32.const 4505040
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505040
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 1
    i32.const 0
    i32.const 0
    i32.const 924
    call_indirect (type 5)
    i32.eqz
    if  ;; label = @1
      local.get 0
      i32.load8_u offset=57
      if  ;; label = @2
        local.get 0
        i32.const 0
        local.get 0
        i32.const 2810
        call_indirect (type 3)
      end
      local.get 0
      local.get 1
      i32.store offset=424
      local.get 0
      i32.const 5
      i32.store offset=24
      local.get 0
      local.get 2
      local.get 0
      i32.const 20231
      call_indirect (type 3)
    end)

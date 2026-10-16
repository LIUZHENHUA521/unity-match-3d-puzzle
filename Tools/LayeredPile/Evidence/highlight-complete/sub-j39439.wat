  (func $j39439 (type 4) (param i32 i32)
    i32.const 4505060
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3468540
      i32.const 870
      call_indirect (type 0)
      i32.const 3468544
      i32.const 870
      call_indirect (type 0)
      i32.const 4505060
      i32.const 1
      i32.store8
    end
    i32.const 3432484
    i32.load
    i32.const 871
    call_indirect (type 0)
    i32.const 3432484
    i32.load
    i32.load offset=92
    i32.load offset=132
    local.get 0
    i32.const 3468544
    i32.load
    i32.const 1235
    call_indirect (type 5)
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=132
      local.get 0
      i32.const 3468540
      i32.load
      i32.const 903
      call_indirect (type 3)
    end)

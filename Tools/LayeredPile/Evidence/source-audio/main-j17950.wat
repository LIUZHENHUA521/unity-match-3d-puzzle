  (func $j17950 (type 1) (param i32 i32 i32)
    i32.const 4508433
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870
      i32.const 4508433
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
    local.get 1
    i32.const 3437580
    i32.load
    i32.load offset=92
    i32.load offset=28
    i32.eq
    if  ;; label = @1
      i32.const 3437580
      i32.load
      call $j871
      local.get 1
      local.get 1
      call $j9352
    end)

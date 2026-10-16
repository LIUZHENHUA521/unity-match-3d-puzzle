  (func $j33495 (type 0) (param i32 i32)
    i32.const 4508431
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435224
      call $j870
      i32.const 3437580
      call $j870
      i32.const 4508431
      i32.const 1
      i32.store8
    end
    i32.const 3435224
    i32.load
    call $j871
    local.get 0
    i32.const 0
    call $j2923
    i32.load8_u offset=9
    local.get 0
    call $j33489
    i32.const 3437580
    i32.load
    call $j871
    i32.const 3437580
    i32.load
    i32.load offset=92
    i32.const 1
    i32.store offset=28)

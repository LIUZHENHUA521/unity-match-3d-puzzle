  (func $j38465 (type 2) (param i32 i32) (result i32)
    i32.const 4507816
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3457636
      call $j870
      i32.const 3434332
      call $j870
      i32.const 4507816
      i32.const 1
      i32.store8
    end
    i32.const 3434332
    i32.load
    call $j871
    i32.const 3434332
    i32.load
    i32.load offset=92
    i32.load offset=8
    local.get 0
    i32.const 3457636
    i32.load
    call $j1060
    return)

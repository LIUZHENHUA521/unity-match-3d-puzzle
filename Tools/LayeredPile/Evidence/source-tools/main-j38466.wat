  (func $j38466 (type 0) (param i32 i32)
    (local i32)
    i32.const 4507809
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434332
      call $j870
      i32.const 3481056
      call $j870
      i32.const 4507809
      i32.const 1
      i32.store8
    end
    i32.const 3434332
    i32.load
    call $j871
    i32.const 3434332
    i32.load
    i32.load offset=92
    i32.load offset=12
    local.set 1
    i32.const 3434332
    i32.load
    i32.load offset=92
    i32.load offset=4
    local.set 2
    local.get 0
    i32.const 0
    i32.store
    local.get 0
    i32.const 0
    i32.store offset=4
    local.get 0
    local.get 1
    local.get 2
    i32.const 3481056
    i32.load
    call $j1483)

  (func $j69528 (type 1) (param i32 i32 i32)
    i32.const 4508440
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3429072
      call $j870
      i32.const 3437580
      call $j870
      i32.const 4508440
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 0
    call $j2135
    local.set 0
    i32.const 3437580
    i32.load
    call $j871
    local.get 0
    i32.const 3429072
    i32.load
    call $j919
    local.set 1
    i32.const 3437580
    i32.load
    i32.load offset=92
    local.get 1
    i32.store offset=36
    i32.const 3437580
    i32.load
    i32.load offset=92
    drop
    local.get 0
    i32.const 3429072
    i32.load
    call $j919
    drop
    local.get 0
    i32.const 2
    local.get 0
    call $j17950)

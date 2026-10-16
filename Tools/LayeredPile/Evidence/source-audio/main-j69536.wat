  (func $j69536 (type 0) (param i32 i32)
    (local f32)
    i32.const 4508416
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870
      i32.const 3440340
      call $j870
      i32.const 4508416
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
    i32.const 3437580
    i32.load
    i32.load offset=92
    local.get 0
    i32.store
    i32.const 3437580
    i32.load
    i32.load offset=92
    drop
    local.get 0
    f32.load offset=52
    local.set 2
    i32.const 3440340
    i32.load
    call $j873
    local.set 1
    local.get 1
    i32.const -1082130432
    i32.store offset=12
    local.get 1
    local.get 2
    f32.store offset=8
    local.get 0
    local.get 1
    i32.store offset=56)

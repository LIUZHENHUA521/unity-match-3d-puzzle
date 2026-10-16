  (func $j9353 (type 0) (param i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    local.get 0
    i32.store offset=12
    i32.const 4508425
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870
      i32.const 3437584
      call $j870
      i32.const 4508425
      i32.const 1
      i32.store8
    end
    i32.const 3437584
    i32.load
    local.set 0
    local.get 1
    i32.const -1
    i32.store offset=4
    local.get 1
    local.get 0
    i32.store
    local.get 1
    local.get 1
    i32.load offset=12
    i32.store offset=8
    local.get 1
    i32.const 0
    call $j956
    local.set 0
    i32.const 3437580
    i32.load
    call $j871
    local.get 0
    local.get 1
    call $j9354
    local.get 1
    i32.const 16
    i32.add
    global.set 0)

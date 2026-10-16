  (func $j95666 (type 3) (param i32)
    i32.const 4508667
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428772
      call $j870
      i32.const 3454252
      call $j870
      i32.const 3419936
      call $j870
      i32.const 4508667
      i32.const 1
      i32.store8
    end
    i32.const 3419936
    i32.load
    call $j873
    local.set 0
    local.get 0
    i32.const 3454252
    i32.load
    call $j920
    i32.const 3428772
    i32.load
    i32.load offset=92
    local.get 0
    i32.store offset=4
    i32.const 3428772
    i32.load
    i32.load offset=92
    drop)
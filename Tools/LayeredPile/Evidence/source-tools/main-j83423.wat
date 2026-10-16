  (func $j83423 (type 2) (param i32 i32) (result i32)
    i32.const 4509400
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3443516
      call $j870
      i32.const 4509400
      i32.const 1
      i32.store8
    end
    i32.const 3443516
    i32.load
    call $j873
    local.set 1
    local.get 1
    i32.const 0
    i32.store offset=8
    local.get 1
    local.get 0
    i32.store offset=16
    local.get 1
    return)

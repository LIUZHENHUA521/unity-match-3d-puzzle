  (func $j36722 (type 30) (param i32 i32 f32 i32) (result i32)
    i32.const 4507836
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3444176
      call $j870
      i32.const 4507836
      i32.const 1
      i32.store8
    end
    i32.const 3444176
    i32.load
    call $j873
    local.set 3
    local.get 3
    i32.const 0
    i32.store offset=8
    local.get 3
    local.get 0
    i32.store offset=16
    local.get 3
    local.get 1
    i32.load offset=8
    i32.store offset=28
    local.get 1
    i32.load offset=4
    local.set 0
    local.get 3
    local.get 1
    i32.load
    i32.store offset=20
    local.get 3
    local.get 0
    i32.store offset=24
    local.get 3
    local.get 2
    f32.store offset=32
    local.get 3
    return)

  (func $j92875 (type 17) (param i32 i32) (result f32)
    (local f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    local.get 0
    i32.load offset=28
    i32.const 0
    i32.const 1028
    call_indirect (type 3)
    local.get 1
    f32.load offset=4
    local.set 2
    local.get 1
    i32.const 16
    i32.add
    global.set 0
    local.get 2
    return)

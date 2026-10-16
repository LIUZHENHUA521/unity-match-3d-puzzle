  (func $j38735 (type 1) (param i32 i32 i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 1
    i32.store offset=12
    local.get 0
    local.get 3
    i32.const 12
    i32.add
    local.get 2
    call $j19949
    local.get 3
    i32.const 16
    i32.add
    global.set 0)
  (func $j38863 (type 31) (param i32 i32 i32 f32 i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 5
    local.get 5
    global.set 0
    local.get 5
    local.get 3
    f32.store offset=12
    local.get 5
    local.get 4
    i32.store offset=8
    local.get 5
    local.get 5
    i32.const 12
    i32.add
    i32.store offset=4
    local.get 1
    i32.load offset=8
    local.get 0
    local.get 1
    local.get 2
    local.get 5
    i32.const 4
    i32.add
    local.get 4
    call $redir_viiiii
    local.get 5
    i32.const 16
    i32.add
    global.set 0)

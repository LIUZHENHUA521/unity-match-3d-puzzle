  (func $j9885 (type 7) (param i32 i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 1
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 1
    local.get 3
    f32.load offset=4
    local.get 0
    local.get 2
    f32.const 0x1p+0 (;=1;)
    local.get 3
    call $j19871
    f32.lt
    i32.eqz
    i32.const 0
    call $j6602
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

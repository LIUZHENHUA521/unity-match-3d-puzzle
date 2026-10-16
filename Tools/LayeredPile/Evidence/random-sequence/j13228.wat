  (func $j13228 (type 31) (param i32 i32 i32 f32 i32)
    (local f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    local.get 2
    local.get 3
    local.get 1
    call $j6680
    local.set 2
    local.get 1
    local.get 2
    i32.const 0
    call $j1028
    local.get 1
    f32.load offset=8
    local.set 3
    local.get 1
    f32.load offset=4
    local.set 5
    local.get 1
    f32.load
    local.set 6
    f32.const -0x1p+0 (;=-1;)
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j1345
    local.set 7
    local.get 1
    local.get 2
    i32.const 0
    call $j1387
    local.get 1
    f32.load
    local.set 8
    local.get 1
    local.get 2
    i32.const 0
    call $j1028
    local.get 1
    f32.load offset=4
    local.set 9
    f32.const -0x1p+0 (;=-1;)
    f32.const 0x1p+0 (;=1;)
    i32.const 0
    call $j1345
    local.set 10
    local.get 1
    local.get 2
    i32.const 0
    call $j1387
    local.get 0
    local.get 3
    local.get 10
    local.get 1
    f32.load offset=8
    f32.mul
    f32.const 0x1.ccccccp-2 (;=0.45;)
    f32.mul
    f32.add
    f32.store offset=8
    local.get 0
    local.get 5
    local.get 9
    f32.neg
    f32.add
    f32.store offset=4
    local.get 0
    local.get 6
    local.get 7
    local.get 8
    f32.mul
    f32.const 0x1.ccccccp-2 (;=0.45;)
    f32.mul
    f32.add
    f32.store
    local.get 1
    i32.const 16
    i32.add
    global.set 0)
  (func $j36723 (type 16) (param i32 f32 i32)
    (local i32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    i32.const 0
    call $j944
    local.set 3
    local.get 0
    f32.load offset=152
    local.set 4
    f32.const 0x0p+0 (;=0;)
    local.get 1
    f32.const 0x1p+0 (;=1;)
    f32.min
    local.get 1
    f32.const 0x0p+0 (;=0;)
    f32.lt
    select
    local.set 1
    local.get 4
    local.get 0
    f32.load offset=136
    local.get 4
    f32.sub
    local.get 1
    f32.mul
    f32.add
    local.set 4
    local.get 0
    f32.load offset=156
    local.set 5
    local.get 5
    local.get 0
    f32.load offset=140
    local.get 5
    f32.sub
    local.get 1
    f32.mul
    f32.add
    local.set 5
    local.get 0
    f32.load offset=160
    local.set 6
    local.get 2
    local.get 6
    local.get 0
    f32.load offset=144
    local.get 6
    f32.sub
    local.get 1
    f32.mul
    f32.add
    f32.store offset=24
    local.get 2
    local.get 2
    i32.load offset=24
    i32.store offset=8
    local.get 2
    local.get 5
    f32.store offset=20
    local.get 2
    local.get 4
    f32.store offset=16
    local.get 2
    i32.load offset=20
    local.set 0
    local.get 2
    local.get 2
    i32.load offset=16
    i32.store
    local.get 2
    local.get 0
    i32.store offset=4
    local.get 3
    local.get 2
    i32.const 0
    call $j1386
    local.get 2
    i32.const 32
    i32.add
    global.set 0)

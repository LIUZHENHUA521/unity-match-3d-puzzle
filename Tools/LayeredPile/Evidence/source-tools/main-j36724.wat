  (func $j36724 (type 16) (param i32 f32 i32)
    (local i32 i32 i32 i32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 160
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    i32.const 0
    call $j944
    local.set 5
    local.get 2
    i32.const 128
    i32.add
    local.get 0
    i32.load offset=132
    i32.const 0
    call $j1028
    local.get 2
    local.get 2
    i32.load offset=136
    i32.store offset=152
    local.get 2
    i32.load offset=128
    local.set 4
    local.get 2
    i32.load offset=132
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=144
    local.get 2
    local.get 3
    i32.store offset=148
    local.get 2
    i32.const 112
    i32.add
    local.get 0
    i32.load offset=132
    i32.const 0
    call $j4551
    local.get 2
    local.get 2
    i32.load offset=120
    i32.store offset=136
    local.get 2
    i32.load offset=112
    local.set 3
    local.get 2
    i32.load offset=116
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=128
    local.get 2
    local.get 4
    i32.store offset=132
    local.get 0
    f32.load offset=148
    local.set 9
    i32.const 0
    call $j1414
    local.set 7
    local.get 2
    local.get 2
    i32.load offset=152
    i32.store offset=104
    local.get 2
    local.get 2
    i32.load offset=136
    i32.store offset=88
    local.get 2
    i32.load offset=144
    local.set 4
    local.get 2
    i32.load offset=148
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=96
    local.get 2
    local.get 3
    i32.store offset=100
    local.get 2
    i32.load offset=128
    local.set 3
    local.get 2
    i32.load offset=132
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=80
    local.get 2
    local.get 4
    i32.store offset=84
    local.get 2
    local.get 2
    i32.load offset=104
    i32.store offset=40
    local.get 2
    local.get 2
    i32.load offset=88
    i32.store offset=24
    local.get 2
    i32.load offset=96
    local.set 4
    local.get 2
    i32.load offset=100
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=32
    local.get 2
    local.get 3
    i32.store offset=36
    local.get 2
    i32.load offset=80
    local.set 3
    local.get 2
    i32.load offset=84
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=16
    local.get 2
    local.get 4
    i32.store offset=20
    local.get 5
    local.get 2
    i32.const 32
    i32.add
    local.get 2
    i32.const 16
    i32.add
    local.get 9
    local.get 7
    f32.mul
    local.get 1
    f32.mul
    i32.const 0
    call $j6370
    local.get 0
    i32.const 0
    call $j944
    local.set 5
    local.get 2
    i32.const 112
    i32.add
    local.get 0
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 2
    f32.load offset=120
    local.set 1
    local.get 2
    f32.load offset=116
    local.set 9
    local.get 2
    f32.load offset=112
    local.set 7
    local.get 2
    i32.const 112
    i32.add
    local.get 0
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 2
    f32.load offset=112
    local.set 8
    local.get 0
    f32.load offset=140
    local.set 10
    local.get 2
    i32.const 112
    i32.add
    local.get 0
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 2
    f32.load offset=120
    local.set 11
    local.get 7
    local.set 12
    local.get 8
    local.get 7
    f32.sub
    local.set 8
    i32.const 0
    call $j1414
    local.set 7
    local.get 7
    local.get 7
    f32.add
    local.set 7
    f32.const 0x0p+0 (;=0;)
    local.get 7
    f32.const 0x1p+0 (;=1;)
    f32.min
    local.get 7
    f32.const 0x0p+0 (;=0;)
    f32.lt
    select
    local.set 7
    local.get 12
    local.get 8
    local.get 7
    f32.mul
    f32.add
    local.set 8
    local.get 2
    local.get 1
    local.get 11
    local.get 1
    f32.sub
    local.get 7
    f32.mul
    f32.add
    f32.store offset=72
    local.get 2
    local.get 2
    i32.load offset=72
    i32.store offset=8
    local.get 2
    local.get 9
    local.get 10
    local.get 9
    f32.sub
    local.get 7
    f32.mul
    f32.add
    f32.store offset=68
    local.get 2
    local.get 8
    f32.store offset=64
    local.get 2
    i32.load offset=64
    local.set 4
    local.get 2
    i32.load offset=68
    local.set 3
    local.get 2
    local.get 4
    i32.store
    local.get 2
    local.get 3
    i32.store offset=4
    local.get 5
    local.get 2
    i32.const 0
    call $j1386
    local.get 2
    i32.const 48
    i32.add
    local.get 0
    i32.const 0
    call $j944
    i32.const 0
    call $j1028
    local.get 2
    i32.load offset=56
    local.set 5
    local.get 2
    local.get 5
    i32.store offset=120
    local.get 2
    i32.load offset=48
    local.set 3
    local.get 2
    i32.load offset=52
    local.set 4
    local.get 3
    local.set 6
    local.get 2
    local.get 6
    i32.store offset=112
    local.get 2
    local.get 4
    i32.store offset=116
    local.get 0
    local.get 5
    i32.store offset=160
    local.get 0
    local.set 3
    local.get 3
    local.get 6
    i32.store offset=152
    local.get 3
    local.get 4
    i32.store offset=156
    local.get 2
    i32.const 160
    i32.add
    global.set 0)

  (func $j39441 (type 3) (param i32 i32 i32)
    (local i32 i32 f32 f32 f32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    f32.load offset=896
    local.set 5
    local.get 0
    f32.load offset=868
    local.set 6
    local.get 0
    f32.load offset=876
    local.set 7
    local.get 0
    i32.load offset=888
    local.set 3
    local.get 0
    i32.load offset=892
    local.set 4
    local.get 2
    local.get 3
    i32.store offset=24
    local.get 2
    local.get 4
    i32.store offset=28
    local.get 0
    i32.load offset=880
    local.set 4
    local.get 0
    i32.load offset=884
    local.set 3
    local.get 2
    local.get 4
    i32.store offset=16
    local.get 2
    local.get 3
    i32.store offset=20
    local.get 2
    local.get 1
    i32.load offset=8
    i32.store offset=8
    local.get 1
    i32.load
    local.set 3
    local.get 1
    i32.load offset=4
    local.set 4
    local.get 2
    local.get 3
    i32.store
    local.get 2
    local.get 4
    i32.store offset=4
    local.get 0
    local.get 2
    i32.const 16
    i32.add
    local.get 7
    local.get 6
    local.get 2
    local.get 5
    local.get 2
    call $j39442 ;; HighlightEffect.HitFX
    local.get 2
    i32.const 32
    i32.add
    global.set 0)

  (func $j19871 (type 54) (param i32 i32 f32 i32) (result f32)
    (local f32 f32 f32 f32)
    local.get 0
    f32.load offset=388
    local.set 4
    local.get 0
    i32.const 304
    i32.const 368
    local.get 1
    i32.const 1
    i32.eq
    select
    i32.add
    f32.load
    local.set 5
    local.get 4
    local.get 5
    f32.sub
    local.set 6
    local.get 5
    local.set 7
    local.get 0
    f32.load offset=380
    local.set 5
    local.get 6
    local.get 4
    f32.div
    local.set 4
    local.get 7
    local.get 5
    local.get 0
    f32.load offset=384
    local.get 5
    f32.sub
    f32.const 0x0p+0 (;=0;)
    local.get 4
    f32.const 0x1p+0 (;=1;)
    f32.min
    local.get 4
    f32.const 0x0p+0 (;=0;)
    f32.lt
    select
    f32.const 0x0p+0 (;=0;)
    f32.max
    f32.mul
    f32.add
    local.get 2
    f32.const 0x1p+0 (;=1;)
    f32.min
    f32.mul
    f32.sub
    return)

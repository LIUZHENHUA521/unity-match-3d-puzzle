  (func $j19259 (type 16) (param i32 f32 i32)
    (local f32)
    i32.const 4507834
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3502396
      call $j870
      i32.const 4507834
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.const 0
    i32.store8 offset=19
    local.get 0
    i32.const 1
    i32.store8 offset=16
    local.get 0
    i32.load offset=24
    i32.const 1
    i32.const 0
    call $j907
    local.get 0
    i32.load offset=28
    i32.const 1
    i32.const 0
    call $j907
    local.get 0
    i32.const 1
    i32.store8 offset=18
    local.get 0
    i32.const 0
    call $j944
    local.set 2
    local.get 0
    f32.load offset=64
    f32.const 0x1.8p+0 (;=1.5;)
    f32.mul
    local.set 3
    local.get 2
    local.get 1
    local.get 3
    local.get 1
    local.get 3
    f32.lt
    select
    f32.const 0x1.99999ap-3 (;=0.2;)
    i32.const 0
    call $j1727
    i32.const 26
    i32.const 3502396
    i32.load
    call $j1183
    drop
    local.get 0
    i32.const 1
    local.get 0
    call $j7955)
  (func $j92876 (type 56) (param i32 f32 f32 f32 i32)
    (local i32 f32 f32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    local.get 0
    i32.load offset=28
    local.get 2
    local.get 1
    i32.const 0
    i32.const 0
    i32.const 9406
    call_indirect (type 98)
    drop
    local.get 0
    i32.load offset=28
    local.set 5
    local.get 4
    i32.const 32
    i32.add
    local.get 5
    i32.const 0
    i32.const 2662
    call_indirect (type 3)
    local.get 4
    f32.load offset=40
    local.set 2
    local.get 4
    f32.load offset=36
    local.set 6
    local.get 4
    f32.load offset=32
    local.set 7
    i32.const 4524773
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3439908
      i32.const 870
      call_indirect (type 0)
      i32.const 4524773
      i32.const 1
      i32.store8
    end
    i32.const 3439908
    i32.load
    i32.load offset=92
    local.set 0
    local.get 7
    local.get 0
    f32.load offset=24
    local.get 3
    f32.mul
    f32.add
    local.set 7
    local.get 6
    local.get 0
    f32.load offset=28
    local.get 3
    f32.mul
    f32.add
    local.set 6
    local.get 4
    local.get 2
    local.get 0
    f32.load offset=32
    local.get 3
    f32.mul
    f32.add
    f32.store offset=24
    local.get 4
    local.get 4
    i32.load offset=24
    i32.store offset=8
    local.get 4
    local.get 6
    f32.store offset=20
    local.get 4
    local.get 7
    f32.store offset=16
    local.get 4
    i32.load offset=20
    local.set 0
    local.get 4
    local.get 4
    i32.load offset=16
    i32.store
    local.get 4
    local.get 0
    i32.store offset=4
    local.get 5
    local.get 4
    local.get 1
    i32.const 0
    i32.const 0
    i32.const 9404
    call_indirect (type 48)
    drop
    local.get 4
    i32.const 48
    i32.add
    global.set 0)

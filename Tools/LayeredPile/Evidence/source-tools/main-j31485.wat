  (func $j31485 (type 31) (param i32 i32 i32 f32 i32)
    (local i32 i32 i32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    local.get 0
    i32.const 0
    call $j925
    i32.const 1
    i32.const 0
    call $j907
    local.get 0
    i32.const 0
    i32.store offset=44
    local.get 1
    local.set 5
    local.get 5
    i32.load
    local.set 6
    local.get 5
    i32.load offset=4
    local.set 7
    local.get 0
    local.get 6
    i32.store offset=16
    local.get 0
    local.get 7
    i32.store offset=20
    local.get 0
    local.get 5
    i32.load offset=8
    i32.store offset=24
    local.get 3
    local.get 0
    f32.load offset=48
    f32.mul
    local.set 3
    local.get 0
    local.get 2
    i32.store offset=28
    local.get 0
    local.get 3
    f32.store offset=32
    local.get 0
    i32.const 0
    call $j944
    local.set 1
    local.get 4
    local.get 0
    i32.load offset=24
    i32.store offset=24
    local.get 0
    i32.load offset=16
    local.set 7
    local.get 0
    i32.load offset=20
    local.set 6
    local.get 4
    local.get 7
    i32.store offset=16
    local.get 4
    local.get 6
    i32.store offset=20
    local.get 1
    local.get 4
    i32.const 16
    i32.add
    i32.const 0
    call $j1386
    local.get 0
    i32.const 0
    call $j944
    local.set 1
    i32.const 4525920
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3439908
      call $j870
      i32.const 4525920
      i32.const 1
      i32.store8
    end
    i32.const 3439908
    i32.load
    i32.load offset=92
    local.set 2
    local.get 2
    local.set 5
    local.get 5
    i32.load
    local.set 6
    local.get 5
    i32.load offset=4
    local.set 7
    local.get 4
    local.get 5
    i32.load offset=8
    i32.store offset=40
    local.get 4
    local.get 4
    i32.load offset=40
    i32.store offset=8
    local.get 4
    local.get 6
    i32.store offset=32
    local.get 4
    local.get 7
    i32.store offset=36
    local.get 4
    i32.load offset=32
    local.set 7
    local.get 4
    i32.load offset=36
    local.set 6
    local.get 4
    local.get 7
    i32.store
    local.get 4
    local.get 6
    i32.store offset=4
    local.get 1
    local.get 4
    i32.const 0
    call $j1161
    local.get 0
    local.get 0
    local.get 0
    call $j31486
    i32.const 0
    call $j1135
    drop
    local.get 0
    i32.load offset=12
    i32.const 0
    call $j3343
    local.get 4
    i32.const 48
    i32.add
    global.set 0)

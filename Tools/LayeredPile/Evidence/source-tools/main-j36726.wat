  (func $j36726 (type 16) (param i32 f32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4507839
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3502500
      call $j870
      i32.const 4507839
      i32.const 1
      i32.store8
    end
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
    i32.const 0
    call $j944
    local.set 3
    local.get 2
    local.get 0
    i32.load offset=144
    i32.store offset=8
    local.get 0
    i32.load offset=140
    local.set 4
    local.get 2
    local.get 0
    i32.load offset=136
    i32.store
    local.get 2
    local.get 4
    i32.store offset=4
    local.get 3
    local.get 2
    local.get 1
    i32.const 0
    i32.const 0
    call $j3031
    i32.const 1
    i32.const 3502500
    i32.load
    call $j1281
    drop
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

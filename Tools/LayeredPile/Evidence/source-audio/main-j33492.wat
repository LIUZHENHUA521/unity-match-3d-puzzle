  (func $j33492 (type 0) (param i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4508432
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3417424
      call $j870
      i32.const 3483380
      call $j870
      i32.const 3429140
      call $j870
      i32.const 3499972
      call $j870
      i32.const 3499976
      call $j870
      i32.const 4508432
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.const 1
    i32.store offset=28
    i32.const 3429140
    i32.load
    local.set 3
    local.get 1
    i32.const 16
    i32.add
    local.set 2
    local.get 2
    i32.const -1
    i32.store offset=4
    local.get 2
    local.get 3
    i32.store
    local.get 2
    local.get 1
    i32.load offset=28
    i32.store offset=8
    local.get 2
    i32.const 0
    call $j956
    i32.const 1
    i32.const 3483380
    i32.load
    call $j1698
    local.set 2
    i32.const 3417424
    i32.load
    call $j873
    local.set 3
    local.get 3
    local.get 0
    i32.const 3499972
    i32.load
    i32.const 0
    call $j931
    local.get 2
    local.get 3
    i32.const 0
    call $j1933
    local.get 1
    i32.const 2
    i32.store offset=28
    i32.const 3429140
    i32.load
    local.set 2
    local.get 1
    i32.const -1
    i32.store offset=4
    local.get 1
    local.get 2
    i32.store
    local.get 1
    local.get 1
    i32.load offset=28
    i32.store offset=8
    local.get 1
    i32.const 0
    call $j956
    i32.const 1
    i32.const 3483380
    i32.load
    call $j1698
    local.set 2
    i32.const 3417424
    i32.load
    call $j873
    local.set 3
    local.get 3
    local.get 0
    i32.const 3499976
    i32.load
    i32.const 0
    call $j931
    local.get 2
    local.get 3
    i32.const 0
    call $j1933
    local.get 1
    i32.const 32
    i32.add
    global.set 0)

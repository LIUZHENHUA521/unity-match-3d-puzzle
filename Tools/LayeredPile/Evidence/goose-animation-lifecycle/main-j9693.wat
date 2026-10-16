  (func $j9693 (type 1) (param i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    local.get 0
    i32.load8_u offset=92
    i32.eqz
    if  ;; label = @1
      local.get 1
      if  ;; label = @2
        local.get 0
        i32.const 0
        call $j944
        local.set 1
        i32.const 4518492
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4518492
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 3
        local.get 2
        local.get 3
        i32.load offset=20
        i32.store offset=8
        local.get 3
        i32.load offset=16
        local.set 4
        local.get 2
        local.get 3
        i32.load offset=12
        i32.store
        local.get 2
        local.get 4
        i32.store offset=4
        local.get 1
        local.get 2
        i32.const 0
        call $j1161
      end
      local.get 0
      i32.const 1
      i32.store8 offset=92
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)
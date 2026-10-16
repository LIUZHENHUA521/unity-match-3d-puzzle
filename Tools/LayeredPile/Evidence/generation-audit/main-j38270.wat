  (func $j38270 (type 1) (param i32 i32 i32)
    (local i32 i32)
    local.get 1
    i32.eqz
    if  ;; label = @1
      i32.const 34
      i32.const 0
      call $j1272
    end
    local.get 0
    i32.load offset=12
    local.set 3
    local.get 3
    i32.const 2
    i32.ge_s
    if  ;; label = @1
      local.get 0
      i32.load offset=8
      local.set 4
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.const 52
      call $j874
      call $j871
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.load offset=204
      i32.load
      local.get 2
      i32.load offset=16
      i32.load offset=96
      i32.load offset=204
      i32.const 0
      local.get 4
      i32.const 0
      local.get 3
      local.get 1
      call $j3208
    end
    local.get 0
    local.get 0
    i32.load offset=16
    i32.const 1
    i32.add
    i32.store offset=16)

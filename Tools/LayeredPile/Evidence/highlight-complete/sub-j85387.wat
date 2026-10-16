  (func $j85387 (type 5) (param i32 i32 i32) (result i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4505065
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3451168
      i32.const 870
      call_indirect (type 0)
      i32.const 4505065
      i32.const 1
      i32.store8
    end
    local.get 2
    i32.const 0
    i32.store offset=12
    local.get 0
    i32.load offset=948
    local.set 0
    local.get 0
    if  ;; label = @1
      local.get 0
      local.get 1
      local.get 2
      i32.const 12
      i32.add
      i32.const 3451168
      i32.load
      i32.const 1012
      call_indirect (type 8)
      drop
      local.get 2
      i32.load offset=12
      local.set 3
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0
    local.get 3
    return)

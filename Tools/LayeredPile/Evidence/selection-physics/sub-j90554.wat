  (func $j90554 (type 48) (param i32 i32 f32 i32 i32) (result i32)
    (local i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 5
    local.get 5
    global.set 0
    i32.const 4505198
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3418920
      i32.const 870
      call_indirect (type 0)
      i32.const 3418960
      i32.const 870
      call_indirect (type 0)
      i32.const 3430336
      i32.const 870
      call_indirect (type 0)
      i32.const 3497888
      i32.const 870
      call_indirect (type 0)
      i32.const 3502472
      i32.const 870
      call_indirect (type 0)
      i32.const 3512036
      i32.const 870
      call_indirect (type 0)
      i32.const 3442204
      i32.const 870
      call_indirect (type 0)
      i32.const 4505198
      i32.const 1
      i32.store8
    end
    i32.const 3442204
    i32.load
    i32.const 873
    call_indirect (type 1)
    local.set 4
    local.get 4
    local.get 0
    i32.store offset=8
    i32.const 3418920
    i32.load
    i32.const 873
    call_indirect (type 1)
    local.set 0
    local.get 0
    local.get 4
    i32.const 3512036
    i32.load
    i32.const 1131
    call_indirect (type 3)
    local.get 4
    i32.load offset=8
    local.set 6
    i32.const 3418960
    i32.load
    i32.const 873
    call_indirect (type 1)
    local.set 7
    local.get 7
    local.get 6
    i32.const 3497888
    i32.load
    i32.const 1130
    call_indirect (type 3)
    i32.const 3430336
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 5
    local.get 1
    i32.load offset=8
    i32.store offset=8
    local.get 1
    i32.load offset=4
    local.set 6
    local.get 5
    local.get 1
    i32.load
    i32.store
    local.get 5
    local.get 6
    i32.store offset=4
    local.get 0
    local.get 7
    local.get 5
    local.get 2
    i32.const 0
    i32.const 1874
    call_indirect (type 38)
    local.set 1
    local.get 1
    local.set 0
    block  ;; label = @1
      local.get 0
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=144
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      local.get 3
      i32.store8 offset=236
    end
    local.get 0
    local.get 4
    i32.load offset=8
    i32.const 3502472
    i32.load
    i32.const 1157
    call_indirect (type 5)
    drop
    local.get 5
    i32.const 16
    i32.add
    global.set 0
    local.get 1
    return)

  (func $j69531 (type 4) (param i32 i32)
    (local i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    local.get 0
    i32.store offset=12
    i32.const 4508428
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3483356
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 3437580
      i32.const 870
      call_indirect (type 0)
      i32.const 3437584
      i32.const 870
      call_indirect (type 0)
      i32.const 4508428
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    i32.const 871
    call_indirect (type 0)
    block  ;; label = @1
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=20
      i32.load8_u offset=8
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      local.set 0
      i32.const 3435416
      i32.load
      i32.const 871
      call_indirect (type 0)
      local.get 0
      i32.const 0
      i32.const 0
      i32.const 924
      call_indirect (type 5)
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=28
      local.set 2
      i32.const 3437584
      i32.load
      local.set 0
      local.get 1
      i32.const -1
      i32.store offset=4
      local.get 1
      local.get 0
      i32.store
      local.get 1
      local.get 1
      i32.load offset=12
      i32.store offset=8
      local.get 2
      local.get 1
      i32.const 0
      i32.const 956
      call_indirect (type 2)
      i32.const 3483356
      i32.load
      i32.const 3406
      call_indirect (type 2)
      i32.const 0
      i32.const 5355
      call_indirect (type 3)
    end
    local.get 1
    i32.const 16
    i32.add
    global.set 0)

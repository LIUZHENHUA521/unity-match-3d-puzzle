  (func $j13406 (type 2) (param i32 i32) (result i32)
    i32.const 4505023
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      i32.const 870
      call_indirect (type 0)
      i32.const 3495724
      i32.const 870
      call_indirect (type 0)
      i32.const 3435416
      i32.const 870
      call_indirect (type 0)
      i32.const 4505023
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=468
    local.set 1
    i32.const 3435416
    i32.load
    i32.const 871
    call_indirect (type 0)
    local.get 0
    i32.const 468
    i32.add
    local.set 0
    block  ;; label = @1
      local.get 1
      i32.const 0
      i32.const 0
      i32.const 924
      call_indirect (type 5)
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=52
      local.set 1
      i32.const 3435416
      i32.load
      i32.const 871
      call_indirect (type 0)
      local.get 1
      i32.const 0
      i32.const 0
      i32.const 915
      call_indirect (type 5)
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      i32.const 871
      call_indirect (type 0)
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=52
      local.set 1
      i32.const 3435416
      i32.load
      i32.const 871
      call_indirect (type 0)
      local.get 0
      local.get 1
      i32.const 3495724
      i32.load
      i32.const 1764
      call_indirect (type 2)
      i32.store
    end
    local.get 0
    i32.load
    return)

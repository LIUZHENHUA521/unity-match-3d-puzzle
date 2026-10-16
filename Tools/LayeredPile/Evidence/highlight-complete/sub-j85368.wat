  (func $j85368 (type 3) (param i32 i32 i32)
    (local i32 i32 i32 i32)
    i32.const 4505015
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435576
      i32.const 870
      call_indirect (type 0)
      i32.const 4505015
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.const 324
    i32.add
    local.set 2
    local.get 0
    i32.load offset=324
    local.set 0
    loop  ;; label = @1
      local.get 0
      local.set 5
      local.get 0
      local.get 1
      i32.const 0
      i32.const 1023
      call_indirect (type 5)
      i32.const 3435576
      i32.load
      i32.const 919
      call_indirect (type 2)
      local.set 6
      local.get 2
      i32.load
      local.set 3
      local.get 0
      local.get 3
      i32.eq
      local.set 4
      local.get 2
      local.get 6
      local.get 3
      local.get 4
      select
      i32.store
      local.get 0
      local.get 3
      local.get 4
      select
      local.set 0
      local.get 5
      local.get 0
      i32.ne
      br_if 0 (;@1;)
    end)

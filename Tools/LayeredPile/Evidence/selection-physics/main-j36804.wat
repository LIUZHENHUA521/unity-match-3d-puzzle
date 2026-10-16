  (func $j36804 (type 0) (param i32 i32)
    (local i32 i32 i32)
    global.get 0
    i32.const 32
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507819
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4507819
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 2
    i32.const 3435416
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 2
      i32.const 0
      i32.const 0
      call $j924 ;; Object.op_Equality
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=12
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      local.get 0
      i32.load offset=16
      i32.const 0
      call $j1387 ;; Transform.get_localScale
      local.get 1
      i32.load offset=8
      local.set 2
      local.get 1
      local.get 2
      i32.store offset=24
      local.get 1
      i32.load offset=4
      local.set 3
      local.get 1
      i32.load
      local.set 4
      local.get 1
      local.get 4
      i32.store offset=16
      local.get 1
      local.get 3
      i32.store offset=20
      local.get 0
      local.get 2
      i32.store offset=28
      local.get 0
      local.get 4
      i32.store offset=20
      local.get 0
      local.get 3
      i32.store offset=24
    end
    local.get 1
    i32.const 32
    i32.add
    global.set 0)

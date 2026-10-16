  (func $j2319 (type 0) (param i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    local.get 1
    local.get 0
    i32.store offset=12
    i32.const 4508424
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3454736
      call $j870
      i32.const 3454740
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3439432
      call $j870
      i32.const 4508424
      i32.const 1
      i32.store8
    end
    i32.const 3437580
    i32.load
    call $j871
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
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      local.set 0
      i32.const 3435416
      i32.load
      call $j871
      local.get 0
      i32.const 0
      i32.const 0
      call $j924
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.set 2
      i32.const 3439432
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
      call $j956
      i32.const 3454736
      i32.load
      call $j991
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=12
      local.set 2
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.set 3
      i32.const 3439432
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
      local.get 3
      local.get 1
      i32.const 0
      call $j956
      i32.const 3454740
      i32.load
      call $j989
      i32.const 0
      call $j5355
    end
    local.get 1
    i32.const 16
    i32.add
    global.set 0)

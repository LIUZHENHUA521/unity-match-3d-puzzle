  (func $j9354 (type 0) (param i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4508426
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3417424
      call $j870
      i32.const 3483380
      call $j870
      i32.const 3454736
      call $j870
      i32.const 3471620
      call $j870
      i32.const 3471632
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3437584
      call $j870
      i32.const 3516880
      call $j870
      i32.const 3445096
      call $j870
      i32.const 4508426
      i32.const 1
      i32.store8
    end
    i32.const 3445096
    i32.load
    call $j873
    local.set 1
    local.get 1
    local.get 0
    i32.store offset=8
    local.get 1
    i32.load offset=8
    local.set 0
    local.get 2
    i32.const 0
    i32.store offset=12
    local.get 0
    local.set 3
    i32.const 3437584
    i32.load
    local.set 0
    local.get 2
    i32.const -1
    i32.store offset=4
    local.get 2
    local.get 0
    i32.store
    local.get 2
    local.get 2
    i32.load offset=12
    i32.store offset=8
    block  ;; label = @1
      local.get 3
      local.get 2
      i32.const 0
      call $j956
      i32.const 0
      call $j899
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.get 1
      i32.load offset=8
      i32.const 3454736
      i32.load
      call $j991
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=4
      local.get 1
      i32.load offset=8
      i32.const 3471632
      i32.load
      call $j1235
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=4
      local.get 1
      i32.load offset=8
      i32.const 3471620
      i32.load
      call $j903
      local.get 1
      i32.load offset=8
      i32.const 1
      i32.const 3483380
      i32.load
      call $j1698
      local.set 0
      i32.const 3417424
      i32.load
      call $j873
      local.set 3
      local.get 3
      local.get 1
      i32.const 3516880
      i32.load
      i32.const 0
      call $j931
      local.get 0
      local.get 3
      i32.const 0
      call $j1933
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

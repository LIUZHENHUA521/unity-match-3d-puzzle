  (func $j3492 (type 7) (param i32 i32 i32 i32)
    (local i32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    local.get 3
    local.get 0
    i32.store offset=12
    i32.const 4508430
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3454736
      call $j870
      i32.const 3454740
      call $j870
      i32.const 3471620
      call $j870
      i32.const 3471628
      call $j870
      i32.const 3471676
      call $j870
      i32.const 3471680
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 3437584
      call $j870
      i32.const 4508430
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
      local.set 4
      i32.const 3435416
      i32.load
      call $j871
      local.get 4
      i32.const 0
      i32.const 0
      call $j924
      br_if 0 (;@1;)
      i32.const 3437580
      i32.load
      call $j871
      local.get 0
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=12
      i32.eq
      if  ;; label = @2
        i32.const 0
        call $j1519
        local.set 5
        i32.const 3437580
        i32.load
        call $j871
        local.get 5
        i32.const 3437580
        i32.load
        i32.load offset=92
        f32.load offset=16
        f32.sub
        f32.const 0x1.99999ap-5 (;=0.05;)
        f32.lt
        br_if 1 (;@1;)
      end
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      local.get 0
      i32.store offset=12
      i32.const 0
      call $j1519
      local.set 5
      i32.const 3437580
      i32.load
      i32.load offset=92
      local.get 5
      f32.store offset=16
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.set 4
      i32.const 3437584
      i32.load
      local.set 0
      local.get 3
      i32.const -1
      i32.store offset=4
      local.get 3
      local.get 0
      i32.store
      local.get 3
      local.get 3
      i32.load offset=12
      i32.store offset=8
      local.get 4
      local.get 3
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
      local.set 0
      local.get 1
      i32.eqz
      if  ;; label = @2
        local.get 0
        i32.load
        i32.load offset=32
        i32.load offset=16
        local.set 1
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=8
        local.set 2
        i32.const 3437584
        i32.load
        local.set 0
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 0
        i32.store
        local.get 3
        local.get 3
        i32.load offset=12
        i32.store offset=8
        local.get 1
        local.get 2
        local.get 3
        i32.const 0
        call $j956
        i32.const 3454740
        i32.load
        call $j989
        i32.const 0
        call $j5355
        br 1 (;@1;)
      end
      i32.const 3471676
      i32.load
      drop
      local.get 0
      i32.load offset=24
      i32.load offset=12
      local.set 1
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=24
      local.set 0
      local.get 1
      i32.eqz
      if  ;; label = @2
        local.get 0
        local.get 2
        i32.const 3471620
        i32.load
        call $j903
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=32
        i32.load offset=16
        local.set 1
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=8
        local.set 2
        i32.const 3437584
        i32.load
        local.set 0
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 0
        i32.store
        local.get 3
        local.get 3
        i32.load offset=12
        i32.store offset=8
        local.get 1
        local.get 2
        local.get 3
        i32.const 0
        call $j956
        i32.const 3454740
        i32.load
        call $j989
        i32.const 0
        call $j5355
        br 1 (;@1;)
      end
      block  ;; label = @2
        local.get 2
        local.get 0
        i32.const 0
        i32.const 3471680
        i32.load
        call $j895
        i32.const 0
        call $j899
        i32.eqz
        br_if 0 (;@2;)
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3471676
        i32.load
        drop
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=24
        i32.load offset=12
        i32.const 2
        i32.gt_s
        br_if 0 (;@2;)
        i32.const 3437580
        i32.load
        call $j871
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=24
        local.get 2
        i32.const 3471620
        i32.load
        call $j903
        i32.const 3471676
        i32.load
        drop
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        i32.load offset=32
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=24
        i32.load offset=12
        i32.const 1
        i32.sub
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 1
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load offset=8
        local.set 2
        i32.const 3437584
        i32.load
        local.set 0
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 0
        i32.store
        local.get 3
        local.get 3
        i32.load offset=12
        i32.store offset=8
        local.get 1
        local.get 2
        local.get 3
        i32.const 0
        call $j956
        i32.const 3454740
        i32.load
        call $j989
        i32.const 0
        call $j5355
        br 1 (;@1;)
      end
      i32.const 3437580
      i32.load
      call $j871
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=24
      i32.const 3471628
      i32.load
      call $j974
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=24
      local.get 2
      i32.const 3471620
      i32.load
      call $j903
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      i32.load offset=32
      i32.load offset=16
      local.set 1
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.set 2
      i32.const 3437584
      i32.load
      local.set 0
      local.get 3
      i32.const -1
      i32.store offset=4
      local.get 3
      local.get 0
      i32.store
      local.get 3
      local.get 3
      i32.load offset=12
      i32.store offset=8
      local.get 1
      local.get 2
      local.get 3
      i32.const 0
      call $j956
      i32.const 3454740
      i32.load
      call $j989
      i32.const 0
      call $j5355
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

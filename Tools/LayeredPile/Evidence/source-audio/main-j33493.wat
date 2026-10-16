  (func $j33493 (type 0) (param i32 i32)
    (local i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4508423
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870
      i32.const 3437584
      call $j870
      i32.const 3439432
      call $j870
      i32.const 4508423
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 1
    loop  ;; label = @1
      local.get 0
      i32.load offset=20
      local.set 2
      local.get 2
      i32.load offset=12
      local.get 1
      i32.gt_s
      if  ;; label = @2
        local.get 2
        local.get 1
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 2
        i32.const 3437580
        i32.load
        call $j871
        local.get 2
        local.get 3
        call $j9353
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    i32.const 0
    local.set 1
    loop  ;; label = @1
      local.get 0
      i32.load offset=16
      local.set 2
      local.get 2
      i32.load offset=12
      local.get 1
      i32.gt_s
      if  ;; label = @2
        i32.const 3439432
        i32.load
        local.set 4
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 4
        i32.store
        local.get 3
        local.get 2
        local.get 1
        i32.const 2
        i32.shl
        i32.add
        i32.const 16
        i32.add
        i32.load
        i32.store offset=8
        local.get 3
        i32.const 0
        call $j956
        local.set 2
        i32.const 3437580
        i32.load
        call $j871
        local.get 2
        local.get 3
        call $j9354
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    i32.const 0
    local.set 1
    loop  ;; label = @1
      local.get 0
      i32.load offset=24
      local.set 2
      local.get 2
      i32.load offset=12
      local.get 1
      i32.gt_s
      if  ;; label = @2
        i32.const 3437584
        i32.load
        local.set 4
        local.get 3
        i32.const -1
        i32.store offset=4
        local.get 3
        local.get 4
        i32.store
        local.get 3
        local.get 2
        local.get 1
        i32.const 2
        i32.shl
        i32.add
        i32.const 16
        i32.add
        i32.load
        i32.store offset=8
        local.get 3
        i32.const 0
        call $j956
        local.set 2
        i32.const 3437580
        i32.load
        call $j871
        local.get 2
        local.get 3
        call $j9354
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

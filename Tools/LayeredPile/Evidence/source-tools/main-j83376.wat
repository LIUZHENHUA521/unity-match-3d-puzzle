  (func $j83376 (type 1) (param i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4507811
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3455940
      call $j870
      i32.const 3455932
      call $j870
      i32.const 3457596
      call $j870
      i32.const 3457636
      call $j870
      i32.const 3420516
      call $j870
      i32.const 3434152
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434332
      call $j870
      i32.const 3438836
      call $j870
      i32.const 3439168
      call $j870
      i32.const 3546856
      call $j870
      i32.const 3565672
      call $j870
      i32.const 4507811
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      block  ;; label = @2
        i32.const 3439168
        i32.load
        i32.load offset=92
        i32.load8_u offset=8
        if  ;; label = @3
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load
          local.get 1
          i32.const 0
          call $j13232
          br_if 1 (;@2;)
          br 2 (;@1;)
        end
        i32.const 3434332
        i32.load
        call $j871
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.get 1
        i32.const 3457636
        i32.load
        call $j1060
        i32.const 1
        i32.lt_s
        br_if 0 (;@2;)
        i32.const 3434312
        i32.load
        call $j871
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load
        local.get 1
        i32.const 0
        call $j13232
        i32.eqz
        br_if 1 (;@1;)
      end
      i32.const 3434332
      i32.load
      call $j871
      block  ;; label = @2
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=12
        local.get 1
        i32.const 3457596
        i32.load
        call $j8243
        br_if 0 (;@2;)
        i32.const 3439168
        i32.load
        i32.load offset=92
        i32.load8_u offset=8
        br_if 0 (;@2;)
        i32.const 3434152
        i32.load
        call $j871
        i32.const 600
        i32.const 3565672
        i32.load
        i32.const 0
        call $j1117
        br 1 (;@1;)
      end
      local.get 1
      i32.const 6
      i32.gt_u
      br_if 0 (;@1;)
      i32.const 1
      local.get 1
      i32.shl
      i32.const 103
      i32.and
      i32.eqz
      if  ;; label = @2
        i32.const 3420516
        i32.load
        call $j873
        local.set 0
        local.get 0
        i32.const 3455932
        i32.load
        call $j920
        local.get 2
        local.get 1
        i32.store offset=12
        i32.const 3438836
        i32.load
        local.get 2
        i32.const 12
        i32.add
        call $j886
        local.set 1
        local.get 0
        i32.const 3546856
        i32.load
        local.get 1
        i32.const 3455940
        i32.load
        call $j904
        i32.const 3434152
        i32.load
        call $j871
        i32.const 200
        local.get 0
        i32.const 0
        i32.const 0
        call $j1091
        br 1 (;@1;)
      end
      i32.const 3434332
      i32.load
      call $j871
      block  ;; label = @2
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.get 1
        i32.const 3457636
        i32.load
        call $j1060
        i32.const 0
        i32.gt_s
        br_if 0 (;@2;)
        i32.const 3439168
        i32.load
        i32.load offset=92
        i32.load8_u offset=8
        br_if 0 (;@2;)
        i32.const 3420516
        i32.load
        call $j873
        local.set 0
        local.get 0
        i32.const 3455932
        i32.load
        call $j920
        local.get 2
        local.get 1
        i32.store offset=8
        i32.const 3438836
        i32.load
        local.get 2
        i32.const 8
        i32.add
        call $j886
        local.set 1
        local.get 0
        i32.const 3546856
        i32.load
        local.get 1
        i32.const 3455940
        i32.load
        call $j904
        i32.const 3434152
        i32.load
        call $j871
        i32.const 200
        local.get 0
        i32.const 0
        i32.const 0
        call $j1091
        br 1 (;@1;)
      end
      local.get 0
      local.get 1
      i32.const 0
      local.get 1
      call $j8062
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

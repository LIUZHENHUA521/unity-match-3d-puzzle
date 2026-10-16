  (func $j12985 (type 20) (param i32 i32 f32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 5
    local.get 5
    global.set 0
    i32.const 4507832
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3438972
      call $j870
      i32.const 3502300
      call $j870
      i32.const 3502396
      call $j870
      i32.const 3502500
      call $j870
      i32.const 3515504
      call $j870
      i32.const 3515508
      call $j870
      i32.const 3444164
      call $j870
      i32.const 4507832
      i32.const 1
      i32.store8
    end
    i32.const 3444164
    i32.load
    call $j873
    local.set 3
    local.get 3
    local.get 0
    i32.store offset=8
    local.get 3
    local.get 2
    f32.store offset=12
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=16
      i32.eqz
      br_if 0 (;@1;)
      local.get 5
      local.get 0
      i32.const 0
      call $j944
      i32.const 0
      call $j1387
      local.get 3
      local.get 5
      f32.load
      f32.store offset=16
      local.get 0
      i32.const 0
      call $j944
      i32.const 0
      i32.const 0
      call $j1360
      drop
      local.get 0
      i32.const 1
      local.get 0
      call $j6602
      local.get 0
      i32.const 0
      i32.store8 offset=19
      local.get 0
      i32.const 0
      i32.store8 offset=16
      local.get 0
      i32.const 0
      local.get 0
      call $j7954
      local.get 0
      local.get 0
      local.get 0
      call $j36728
      i32.const 0
      call $j1135
      drop
      block  ;; label = @2
        block  ;; label = @3
          local.get 1
          i32.eqz
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          i32.const 0
          call $j5871
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load
          local.set 1
          i32.const 3435416
          i32.load
          call $j871
          block  ;; label = @4
            local.get 1
            i32.const 0
            call $j982
            i32.eqz
            br_if 0 (;@4;)
            i32.const 3434312
            i32.load
            call $j871
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load
            i32.load8_u offset=600
            i32.eqz
            br_if 0 (;@4;)
            local.get 0
            i32.const 0
            call $j944
            local.set 1
            local.get 3
            f32.load offset=16
            local.set 2
            i32.const 3434312
            i32.load
            call $j871
            local.get 1
            local.get 2
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load
            f32.load offset=616
            f32.mul
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load
            f32.load offset=612
            i32.const 0
            call $j1727
            i32.const 1
            i32.const 3502500
            i32.load
            call $j1281
            local.set 1
            i32.const 3438972
            i32.load
            call $j873
            local.set 4
            local.get 4
            local.get 3
            i32.const 3515504
            i32.load
            i32.const 0
            call $j1486
            local.get 1
            local.get 4
            i32.const 3502300
            i32.load
            call $j1889
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load
            i32.load offset=604
            i32.const 3502396
            i32.load
            call $j1183
            drop
            br 2 (;@2;)
          end
          local.get 0
          i32.const 0
          call $j944
          local.get 0
          f32.load offset=64
          local.get 3
          f32.load offset=12
          i32.const 0
          call $j1727
          i32.const 1
          i32.const 3502500
          i32.load
          call $j1281
          local.set 1
          i32.const 3438972
          i32.load
          call $j873
          local.set 4
          local.get 4
          local.get 3
          i32.const 3515508
          i32.load
          i32.const 0
          call $j1486
          local.get 1
          local.get 4
          i32.const 3502300
          i32.load
          call $j1889
          drop
          br 1 (;@2;)
        end
        local.get 0
        i32.const 1
        i32.store8 offset=93
      end
      local.get 0
      i32.const 1
      local.get 0
      call $j7955
      local.get 0
      i32.load offset=124
      local.set 1
      i32.const 3435416
      i32.load
      call $j871
      local.get 1
      i32.const 0
      call $j982
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=124
      local.set 0
      local.get 0
      local.set 1
      local.get 0
      i32.load
      local.set 0
      local.get 0
      i32.load offset=244
      local.get 1
      local.get 0
      i32.load offset=248
      call $redir_vii
    end
    local.get 5
    i32.const 16
    i32.add
    global.set 0)

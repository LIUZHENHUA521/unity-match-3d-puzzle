  (func $j65504 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4507910
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435480
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3440340
      call $j870
      i32.const 4507910
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 1
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                local.get 0
                i32.load offset=8
                br_table 0 (;@6;) 1 (;@5;) 2 (;@4;) 5 (;@1;)
              end
              local.get 0
              i32.const -1
              i32.store offset=8
              br 2 (;@3;)
            end
            local.get 0
            i32.const -1
            i32.store offset=8
            local.get 1
            f32.load offset=24
            local.get 1
            f32.load offset=20
            f32.ge
            i32.eqz
            br_if 1 (;@3;)
            br 2 (;@2;)
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          local.get 1
          i32.const 0
          call $j925
          local.set 0
          i32.const 3435416
          i32.load
          call $j871
          local.get 0
          i32.const 0
          call $j982
          i32.eqz
          br_if 2 (;@1;)
          local.get 1
          i32.const 0
          call $j925
          local.set 0
          i32.const 3435480
          i32.load
          call $j871
          local.get 0
          i32.const 0
          i32.const 1
          i32.const 0
          call $j2538
          local.get 1
          i32.load offset=12
          i32.const 0
          call $j9665
          br 2 (;@1;)
        end
        local.get 1
        i32.load offset=16
        local.set 3
        i32.const 3435416
        i32.load
        call $j871
        local.get 3
        i32.const 0
        i32.const 0
        call $j915
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        i32.load offset=16
        i32.const 0
        call $j1418
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        i32.const 0
        call $j944
        local.set 3
        local.get 2
        i32.const 32
        i32.add
        local.get 1
        i32.load offset=16
        i32.const 0
        call $j1027
        i32.const 0
        call $j1028
        local.get 2
        i32.load offset=40
        local.set 4
        local.get 2
        local.get 4
        i32.store offset=24
        local.get 2
        local.get 4
        i32.store offset=8
        local.get 2
        i32.load offset=36
        local.set 4
        local.get 2
        i32.load offset=32
        local.set 5
        local.get 2
        local.get 5
        i32.store offset=16
        local.get 2
        local.get 4
        i32.store offset=20
        local.get 2
        local.get 5
        i32.store
        local.get 2
        local.get 4
        i32.store offset=4
        local.get 3
        local.get 2
        i32.const 0
        call $j1386
        local.get 1
        local.get 1
        f32.load offset=24
        i32.const 0
        call $j1511
        f32.add
        f32.store offset=24
        local.get 0
        i32.const 0
        i32.store offset=12
        i32.const 1
        local.set 3
        local.get 0
        i32.const 1
        i32.store offset=8
        br 1 (;@1;)
      end
      i32.const 3440340
      i32.load
      call $j873
      local.set 1
      local.get 1
      i32.const -1082130432
      i32.store offset=12
      local.get 1
      f32.const 0x1p+1 (;=2;)
      f32.store offset=8
      local.get 0
      local.get 1
      i32.store offset=12
      local.get 0
      i32.const 2
      i32.store offset=8
      i32.const 1
      local.set 3
    end
    local.get 2
    i32.const 48
    i32.add
    global.set 0
    local.get 3
    return)

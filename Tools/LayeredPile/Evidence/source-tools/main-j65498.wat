  (func $j65498 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 96
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507907
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435480
      call $j870
      i32.const 3435416
      call $j870
      i32.const 4507907
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 2
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              local.get 0
              i32.load offset=8
              br_table 0 (;@5;) 1 (;@4;) 4 (;@1;)
            end
            local.get 0
            i32.const -1
            i32.store offset=8
            br 1 (;@3;)
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          local.get 2
          f32.load offset=44
          local.get 2
          f32.load offset=32
          f32.ge
          br_if 1 (;@2;)
        end
        local.get 2
        i32.load offset=28
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
        local.get 2
        i32.load offset=28
        i32.const 0
        call $j1418
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        i32.const 80
        i32.add
        local.get 2
        i32.load offset=28
        i32.const 0
        call $j1027
        i32.const 0
        call $j1028
        local.get 1
        f32.load offset=88
        local.set 9
        local.get 1
        f32.load offset=84
        local.set 8
        local.get 1
        f32.load offset=80
        local.set 7
        local.get 1
        i32.const 80
        i32.add
        local.get 2
        i32.const 0
        call $j944
        i32.const 0
        call $j1028
        local.get 1
        f32.load offset=88
        local.set 10
        local.get 1
        f32.load offset=84
        local.set 11
        local.get 1
        f32.load offset=80
        local.set 12
        i32.const 4509457
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3434688
          call $j870
          i32.const 4509457
          i32.const 1
          i32.store8
        end
        local.get 7
        local.get 12
        f32.sub
        local.set 7
        local.get 8
        local.get 11
        f32.sub
        local.set 8
        local.get 9
        local.get 10
        f32.sub
        local.set 9
        i32.const 3434688
        i32.load
        call $j871
        local.get 7
        local.get 7
        f32.mul
        local.get 8
        local.get 8
        f32.mul
        f32.add
        local.get 9
        local.get 9
        f32.mul
        f32.add
        local.set 7
        local.get 2
        i32.const 0
        call $j944
        local.set 6
        local.get 1
        i32.const -64
        i32.sub
        local.get 2
        i32.load offset=28
        i32.const 0
        call $j1027
        i32.const 0
        call $j1028
        local.get 1
        local.get 1
        i32.load offset=72
        i32.store offset=88
        local.get 1
        i32.load offset=64
        local.set 4
        local.get 1
        i32.load offset=68
        local.set 5
        local.get 1
        local.get 4
        i32.store offset=80
        local.get 1
        local.get 5
        i32.store offset=84
        i32.const 4524773
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4524773
          i32.const 1
          i32.store8
        end
        local.get 7
        f32.sqrt
        local.set 8
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 3
        local.get 1
        local.get 1
        i32.load offset=88
        i32.store offset=40
        local.get 1
        i32.load offset=80
        local.set 5
        local.get 1
        i32.load offset=84
        local.set 4
        local.get 1
        local.get 5
        i32.store offset=32
        local.get 1
        local.get 4
        i32.store offset=36
        local.get 1
        local.get 3
        i32.load offset=32
        i32.store offset=24
        local.get 3
        i32.load offset=24
        local.set 4
        local.get 3
        i32.load offset=28
        local.set 5
        local.get 1
        local.get 4
        i32.store offset=16
        local.get 1
        local.get 5
        i32.store offset=20
        local.get 6
        local.get 1
        i32.const 32
        i32.add
        local.get 1
        i32.const 16
        i32.add
        i32.const 0
        call $j12087
        local.get 2
        i32.const 0
        call $j944
        local.set 3
        local.get 2
        f32.load offset=40
        local.set 7
        local.get 1
        local.get 8
        local.get 2
        f32.load offset=36
        f32.mul
        f32.store offset=56
        local.get 1
        local.get 1
        i32.load offset=56
        i32.store offset=8
        local.get 1
        local.get 7
        f32.store offset=52
        local.get 1
        local.get 7
        f32.store offset=48
        local.get 1
        i32.load offset=48
        local.set 5
        local.get 1
        i32.load offset=52
        local.set 4
        local.get 1
        local.get 5
        i32.store
        local.get 1
        local.get 4
        i32.store offset=4
        local.get 3
        local.get 1
        i32.const 0
        call $j1161
        local.get 2
        local.get 2
        f32.load offset=44
        i32.const 0
        call $j1511
        f32.add
        f32.store offset=44
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
      i32.const 0
      local.set 3
      local.get 2
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
      br_if 0 (;@1;)
      local.get 2
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
      local.get 2
      i32.load offset=12
      i32.const 0
      call $j9665
    end
    local.get 1
    i32.const 96
    i32.add
    global.set 0
    local.get 3
    return)
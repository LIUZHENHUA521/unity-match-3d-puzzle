  (func $j6603 (type 7) (param i32 i32 i32 i32)
    (local i32 i32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 96
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4507830
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3502396
      call $j870
      i32.const 4507830
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=16
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=92
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=18
      br_if 0 (;@1;)
      local.get 0
      local.get 1
      i32.store8 offset=19
      block  ;; label = @2
        local.get 1
        if  ;; label = @3
          local.get 0
          i32.const 0
          local.get 0
          call $j7954
          i32.const 3434312
          i32.load
          call $j871
          block  ;; label = @4
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load offset=8
            i32.const 2
            i32.ne
            br_if 0 (;@4;)
            i32.const 3434312
            i32.load
            call $j871
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load offset=12
            i32.const 2
            i32.eq
            br_if 0 (;@4;)
            local.get 0
            local.get 0
            call $j19262
            br_if 0 (;@4;)
            local.get 0
            i32.const 0
            call $j944
            i32.const 0
            i32.const 0
            call $j1360
            drop
            local.get 3
            i32.const 80
            i32.add
            local.get 0
            i32.const 0
            call $j944
            i32.const 0
            call $j1028
            local.get 3
            f32.load offset=88
            local.set 8
            local.get 3
            f32.load offset=84
            local.set 7
            local.get 3
            f32.load offset=80
            local.set 9
            local.get 3
            i32.const 80
            i32.add
            local.get 2
            i32.const 0
            call $j2886
            local.get 0
            f32.load offset=84
            local.set 6
            local.get 3
            f32.load offset=80
            local.get 6
            f32.mul
            local.set 10
            local.get 7
            local.get 3
            f32.load offset=84
            local.get 6
            f32.mul
            f32.add
            local.set 7
            local.get 0
            local.get 8
            local.get 3
            f32.load offset=88
            local.get 6
            f32.mul
            f32.add
            f32.store offset=80
            local.get 0
            local.get 7
            f32.store offset=76
            local.get 0
            local.get 9
            local.get 10
            f32.add
            f32.store offset=72
            local.get 0
            i32.const 0
            call $j944
            local.set 1
            local.get 0
            f32.load offset=88
            local.set 6
            local.get 3
            local.get 0
            i32.load offset=80
            i32.store offset=8
            local.get 0
            i32.load offset=72
            local.set 2
            local.get 0
            i32.load offset=76
            local.set 4
            local.get 3
            local.get 2
            i32.store
            local.get 3
            local.get 4
            i32.store offset=4
            local.get 1
            local.get 3
            local.get 6
            i32.const 0
            i32.const 0
            call $j3031
            i32.const 1
            i32.const 3502396
            i32.load
            call $j1183
            drop
          end
          local.get 0
          i32.load offset=120
          local.set 1
          i32.const 3435416
          i32.load
          call $j871
          local.get 1
          i32.const 0
          call $j982
          i32.eqz
          br_if 1 (;@2;)
          local.get 0
          i32.load offset=120
          local.get 0
          call $j36803
          br 1 (;@2;)
        end
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
        call $j7954
        local.get 0
        i32.load offset=20
        local.set 1
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 2
        local.get 2
        local.set 5
        local.get 5
        i32.load
        local.set 4
        local.get 5
        i32.load offset=4
        local.set 2
        local.get 3
        local.get 5
        i32.load offset=8
        i32.store offset=72
        local.get 3
        local.get 3
        i32.load offset=72
        i32.store offset=40
        local.get 3
        local.get 4
        i32.store offset=64
        local.get 3
        local.get 2
        i32.store offset=68
        local.get 3
        i32.load offset=64
        local.set 2
        local.get 3
        i32.load offset=68
        local.set 4
        local.get 3
        local.get 2
        i32.store offset=32
        local.get 3
        local.get 4
        i32.store offset=36
        local.get 1
        local.get 3
        i32.const 32
        i32.add
        i32.const 0
        call $j6538
        local.get 0
        i32.load offset=20
        local.set 1
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 2
        local.get 2
        local.set 5
        local.get 5
        i32.load
        local.set 4
        local.get 5
        i32.load offset=4
        local.set 2
        local.get 3
        local.get 5
        i32.load offset=8
        i32.store offset=56
        local.get 3
        local.get 3
        i32.load offset=56
        i32.store offset=24
        local.get 3
        local.get 4
        i32.store offset=48
        local.get 3
        local.get 2
        i32.store offset=52
        local.get 3
        i32.load offset=48
        local.set 2
        local.get 3
        i32.load offset=52
        local.set 4
        local.get 3
        local.get 2
        i32.store offset=16
        local.get 3
        local.get 4
        i32.store offset=20
        local.get 1
        local.get 3
        i32.const 16
        i32.add
        i32.const 0
        call $j18665
      end
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
      i32.load offset=236
      local.get 1
      local.get 0
      i32.const 48
      i32.add
      i32.load offset=192
      call $redir_vii
    end
    local.get 3
    i32.const 96
    i32.add
    global.set 0)

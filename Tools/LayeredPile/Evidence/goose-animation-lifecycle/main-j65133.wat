  (func $j65133 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 i32 f32 f32 f32 f32 f32)
    global.get 0
    i32.const 80
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507768
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 3470264
      call $j870
      i32.const 3470268
      call $j870
      i32.const 3438764
      call $j870
      i32.const 4507768
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=8
    local.set 7
    local.get 7
    i32.const 1
    i32.le_u
    if  ;; label = @1
      local.get 0
      i32.load offset=16
      local.set 2
      local.get 0
      i32.const -1
      i32.store offset=8
      loop  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        local.get 3
        local.get 2
        i32.load offset=192
        i32.load offset=12
        i32.lt_s
        if  ;; label = @3
          local.get 1
          i32.const -64
          i32.sub
          local.get 2
          i32.load offset=192
          local.get 3
          i32.const 3470268
          i32.load
          call $j895
          i32.const 0
          call $j944
          i32.const 0
          call $j1387
          block  ;; label = @4
            block  ;; label = @5
              local.get 1
              f32.load offset=64
              f32.const 0x1.fae148p-1 (;=0.99;)
              f32.lt
              i32.eqz
              br_if 0 (;@5;)
              local.get 2
              i32.load8_u offset=552
              i32.eqz
              br_if 0 (;@5;)
              local.get 2
              i32.load offset=192
              local.get 3
              i32.const 3470268
              i32.load
              call $j895
              i32.const 0
              call $j944
              local.set 5
              local.get 1
              i32.const -64
              i32.sub
              local.get 5
              i32.const 0
              call $j1387
              local.get 1
              f32.load offset=72
              local.set 9
              local.get 1
              f32.load offset=68
              local.set 8
              local.get 1
              f32.load offset=64
              local.set 11
              i32.const 4518492
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3439908
                call $j870
                i32.const 4518492
                i32.const 1
                i32.store8
              end
              i32.const 3439908
              i32.load
              i32.load offset=92
              local.set 4
              local.get 2
              i32.const 496
              i32.const 480
              local.get 2
              i32.load8_u offset=500
              select
              i32.add
              f32.load
              local.set 10
              local.get 4
              f32.load offset=12
              local.get 10
              f32.mul
              local.set 12
              local.get 8
              local.get 4
              f32.load offset=16
              local.get 10
              f32.mul
              f32.add
              local.set 8
              local.get 1
              local.get 9
              local.get 4
              f32.load offset=20
              local.get 10
              f32.mul
              f32.add
              f32.store offset=56
              local.get 1
              local.get 1
              i32.load offset=56
              i32.store offset=8
              local.get 1
              local.get 8
              f32.store offset=52
              local.get 1
              local.get 11
              local.get 12
              f32.add
              f32.store offset=48
              local.get 1
              i32.load offset=48
              local.set 4
              local.get 1
              i32.load offset=52
              local.set 6
              local.get 1
              local.get 4
              i32.store
              local.get 1
              local.get 6
              i32.store offset=4
              local.get 5
              local.get 1
              i32.const 0
              call $j1161
              br 1 (;@4;)
            end
            local.get 2
            i32.load8_u offset=500
            if  ;; label = @5
              i32.const 3434312
              i32.load
              call $j871
              i32.const 3434312
              i32.load
              i32.load offset=92
              i32.load offset=12
              i32.eqz
              br_if 1 (;@4;)
            end
            local.get 2
            local.get 2
            i32.load offset=192
            local.get 3
            i32.const 3470268
            i32.load
            call $j895
            i32.const 1
            f32.const 0x1p+0 (;=1;)
            i32.const 0
            call $j5869
          end
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          br 1 (;@2;)
        end
      end
      i32.const 3434312
      i32.load
      call $j871
      block  ;; label = @2
        i32.const 0
        call $j1923
        i32.eqz
        br_if 0 (;@2;)
        i32.const 0
        local.set 3
        loop  ;; label = @3
          i32.const 3470264
          i32.load
          drop
          local.get 3
          local.get 2
          i32.load offset=208
          i32.load offset=12
          i32.ge_s
          br_if 1 (;@2;)
          local.get 1
          i32.const -64
          i32.sub
          local.get 2
          i32.load offset=208
          local.get 3
          i32.const 3470268
          i32.load
          call $j895
          i32.const 0
          call $j944
          i32.const 0
          call $j1387
          block  ;; label = @4
            block  ;; label = @5
              local.get 1
              f32.load offset=64
              f32.const 0x1.fae148p-1 (;=0.99;)
              f32.lt
              i32.eqz
              br_if 0 (;@5;)
              local.get 2
              i32.load8_u offset=552
              i32.eqz
              br_if 0 (;@5;)
              local.get 2
              i32.load offset=208
              local.get 3
              i32.const 3470268
              i32.load
              call $j895
              i32.const 0
              call $j944
              local.set 5
              local.get 1
              i32.const -64
              i32.sub
              local.get 5
              i32.const 0
              call $j1387
              local.get 1
              f32.load offset=72
              local.set 10
              local.get 1
              f32.load offset=68
              local.set 9
              local.get 1
              f32.load offset=64
              local.set 8
              i32.const 4518492
              i32.load8_u
              i32.eqz
              if  ;; label = @6
                i32.const 3439908
                call $j870
                i32.const 4518492
                i32.const 1
                i32.store8
              end
              i32.const 3439908
              i32.load
              i32.load offset=92
              local.set 4
              local.get 4
              f32.load offset=20
              local.set 11
              local.get 8
              local.get 4
              f32.load offset=12
              f32.const 0x1.99999ap-4 (;=0.1;)
              f32.mul
              f32.add
              local.set 8
              local.get 9
              local.get 4
              f32.load offset=16
              f32.const 0x1.99999ap-4 (;=0.1;)
              f32.mul
              f32.add
              local.set 9
              local.get 1
              local.get 10
              local.get 11
              f32.const 0x1.99999ap-4 (;=0.1;)
              f32.mul
              f32.add
              f32.store offset=40
              local.get 1
              local.get 1
              i32.load offset=40
              i32.store offset=24
              local.get 1
              local.get 9
              f32.store offset=36
              local.get 1
              local.get 8
              f32.store offset=32
              local.get 1
              i32.load offset=32
              local.set 6
              local.get 1
              i32.load offset=36
              local.set 4
              local.get 1
              local.get 6
              i32.store offset=16
              local.get 1
              local.get 4
              i32.store offset=20
              local.get 5
              local.get 1
              i32.const 16
              i32.add
              i32.const 0
              call $j1161
              br 1 (;@4;)
            end
            local.get 2
            local.get 2
            i32.load offset=208
            local.get 3
            i32.const 3470268
            i32.load
            call $j895
            i32.const 2
            f32.const 0x1p+0 (;=1;)
            i32.const 0
            call $j5869
          end
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          br 0 (;@3;)
        end
        unreachable
      end
      i32.const 3438764
      i32.load
      call $j871
      local.get 0
      i32.const 3438764
      i32.load
      i32.load offset=92
      i32.load offset=12
      i32.store offset=12
      local.get 0
      i32.const 1
      i32.store offset=8
    end
    local.get 1
    i32.const 80
    i32.add
    global.set 0
    local.get 7
    i32.const 2
    i32.lt_u
    return)
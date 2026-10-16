  (func $j83886 (type 0) (param i32 i32)
    (local i32 i32 i32 i32 i32 f32 f32 f32)
    block  ;; label = @1
      local.get 0
      f32.load offset=24
      f32.const 0x0p+0 (;=0;)
      f32.eq
      br_if 0 (;@1;)
      local.get 0
      i32.const 0
      i32.store offset=28
      loop  ;; label = @2
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        i32.load
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        local.get 0
        i32.load offset=8
        call $j917
        local.get 3
        i32.gt_s
        if  ;; label = @3
          local.get 0
          i32.load offset=32
          local.get 3
          i32.ne
          if  ;; label = @4
            local.get 0
            f32.load offset=28
            local.set 7
            local.get 1
            i32.load offset=16
            i32.load offset=96
            i32.load offset=40
            i32.load
            local.get 1
            i32.load offset=16
            i32.load offset=96
            i32.load offset=40
            local.get 0
            i32.load offset=8
            local.get 3
            call $j1306
            local.set 2
            local.get 1
            i32.load offset=16
            i32.load offset=96
            i32.const 5
            call $j874
            i32.load offset=64
            local.set 4
            local.get 2
            local.get 4
            i32.load offset=12
            i32.add
            local.set 2
            local.get 0
            local.get 7
            local.get 2
            local.get 2
            i32.const 8
            i32.sub
            local.get 4
            i32.load offset=8
            i32.load offset=20
            i32.const -1
            i32.gt_s
            select
            f32.load
            f32.add
            f32.store offset=28
          end
          local.get 3
          i32.const 1
          i32.add
          local.set 3
          br 1 (;@2;)
        end
      end
      local.get 0
      f32.load offset=24
      local.get 0
      f32.load offset=28
      f32.div
      local.set 8
      i32.const 0
      local.set 3
      loop  ;; label = @2
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        i32.load
        local.get 1
        i32.load offset=16
        i32.load offset=96
        i32.load offset=32
        local.get 0
        i32.load offset=8
        call $j917
        local.get 3
        i32.le_s
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=32
        local.get 3
        i32.ne
        if  ;; label = @3
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          i32.load
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          local.get 0
          i32.load offset=8
          local.get 3
          call $j1306
          local.set 4
          local.get 4
          local.set 2
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.const 5
          call $j874
          i32.load offset=64
          local.set 5
          local.get 2
          local.get 5
          i32.load offset=32
          i32.add
          local.set 2
          local.get 2
          local.get 2
          i32.const 8
          i32.sub
          local.get 5
          i32.load offset=28
          i32.load offset=20
          i32.const -1
          i32.gt_s
          select
          f32.load
          local.set 9
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          i32.load
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          local.get 0
          i32.load offset=8
          local.get 3
          call $j1306
          local.set 2
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.const 5
          call $j874
          i32.load offset=64
          local.set 5
          local.get 2
          local.get 5
          i32.load offset=12
          i32.add
          local.set 2
          local.get 2
          local.get 2
          i32.const 8
          i32.sub
          local.get 5
          i32.load offset=8
          i32.load offset=20
          i32.const -1
          i32.gt_s
          select
          f32.load
          local.set 7
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.const 5
          call $j874
          i32.load offset=64
          local.set 2
          local.get 4
          local.get 2
          i32.load offset=32
          i32.add
          local.set 4
          local.get 4
          local.get 4
          i32.const 8
          i32.sub
          local.get 2
          i32.load offset=28
          i32.load offset=20
          i32.const -1
          i32.gt_s
          select
          local.get 9
          local.get 8
          local.get 7
          f32.mul
          f32.add
          f32.store
          local.get 2
          i32.load offset=24
          drop
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          i32.load
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          local.get 0
          i32.load offset=8
          local.get 3
          call $j1306
          local.set 4
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          i32.load
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.load offset=40
          local.get 0
          i32.load offset=8
          local.get 3
          call $j1306
          local.set 2
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.const 5
          call $j874
          i32.load offset=64
          local.set 5
          local.get 2
          local.get 5
          i32.load offset=72
          i32.add
          local.set 2
          local.get 2
          local.get 2
          i32.const 8
          i32.sub
          local.get 5
          i32.load offset=68
          i32.load offset=20
          i32.const -1
          i32.gt_s
          select
          i32.load
          local.set 5
          local.get 0
          i32.load offset=20
          local.set 6
          local.get 1
          i32.load offset=16
          i32.load offset=96
          i32.const 5
          call $j874
          i32.load offset=64
          local.set 2
          local.get 4
          local.get 2
          i32.load offset=52
          i32.add
          local.set 4
          local.get 4
          local.get 4
          i32.const 8
          i32.sub
          local.get 2
          i32.load offset=48
          i32.load offset=20
          i32.const -1
          i32.gt_s
          select
          local.get 5
          f32.convert_i32_s
          local.get 6
          f32.convert_i32_s
          f32.div
          f32.store
          local.get 2
          i32.load offset=44
          drop
        end
        local.get 3
        i32.const 1
        i32.add
        local.set 3
        br 0 (;@2;)
      end
      unreachable
    end)

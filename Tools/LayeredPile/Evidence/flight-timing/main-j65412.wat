  (func $j65412 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 f32 f32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 0
    local.set 1
    i32.const 4507798
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430720
      call $j870 ;; 
      i32.const 3470264
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3434688
      call $j870 ;; 
      i32.const 3435480
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3502396
      call $j870 ;; 
      i32.const 3502500
      call $j870 ;; 
      i32.const 3440340
      call $j870 ;; 
      i32.const 3568436
      call $j870 ;; 
      i32.const 4507798
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=20
    local.set 2
    block  ;; label = @1
      block  ;; label = @2
        block  ;; label = @3
          block  ;; label = @4
            block  ;; label = @5
              block  ;; label = @6
                local.get 0
                i32.load offset=8
                br_table 0 (;@6;) 1 (;@5;) 2 (;@4;) 3 (;@3;) 4 (;@2;) 5 (;@1;)
              end
              local.get 0
              i32.const -1
              i32.store offset=8
              local.get 0
              f32.load offset=16
              local.set 6
              local.get 2
              i32.load offset=56
              f32.load offset=228
              local.set 7
              i32.const 3434688
              i32.load
              call $j871 ;; 
              f32.const 0x0p+0 (;=0;)
              local.get 6
              local.get 7
              f32.add
              i32.const 0
              call $j1802 ;; 
              local.set 6
              i32.const 3440340
              i32.load
              call $j873 ;; 
              local.set 1
              local.get 1
              local.set 2
              local.get 2
              i32.const -1082130432
              i32.store offset=12
              local.get 2
              local.get 6
              f32.store offset=8
              local.get 0
              local.get 2
              i32.store offset=12
              i32.const 1
              local.set 1
              local.get 0
              i32.const 1
              i32.store offset=8
              br 4 (;@1;)
            end
            local.get 0
            i32.const -1
            i32.store offset=8
            i32.const 3470264
            i32.load
            drop
            local.get 0
            i32.load offset=24
            i32.load offset=12
            i32.const 2
            i32.le_s
            if  ;; label = @5
              i32.const 3430720
              i32.load
              call $j871 ;; 
              i32.const 3568436
              i32.load
              i32.const 0
              call $j1062 ;; 
              br 4 (;@1;)
            end
            local.get 3
            local.get 0
            i32.load offset=24
            i32.const 1
            i32.const 3470268
            i32.load
            call $j895 ;; 
            i32.const 0
            call $j944 ;; 
            i32.const 0
            call $j1028 ;; 
            local.get 2
            i32.load offset=56
            local.set 1
            local.get 1
            f32.load offset=252
            local.set 6
            local.get 3
            f32.load
            local.get 1
            f32.load offset=244
            f32.add
            local.set 7
            local.get 3
            f32.load offset=4
            local.get 1
            f32.load offset=248
            f32.add
            local.set 8
            local.get 3
            f32.load offset=8
            local.get 6
            f32.add
            local.set 6
            i32.const 0
            local.set 1
            loop  ;; label = @5
              i32.const 3470264
              i32.load
              drop
              local.get 1
              local.get 0
              i32.load offset=24
              i32.load offset=12
              i32.lt_s
              if  ;; label = @6
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                i32.const 0
                i32.const 0
                call $j1360 ;; 
                drop
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.get 8
                local.get 2
                i32.load offset=56
                f32.load offset=236
                i32.const 0
                i32.const 0
                call $j9406 ;; 
                i32.const 1
                i32.const 3502500
                i32.load
                call $j1281 ;; 
                local.get 2
                i32.load offset=56
                i32.load offset=256
                i32.const 3502396
                i32.load
                call $j1183 ;; 
                drop
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.get 6
                local.get 2
                i32.load offset=56
                f32.load offset=236
                i32.const 0
                i32.const 0
                call $j12483 ;; 
                i32.const 1
                i32.const 3502500
                i32.load
                call $j1281 ;; 
                local.get 2
                i32.load offset=56
                i32.load offset=256
                i32.const 3502396
                i32.load
                call $j1183 ;; 
                drop
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.get 7
                local.get 2
                i32.load offset=56
                f32.load offset=236
                i32.const 0
                i32.const 0
                call $j5721 ;; 
                i32.const 1
                i32.const 3502500
                i32.load
                call $j1281 ;; 
                local.get 2
                i32.load offset=56
                i32.load offset=260
                i32.const 3502396
                i32.load
                call $j1183 ;; 
                drop
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.get 2
                i32.load offset=56
                f32.load offset=264
                local.get 0
                i32.load offset=24
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                f32.load offset=64
                f32.mul
                local.get 2
                i32.load offset=56
                f32.load offset=236
                i32.const 0
                call $j1727 ;; 
                i32.const 1
                i32.const 3502500
                i32.load
                call $j1281 ;; 
                local.get 2
                i32.load offset=56
                i32.load offset=272
                i32.const 3502396
                i32.load
                call $j1183 ;; 
                drop
                local.get 1
                i32.const 1
                i32.add
                local.set 1
                br 1 (;@5;)
              end
            end
            local.get 2
            i32.load offset=56
            f32.load offset=236
            local.set 6
            i32.const 3440340
            i32.load
            call $j873 ;; 
            local.set 1
            local.get 1
            local.set 2
            local.get 2
            i32.const -1082130432
            i32.store offset=12
            local.get 2
            local.get 6
            f32.store offset=8
            local.get 0
            local.get 2
            i32.store offset=12
            local.get 0
            i32.const 2
            i32.store offset=8
            i32.const 1
            local.set 1
            br 3 (;@1;)
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          loop  ;; label = @4
            block  ;; label = @5
              i32.const 3470268
              i32.load
              local.set 4
              local.get 0
              i32.load offset=24
              local.set 5
              i32.const 3470264
              i32.load
              drop
              local.get 0
              i32.load offset=24
              i32.load offset=12
              local.get 1
              i32.le_s
              br_if 0 (;@5;)
              local.get 5
              local.get 1
              local.get 4
              call $j895 ;; 
              i32.const 0
              call $j944 ;; 
              i32.const 0
              i32.const 0
              call $j1360 ;; 
              drop
              local.get 1
              i32.const 1
              i32.add
              local.set 1
              br 1 (;@4;)
            end
          end
          local.get 5
          i32.const 0
          local.get 4
          call $j895 ;; 
          i32.const 0
          call $j925 ;; 
          local.set 4
          i32.const 3435480
          i32.load
          call $j871 ;; 
          i32.const 1
          local.set 1
          local.get 4
          i32.const 0
          i32.const 1
          i32.const 0
          call $j2538 ;; ObjectPoolMa.RemoveObj
          local.get 0
          i32.load offset=24
          i32.const 2
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j925 ;; 
          i32.const 0
          i32.const 1
          i32.const 0
          call $j2538 ;; ObjectPoolMa.RemoveObj
          local.get 0
          i32.load offset=24
          i32.const 1
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j944 ;; 
          local.get 2
          i32.load offset=56
          f32.load offset=268
          local.get 0
          i32.load offset=24
          i32.const 1
          i32.const 3470268
          i32.load
          call $j895 ;; 
          f32.load offset=64
          f32.mul
          local.get 2
          i32.load offset=56
          f32.load offset=240
          i32.const 0
          call $j1727 ;; 
          i32.const 1
          i32.const 3502500
          i32.load
          call $j1281 ;; 
          local.get 2
          i32.load offset=56
          i32.load offset=276
          i32.const 3502396
          i32.load
          call $j1183 ;; 
          drop
          local.get 2
          i32.load offset=56
          f32.load offset=240
          local.set 6
          i32.const 3440340
          i32.load
          call $j873 ;; 
          local.set 2
          local.get 2
          i32.const -1082130432
          i32.store offset=12
          local.get 2
          local.get 6
          f32.store offset=8
          local.get 0
          local.get 2
          i32.store offset=12
          local.get 0
          i32.const 3
          i32.store offset=8
          br 2 (;@1;)
        end
        local.get 0
        i32.const -1
        i32.store offset=8
        i32.const 3440340
        i32.load
        call $j873 ;; 
        local.set 1
        local.get 1
        local.set 2
        local.get 2
        i32.const -1082130432
        i32.store offset=12
        local.get 2
        f32.const 0x1.99999ap-5 (;=0.05;)
        f32.store offset=8
        local.get 0
        local.get 2
        i32.store offset=12
        local.get 0
        i32.const 4
        i32.store offset=8
        i32.const 1
        local.set 1
        br 1 (;@1;)
      end
      local.get 0
      i32.const -1
      i32.store offset=8
      local.get 0
      i32.load offset=24
      i32.const 1
      i32.const 3470268
      i32.load
      call $j895 ;; 
      local.set 2
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 2
      i32.const 0
      call $j982 ;; 
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=24
      i32.const 1
      i32.const 3470268
      i32.load
      call $j895 ;; 
      i32.const 0
      call $j944 ;; 
      i32.const 0
      i32.const 0
      call $j1360 ;; 
      drop
      local.get 0
      i32.load offset=24
      i32.const 1
      i32.const 3470268
      i32.load
      call $j895 ;; 
      i32.const 0
      call $j925 ;; 
      local.set 0
      i32.const 3435480
      i32.load
      call $j871 ;; 
      local.get 0
      i32.const 0
      i32.const 1
      i32.const 0
      call $j2538 ;; ObjectPoolMa.RemoveObj
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0
    local.get 1
    return)

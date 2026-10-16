  (func $j65313 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 i32 f32 f32)
    global.get 0
    i32.const 80
    i32.sub
    local.set 1
    local.get 1
    global.set 0
    i32.const 4507774
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3435480
      call $j870 ;; 
      i32.const 3438764
      call $j870 ;; 
      i32.const 3502396
      call $j870 ;; 
      i32.const 3440336
      call $j870 ;; 
      i32.const 4507774
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
                block  ;; label = @7
                  local.get 0
                  i32.load offset=8
                  br_table 0 (;@7;) 1 (;@6;) 2 (;@5;) 3 (;@4;) 5 (;@2;) 6 (;@1;)
                end
                local.get 0
                i32.const -1
                i32.store offset=8
                i32.const 3434312
                i32.load
                call $j871 ;; 
                i32.const 0
                call $j5871 ;; LevelManager.CanUseNewAnim
                br_if 3 (;@3;)
                local.get 0
                i32.load offset=16
                i32.const 0
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.set 2
                i32.const 1
                local.set 5
                local.get 1
                i32.const -64
                i32.sub
                local.get 0
                i32.load offset=16
                i32.const 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                i32.const 0
                call $j1028 ;; 
                local.get 1
                i32.load offset=72
                local.set 3
                local.get 1
                local.get 3
                i32.store offset=56
                local.get 1
                local.get 3
                i32.store offset=24
                local.get 1
                i32.load offset=64
                local.set 3
                local.get 1
                i32.load offset=68
                local.set 6
                local.get 3
                local.set 4
                local.get 1
                local.get 4
                i32.store offset=48
                local.get 1
                local.get 6
                i32.store offset=52
                local.get 1
                local.get 4
                i32.store offset=16
                local.get 1
                local.get 6
                i32.store offset=20
                local.get 2
                local.get 1
                i32.const 16
                i32.add
                f32.const 0x1.99999ap-3 (;=0.2;)
                i32.const 0
                i32.const 0
                call $j3031 ;; 
                drop
                local.get 0
                i32.load offset=16
                i32.const 2
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                local.set 2
                local.get 1
                i32.const -64
                i32.sub
                local.get 0
                i32.load offset=16
                i32.const 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.const 0
                call $j944 ;; 
                i32.const 0
                call $j1028 ;; 
                local.get 1
                i32.load offset=72
                local.set 3
                local.get 1
                local.get 3
                i32.store offset=40
                local.get 1
                local.get 3
                i32.store offset=8
                local.get 1
                i32.load offset=64
                local.set 6
                local.get 1
                i32.load offset=68
                local.set 3
                local.get 6
                local.set 4
                local.get 1
                local.get 4
                i32.store offset=32
                local.get 1
                local.get 3
                i32.store offset=36
                local.get 1
                local.get 4
                i32.store
                local.get 1
                local.get 3
                i32.store offset=4
                local.get 2
                local.get 1
                f32.const 0x1.99999ap-3 (;=0.2;)
                i32.const 0
                i32.const 0
                call $j3031 ;; 
                drop
                i32.const 3438764
                i32.load
                call $j871 ;; 
                local.get 0
                i32.const 3438764
                i32.load
                i32.load offset=92
                i32.load offset=16
                i32.store offset=12
                local.get 0
                i32.const 1
                i32.store offset=8
                br 5 (;@1;)
              end
              local.get 0
              i32.const -1
              i32.store offset=8
              local.get 0
              i32.load offset=16
              i32.const 0
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
              i32.load offset=16
              i32.const 2
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
              i32.load offset=16
              i32.const 0
              i32.const 3470268
              i32.load
              call $j895 ;; 
              i32.const 0
              call $j925 ;; 
              local.set 2
              i32.const 3435480
              i32.load
              call $j871 ;; 
              i32.const 1
              local.set 5
              local.get 2
              i32.const 0
              i32.const 1
              i32.const 0
              call $j2538 ;; ObjectPoolMa.RemoveObj
              local.get 0
              i32.load offset=16
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
              i32.load offset=16
              i32.const 1
              i32.const 3470268
              i32.load
              call $j895 ;; 
              i32.const 0
              call $j944 ;; 
              f32.const 0x0p+0 (;=0;)
              f32.const 0x1.99999ap-3 (;=0.2;)
              i32.const 0
              call $j1727 ;; 
              i32.const 26
              i32.const 3502396
              i32.load
              call $j1183 ;; 
              drop
              i32.const 3438764
              i32.load
              call $j871 ;; 
              local.get 0
              i32.const 3438764
              i32.load
              i32.load offset=92
              i32.load offset=16
              i32.store offset=12
              local.get 0
              i32.const 2
              i32.store offset=8
              br 4 (;@1;)
            end
            local.get 0
            i32.const -1
            i32.store offset=8
            i32.const 1
            local.set 5
            local.get 2
            local.get 0
            i32.load offset=16
            i32.const 1
            i32.const 3470268
            i32.load
            call $j895 ;; 
            local.get 0
            i32.load offset=24
            local.get 0
            i32.load offset=28
            i32.const 0
            call $j19872 ;; LevelManager.EndCombineObjAnim
            i32.const 3440336
            i32.load
            call $j873 ;; 
            local.set 2
            local.get 2
            f32.const 0x1.99999ap-5 (;=0.05;)
            f32.store offset=8
            local.get 0
            local.get 2
            i32.store offset=12
            local.get 0
            i32.const 3
            i32.store offset=8
            br 3 (;@1;)
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          local.get 0
          i32.load offset=16
          i32.const 1
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j925 ;; 
          local.set 3
          i32.const 3435480
          i32.load
          call $j871 ;; 
          local.get 3
          i32.const 0
          i32.const 1
          i32.const 0
          call $j2538 ;; ObjectPoolMa.RemoveObj
          local.get 2
          local.get 0
          i32.load offset=28
          i32.const 0
          i32.const 0
          call $j9879 ;; LevelManager.RefreshContentDisplay
          br 2 (;@1;)
        end
        local.get 2
        i32.load offset=628
        i32.load offset=56
        local.set 2
        local.get 2
        f32.load offset=240
        local.set 7
        local.get 2
        f32.load offset=228
        local.get 2
        f32.load offset=236
        f32.add
        local.get 7
        f32.add
        local.set 7
        local.get 0
        f32.load offset=32
        local.set 8
        i32.const 3440336
        i32.load
        call $j873 ;; 
        local.set 2
        local.get 2
        local.get 7
        local.get 8
        f32.add
        f32.store offset=8
        local.get 0
        local.get 2
        i32.store offset=12
        local.get 0
        i32.const 4
        i32.store offset=8
        i32.const 1
        local.set 5
        br 1 (;@1;)
      end
      local.get 0
      i32.const -1
      i32.store offset=8
      local.get 2
      local.get 0
      i32.load offset=16
      i32.const 1
      i32.const 3470268
      i32.load
      call $j895 ;; 
      local.get 0
      i32.load offset=24
      local.get 0
      i32.load offset=28
      i32.const 0
      call $j19872 ;; LevelManager.EndCombineObjAnim
    end
    local.get 1
    i32.const 80
    i32.add
    global.set 0
    local.get 5
    return)

  (func $j65369 (type 2) (param i32 i32) (result i32)
    (local i32 i32 i32 i32 f32)
    i32.const 4507773
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      call $j870 ;; 
      i32.const 3434152
      call $j870 ;; 
      i32.const 3434312
      call $j870 ;; 
      i32.const 3470236
      call $j870 ;; 
      i32.const 3470260
      call $j870 ;; 
      i32.const 3470256
      call $j870 ;; 
      i32.const 3470232
      call $j870 ;; 
      i32.const 3470264
      call $j870 ;; 
      i32.const 3472460
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3424832
      call $j870 ;; 
      i32.const 3435224
      call $j870 ;; 
      i32.const 3440336
      call $j870 ;; 
      i32.const 3565452
      call $j870 ;; 
      i32.const 4507773
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=20
    local.set 2
    local.get 2
    local.set 4
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
            i32.const 3434312
            i32.load
            call $j871 ;; 
            local.get 0
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load offset=28
            i32.store offset=28
            local.get 0
            local.get 2
            i32.const 188
            i32.const 204
            local.get 0
            i32.load offset=16
            i32.const 1
            i32.eq
            select
            i32.add
            i32.load
            i32.store offset=32
            local.get 2
            i32.const 180
            i32.const 196
            local.get 0
            i32.load offset=16
            i32.const 1
            i32.eq
            select
            i32.add
            i32.load
            local.set 5
            local.get 0
            i32.const 1
            i32.store offset=36
            i32.const 1
            local.set 1
            loop  ;; label = @5
              i32.const 3470264
              i32.load
              drop
              local.get 1
              local.get 0
              i32.load offset=32
              i32.load offset=12
              i32.ge_s
              br_if 3 (;@2;)
              block  ;; label = @6
                local.get 0
                i32.load offset=32
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.load offset=12
                local.get 0
                i32.load offset=32
                local.get 1
                i32.const 1
                i32.sub
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.load offset=12
                i32.const 0
                call $j899 ;; 
                i32.eqz
                if  ;; label = @7
                  local.get 0
                  i32.const 1
                  i32.store offset=36
                  br 1 (;@6;)
                end
                local.get 0
                i32.load offset=36
                i32.const 1
                i32.add
                local.set 3
                local.get 0
                local.get 3
                i32.store offset=36
                local.get 3
                i32.const 3
                i32.ne
                br_if 0 (;@6;)
                local.get 2
                local.get 0
                i32.load offset=32
                local.get 1
                i32.const 3470268
                i32.load
                call $j895 ;; 
                i32.load offset=12
                i32.const 0
                call $j38505 ;; LevelManager.GetTriple
                i32.const 3424832
                i32.load
                call $j873 ;; 
                local.set 3
                local.get 3
                i32.const 3470232
                i32.load
                call $j918 ;; 
                local.get 0
                local.get 3
                i32.store offset=40
                i32.const 0
                local.set 3
                loop  ;; label = @7
                  local.get 3
                  i32.const 3
                  i32.lt_s
                  if  ;; label = @8
                    local.get 5
                    local.get 0
                    i32.load offset=32
                    local.get 1
                    local.get 3
                    i32.sub
                    i32.const 3470268
                    i32.load
                    call $j895 ;; 
                    i32.const 3470256
                    i32.load
                    call $j1342 ;; 
                    drop
                    local.get 0
                    i32.load offset=40
                    local.get 0
                    i32.load offset=32
                    local.get 1
                    local.get 3
                    i32.sub
                    i32.const 3470268
                    i32.load
                    call $j895 ;; 
                    i32.const 3470236
                    i32.load
                    call $j903 ;; 
                    local.get 0
                    i32.load offset=32
                    local.get 1
                    local.get 3
                    i32.sub
                    i32.const 3470260
                    i32.load
                    call $j1234 ;; 
                    local.get 3
                    i32.const 1
                    i32.add
                    local.set 3
                    br 1 (;@7;)
                  end
                end
                i32.const 3434312
                i32.load
                call $j871 ;; 
                i32.const 0
                call $j5871 ;; LevelManager.CanUseNewAnim
                if  ;; label = @7
                  local.get 2
                  i32.load offset=628
                  local.get 0
                  i32.load offset=40
                  local.get 0
                  f32.load offset=24
                  local.get 0
                  i32.load offset=16
                  local.get 1
                  call $j38476 ;; LevelObjAnimManager.CombieObj
                  local.get 2
                  local.get 0
                  i32.load offset=40
                  local.get 0
                  i32.load offset=28
                  local.get 0
                  i32.load offset=16
                  local.get 0
                  f32.load offset=24
                  i32.const 0
                  call $j19873 ;; LevelManager.DestroyAnim
                  local.set 1
                  br 4 (;@3;)
                end
                local.get 0
                f32.load offset=24
                local.set 6
                i32.const 3440336
                i32.load
                call $j873 ;; 
                local.set 1
                local.get 1
                local.get 6
                f32.store offset=8
                local.get 0
                local.get 1
                i32.store offset=12
                local.get 0
                i32.const 1
                i32.store offset=8
                i32.const 1
                return
              end
              local.get 1
              i32.const 1
              i32.add
              local.set 1
              br 0 (;@5;)
            end
            unreachable
          end
          local.get 0
          i32.const -1
          i32.store offset=8
          local.get 2
          local.get 0
          i32.load offset=40
          local.get 0
          i32.load offset=28
          local.get 0
          i32.load offset=16
          f32.const 0x0p+0 (;=0;)
          i32.const 0
          call $j19873 ;; LevelManager.DestroyAnim
          local.set 1
        end
        local.get 4
        local.get 1
        i32.const 0
        call $j1135 ;; 
        drop
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        br_if 0 (;@2;)
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=28
        i32.const 2
        i32.ne
        br_if 0 (;@2;)
        i32.const 3435224
        i32.load
        call $j871 ;; 
        i32.const 4514392
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3435224
          call $j870 ;; 
          i32.const 4514392
          i32.const 1
          i32.store8
        end
        i32.const 3435224
        i32.load
        call $j871 ;; 
        i32.const 3435224
        i32.load
        i32.load offset=92
        i32.load offset=32
        i32.load offset=36
        i32.load offset=8
        i32.const 0
        i32.gt_s
        br_if 0 (;@2;)
        local.get 2
        i32.load offset=244
        i32.const 1
        i32.add
        local.set 1
        local.get 2
        local.get 1
        i32.store offset=244
        block  ;; label = @3
          local.get 1
          i32.const 10
          i32.le_s
          if  ;; label = @4
            block  ;; label = @5
              local.get 1
              i32.const 1
              i32.sub
              i32.const 2
              i32.lt_u
              br_if 0 (;@5;)
              local.get 1
              i32.const 5
              i32.eq
              br_if 0 (;@5;)
              local.get 1
              i32.const 10
              i32.ne
              br_if 3 (;@2;)
            end
            local.get 2
            i32.load offset=244
            local.set 1
            br 1 (;@3;)
          end
          local.get 1
          i32.const 50
          i32.le_s
          if  ;; label = @4
            local.get 1
            i32.const 20
            i32.eq
            br_if 1 (;@3;)
            local.get 1
            i32.const 50
            i32.eq
            br_if 1 (;@3;)
            br 2 (;@2;)
          end
          local.get 1
          i32.const 200
          i32.eq
          br_if 0 (;@3;)
          local.get 1
          i32.const 100
          i32.ne
          br_if 1 (;@2;)
        end
        local.get 1
        i32.const 0
        call $j12169 ;; ThinkingManager.LosePlayerMatchCount
      end
      i32.const 3470264
      i32.load
      drop
      i32.const 3472460
      i32.load
      drop
      block  ;; label = @2
        local.get 0
        i32.load offset=32
        i32.load offset=12
        local.get 2
        i32.load offset=184
        i32.load offset=12
        i32.const 1
        i32.sub
        i32.ne
        br_if 0 (;@2;)
        i32.const 3434312
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 0
          call $j1923 ;; LevelManager.HasTwoPlayer
          i32.eqz
          if  ;; label = @4
            i32.const 3434312
            i32.load
            call $j871 ;; 
            i32.const 600
            local.set 1
            i32.const 3434312
            i32.load
            i32.load offset=92
            i32.load offset=28
            i32.const 1
            i32.ne
            br_if 1 (;@3;)
            br 2 (;@2;)
          end
          i32.const 602
          i32.const 603
          local.get 0
          i32.load offset=16
          i32.const 1
          i32.eq
          select
          local.set 1
        end
        i32.const 3434152
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 3565452
        i32.load
        f32.const 0x1p+0 (;=1;)
        f32.const 0x0p+0 (;=0;)
        i32.const 0
        call $j1857 ;; KUIManager.OpenATip
      end
      i32.const 3470264
      i32.load
      drop
      i32.const 3472460
      i32.load
      drop
      block  ;; label = @2
        local.get 0
        i32.load offset=32
        i32.load offset=12
        local.get 2
        i32.load offset=184
        i32.load offset=12
        i32.lt_s
        br_if 0 (;@2;)
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 0
        call $j1923 ;; LevelManager.HasTwoPlayer
        i32.eqz
        if  ;; label = @3
          local.get 2
          i32.const 0
          i32.const 0
          call $j13225 ;; LevelManager.PrepareLose
          br 1 (;@2;)
        end
        local.get 2
        i32.const 2
        i32.const 2
        i32.const 1
        local.get 0
        i32.load offset=16
        i32.const 1
        i32.eq
        select
        i32.const 0
        call $j13233 ;; LevelManager.BattleLose
      end
      i32.const 3428500
      i32.load
      call $j871 ;; 
      i32.const 0
      call $j4845 ;; ABPackManager.get_NewPlayerGuideABType
      local.set 3
      local.get 0
      i32.load offset=36
      local.set 1
      local.get 3
      i32.const 1
      i32.eq
      if  ;; label = @2
        local.get 2
        i32.const 1
        i32.const 3
        i32.const 2
        local.get 1
        i32.const 3
        i32.eq
        select
        i32.const 0
        call $j9880 ;; LevelManager.NewGuide
        local.get 2
        local.get 0
        i32.load offset=36
        i32.const 3
        i32.eq
        i32.const 0
        call $j38494 ;; LevelManager.NewGuideClickObj
        i32.const 0
        return
      end
      local.get 2
      local.get 1
      i32.const 3
      i32.eq
      i32.const 0
      call $j38483 ;; LevelManager.TeachClickObj
    end
    i32.const 0
    return)

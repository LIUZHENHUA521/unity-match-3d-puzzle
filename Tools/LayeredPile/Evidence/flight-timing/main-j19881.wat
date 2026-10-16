  (func $j19881 (type 7) (param i32 i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    i32.const 4509375
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428500
      call $j870 ;; 
      i32.const 3428556
      call $j870 ;; 
      i32.const 3432052
      call $j870 ;; 
      i32.const 3432084
      call $j870 ;; 
      i32.const 3434152
      call $j870 ;; 
      i32.const 3434312
      call $j870 ;; 
      i32.const 3470264
      call $j870 ;; 
      i32.const 3437592
      call $j870 ;; 
      i32.const 3438928
      call $j870 ;; 
      i32.const 3439248
      call $j870 ;; 
      i32.const 3439348
      call $j870 ;; 
      i32.const 3568344
      call $j870 ;; 
      i32.const 3539240
      call $j870 ;; 
      i32.const 4509375
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      i32.const 3432052
      i32.load
      i32.load offset=92
      i32.load8_u offset=4
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871 ;; 
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load offset=28
      local.get 1
      i32.ne
      br_if 0 (;@1;)
      block  ;; label = @2
        local.get 0
        i32.load8_u offset=48
        if  ;; label = @3
          i32.const 0
          local.set 1
          i32.const 0
          local.set 3
          loop  ;; label = @4
            local.get 0
            i32.load offset=84
            local.set 5
            local.get 1
            local.get 5
            i32.load offset=12
            i32.ge_s
            br_if 2 (;@2;)
            local.get 5
            local.get 1
            i32.const 3
            i32.shl
            i32.add
            i32.const 16
            i32.add
            i32.load offset=4
            local.get 3
            i32.add
            local.set 3
            local.get 1
            i32.const 1
            i32.add
            local.set 1
            br 0 (;@4;)
          end
          unreachable
        end
        i32.const 3470264
        i32.load
        drop
        local.get 0
        i32.load offset=192
        i32.load offset=12
        local.set 3
      end
      i32.const 3434312
      i32.load
      call $j871 ;; 
      local.get 1
      call $j1923 ;; LevelManager.HasTwoPlayer
      local.set 1
      i32.const 3470264
      i32.load
      drop
      local.get 0
      f32.const 0x1p+0 (;=1;)
      local.get 0
      i32.load offset=180
      i32.load offset=12
      f32.convert_i32_s
      local.get 0
      i32.load offset=308
      f32.convert_i32_s
      f32.div
      f32.sub
      i32.const 1
      local.get 1
      call $j8064 ;; LevelManager.SetPrograss
      block  ;; label = @2
        local.get 1
        i32.eqz
        br_if 0 (;@2;)
        i32.const 3470264
        i32.load
        drop
        local.get 0
        f32.const 0x1p+0 (;=1;)
        local.get 0
        i32.load offset=196
        i32.load offset=12
        f32.convert_i32_s
        local.get 0
        i32.load offset=372
        f32.convert_i32_s
        f32.div
        f32.sub
        i32.const 2
        local.get 1
        call $j8064 ;; LevelManager.SetPrograss
        i32.const 3428556
        i32.load
        call $j871 ;; 
        i32.const 3428556
        i32.load
        i32.load offset=92
        i32.load offset=44
        local.set 1
        local.get 1
        i32.eqz
        br_if 0 (;@2;)
        local.get 1
        local.get 0
        f32.load offset=312
        local.get 0
        f32.load offset=376
        call $j44854 ;; 
      end
      block  ;; label = @2
        local.get 3
        br_if 0 (;@2;)
        i32.const 3434312
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=12
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871 ;; 
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=28
          i32.const 1
          i32.eq
          if  ;; label = @4
            local.get 0
            i32.const 1
            i32.const 1
            i32.const 100
            i32.const 0
            local.get 1
            call $j13223 ;; LevelManager.SetMainLevelRecord
            i32.const 3
            i32.const 0
            call $j1804 ;; GameManager.SetGameState
            local.get 0
            i32.const 2
            i32.const 0
            i32.const 0
            local.get 1
            call $j2544 ;; LevelManager.StartGame
            i32.const 3428500
            i32.load
            call $j871 ;; 
            block  ;; label = @5
              i32.const 0
              call $j4845 ;; ABPackManager.get_NewPlayerGuideABType
              i32.const 1
              i32.eq
              if  ;; label = @6
                local.get 0
                i32.load8_u offset=465
                br_if 1 (;@5;)
              end
              local.get 0
              i32.load offset=392
              local.set 1
              i32.const 3438928
              i32.load
              call $j871 ;; 
              local.get 1
              local.get 1
              i32.const 0
              i32.const 0
              call $j1179 ;; TranslateMa.GetTransString
              local.set 1
              i32.const 3434152
              i32.load
              call $j871 ;; 
              i32.const 600
              local.get 1
              f32.const 0x1p+0 (;=1;)
              f32.const 0x0p+0 (;=0;)
              i32.const 0
              call $j1857 ;; KUIManager.OpenATip
            end
            i32.const 3428500
            i32.load
            call $j871 ;; 
            i32.const 0
            call $j4845 ;; ABPackManager.get_NewPlayerGuideABType
            i32.const 1
            i32.eq
            if  ;; label = @5
              local.get 0
              i32.load8_u offset=465
              i32.eqz
              br_if 2 (;@3;)
              local.get 0
              i32.const 3539240
              i32.load
              i32.const 0
              call $j1446 ;; 
              local.get 0
              i32.const 3539240
              i32.load
              i32.const 0
              call $j1535 ;; 
              drop
              i32.const 3439248
              i32.load
              i32.load offset=92
              i32.load
              i32.const 1
              i32.const 0
              call $j17000 ;; UIGamePage.ShowGamePageItem
              br 2 (;@3;)
            end
            i32.const 0
            call $j12256 ;; TeachManager.CloseHandGuide
            local.get 0
            local.get 1
            call $j38514 ;; LevelManager.FirstTeachEnd
            br 1 (;@3;)
          end
          local.get 0
          i32.const 1
          i32.const 2
          i32.const 100
          i32.const 0
          local.get 1
          call $j13223 ;; LevelManager.SetMainLevelRecord
          i32.const 0
          call $j5928 ;; GameManager.GameWin
        end
        i32.const 3434312
        i32.load
        call $j871 ;; 
        block  ;; label = @3
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=12
          i32.const 3
          i32.ne
          br_if 0 (;@3;)
          i32.const 3434312
          i32.load
          call $j871 ;; 
          i32.const 3434312
          i32.load
          i32.load offset=92
          i32.load offset=28
          i32.const 2
          i32.ne
          br_if 0 (;@3;)
          i32.const 3439348
          i32.load
          i32.load offset=92
          i32.load
          i32.const 100
          i32.const 1
          i32.const 0
          call $j5617 ;; UIOnlineGamePage.SetMyProgress
          i32.const 3439348
          i32.load
          i32.load offset=92
          i32.load
          i32.const 101
          i32.const 0
          call $j11975 ;; UIOnlineGamePage.EndOnlineGame
          i32.const 0
          call $j5928 ;; GameManager.GameWin
          i32.const 2
          i32.const 3568344
          i32.load
          i32.const 0
          call $j1464 ;; GameManager.LogInColor
        end
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        i32.const 4
        i32.eq
        if  ;; label = @3
          i32.const 0
          call $j5928 ;; GameManager.GameWin
          i32.const 2
          i32.const 3568344
          i32.load
          i32.const 0
          call $j1464 ;; GameManager.LogInColor
          local.get 0
          i32.const 1
          i32.const 2
          i32.const 100
          i32.const 0
          local.get 1
          call $j19861 ;; LevelManager.SetNewYearLevelRecord
          br 1 (;@2;)
        end
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        i32.const 5
        i32.eq
        if  ;; label = @3
          i32.const 0
          call $j5928 ;; GameManager.GameWin
          i32.const 2
          i32.const 3568344
          i32.load
          i32.const 0
          call $j1464 ;; GameManager.LogInColor
          i32.const 3434312
          i32.load
          call $j871 ;; 
          i32.const 3432084
          i32.load
          local.set 5
          i32.const 3434312
          i32.load
          i32.load offset=92
          local.set 3
          local.get 4
          i32.const -1
          i32.store offset=4
          local.get 4
          local.get 5
          i32.store
          local.get 4
          local.get 3
          i32.load offset=12
          i32.store offset=8
          local.get 0
          local.get 4
          i32.const 0
          call $j956 ;; 
          i32.const 1
          i32.const 2
          i32.const 100
          i32.const 0
          local.get 1
          call $j9877 ;; LevelManager.SetSpLevelRecord
          br 1 (;@2;)
        end
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        i32.const 6
        i32.ne
        br_if 0 (;@2;)
        i32.const 0
        call $j5928 ;; GameManager.GameWin
        i32.const 2
        i32.const 3568344
        i32.load
        i32.const 0
        call $j1464 ;; GameManager.LogInColor
        i32.const 3437592
        i32.load
        call $j871 ;; 
        local.get 0
        i32.const 3437592
        i32.load
        i32.load offset=92
        i32.load offset=4
        i32.const 1
        i32.const 2
        i32.const 100
        i32.const 0
        local.get 1
        call $j9877 ;; LevelManager.SetSpLevelRecord
      end
      i32.const 3434312
      i32.load
      call $j871 ;; 
      local.get 1
      call $j1923 ;; LevelManager.HasTwoPlayer
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3470264
      i32.load
      drop
      local.get 0
      i32.load offset=192
      i32.load offset=12
      i32.const 1
      i32.ge_s
      if  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        local.get 0
        i32.load offset=208
        i32.load offset=12
        i32.const 0
        i32.gt_s
        br_if 1 (;@1;)
      end
      local.get 0
      local.set 3
      i32.const 3470264
      i32.load
      drop
      i32.const 3470264
      i32.load
      drop
      local.get 0
      i32.load offset=192
      i32.load offset=12
      local.get 0
      i32.load offset=208
      i32.load offset=12
      i32.ne
      if  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        i32.const 1
        i32.const 2
        local.get 0
        i32.load offset=192
        i32.load offset=12
        i32.const 1
        i32.lt_s
        select
        local.set 2
      end
      local.get 3
      i32.const 0
      local.get 2
      local.get 1
      call $j13233 ;; LevelManager.BattleLose
    end
    local.get 4
    i32.const 16
    i32.add
    global.set 0)

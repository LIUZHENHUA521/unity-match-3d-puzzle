  (func $j9880 (type 7) (param i32 i32 i32 i32)
    (local i32 f32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 3
    local.get 3
    global.set 0
    i32.const 4509415
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430028
      call $j870 ;; 
      i32.const 3433944
      call $j870 ;; 
      i32.const 3434152
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3437580
      call $j870 ;; 
      i32.const 3439248
      call $j870 ;; 
      i32.const 3439288
      call $j870 ;; 
      i32.const 3539236
      call $j870 ;; 
      i32.const 3534472
      call $j870 ;; 
      i32.const 4509415
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 0
      i32.load8_u offset=465
      i32.eqz
      br_if 0 (;@1;)
      local.get 1
      if  ;; label = @2
        local.get 0
        i32.load offset=840
        local.get 2
        i32.ge_s
        br_if 1 (;@1;)
        local.get 0
        local.get 2
        i32.store offset=840
        local.get 3
        local.get 2
        i32.store offset=12
        i32.const 3433944
        i32.load
        local.get 3
        i32.const 12
        i32.add
        call $j886 ;; 
        local.set 1
        i32.const 3539236
        i32.load
        local.get 1
        i32.const 0
        call $j987 ;; 
        local.set 1
        i32.const 3430028
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        call $j6874 ;; ConfigManager.GetTeachData
        local.set 1
        local.get 1
        if  ;; label = @3
          local.get 1
          i32.load offset=24
          local.set 4
          local.get 1
          i32.load offset=28
          f32.load offset=20
          local.set 5
          i32.const 3439288
          i32.load
          call $j871 ;; 
          local.get 4
          local.get 5
          i32.const 0
          call $j30605 ;; UIGooseWordGuide.ShowWord
          local.get 1
          i32.load offset=16
          i32.const 0
          call $j30606 ;; UIGooseWordGuide.PlayWordSound
          local.get 3
          local.get 2
          i32.store offset=8
          i32.const 3433944
          i32.load
          local.get 3
          i32.const 8
          i32.add
          call $j886 ;; 
          local.set 1
          i32.const 2
          i32.const 3534472
          i32.load
          local.get 1
          i32.const 0
          call $j987 ;; 
          i32.const 0
          call $j1464 ;; GameManager.LogInColor
        end
        block  ;; label = @3
          block  ;; label = @4
            local.get 2
            i32.const 1
            i32.sub
            br_table 0 (;@4;) 1 (;@3;) 3 (;@1;)
          end
          i32.const 3437580
          i32.load
          call $j871 ;; 
          i32.const 3437580
          i32.load
          i32.load offset=92
          i32.load
          local.set 1
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 1
          i32.const 0
          call $j982 ;; 
          if  ;; label = @4
            i32.const 3437580
            i32.load
            call $j871 ;; 
            i32.const 3437580
            i32.load
            i32.load offset=92
            i32.load
            local.get 0
            f32.load offset=836
            i32.const 0
            call $j17948 ;; SoundManager.SetMusicVolume
          end
          i32.const 3439248
          i32.load
          i32.load offset=92
          i32.load
          i32.const 1
          local.get 0
          i32.load offset=192
          i32.const 0
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j944 ;; 
          i32.const 0
          call $j6345 ;; UIGamePage.ShowHand
          local.get 0
          i32.load offset=824
          local.set 1
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 1
          i32.const 0
          call $j982 ;; 
          i32.eqz
          br_if 2 (;@1;)
          local.get 0
          i32.load offset=192
          i32.const 0
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j944 ;; 
          local.set 1
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 1
          i32.const 0
          call $j982 ;; 
          i32.eqz
          br_if 2 (;@1;)
          local.get 0
          i32.load offset=824
          local.get 0
          i32.load offset=192
          i32.const 0
          i32.const 3470268
          i32.load
          call $j895 ;; 
          i32.const 0
          call $j944 ;; 
          i32.const 0
          call $j8126 ;; HighlightEffect.SetTarget
          local.get 0
          i32.load offset=824
          i32.const 1
          i32.const 0
          call $j2810 ;; HighlightEffect.SetHighlighted
          br 2 (;@1;)
        end
        i32.const 3439248
        i32.load
        i32.load offset=92
        i32.load
        i32.const 0
        i32.const 0
        i32.const 0
        call $j6345 ;; UIGamePage.ShowHand
        br 1 (;@1;)
      end
      i32.const 3437580
      i32.load
      call $j871 ;; 
      i32.const 3437580
      i32.load
      i32.load offset=92
      i32.load
      local.set 0
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 0
      i32.const 0
      call $j982 ;; 
      if  ;; label = @2
        i32.const 3437580
        i32.load
        call $j871 ;; 
        i32.const 3437580
        i32.load
        i32.load offset=92
        i32.load
        f32.const 0x1p+0 (;=1;)
        i32.const 0
        call $j17948 ;; SoundManager.SetMusicVolume
      end
      i32.const 3434152
      i32.load
      call $j871 ;; 
      i32.const 801
      i32.const 0
      call $j1369 ;; KUIManager.CloseUI
    end
    local.get 3
    i32.const 16
    i32.add
    global.set 0)

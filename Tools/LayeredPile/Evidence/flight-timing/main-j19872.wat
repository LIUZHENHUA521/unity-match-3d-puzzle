  (func $j19872 (type 8) (param i32 i32 i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 48
    i32.sub
    local.set 4
    local.get 4
    global.set 0
    i32.const 4509374
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3437592
      call $j870 ;; 
      i32.const 4509374
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 1
      i32.load8_u offset=94
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=116
      local.set 5
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 5
      i32.const 0
      call $j982 ;; 
      i32.eqz
      br_if 0 (;@1;)
      local.get 4
      i32.const 32
      i32.add
      local.get 1
      i32.const 0
      call $j944 ;; 
      i32.const 0
      call $j1028 ;; 
      local.get 4
      i32.load offset=40
      local.set 5
      local.get 4
      local.get 5
      i32.store offset=24
      local.get 4
      local.get 5
      i32.store offset=8
      local.get 4
      i32.load offset=36
      local.set 5
      local.get 4
      i32.load offset=32
      local.set 6
      local.get 4
      local.get 6
      i32.store offset=16
      local.get 4
      local.get 5
      i32.store offset=20
      local.get 4
      local.get 6
      i32.store
      local.get 4
      local.get 5
      i32.store offset=4
      local.get 0
      local.get 4
      local.get 4
      call $j19863 ;; LevelManager.PlayCombineParticle
    end
    i32.const 3434312
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load offset=12
      i32.const 4
      i32.eq
      if  ;; label = @2
        local.get 0
        i32.const 3
        local.get 4
        call $j38521 ;; LevelManager.AddNewYearProgress
        br 1 (;@1;)
      end
      i32.const 3437592
      i32.load
      call $j871 ;; 
      i32.const 0
      call $j3935 ;; SpLevelManager.CurLevelIsSpLevel
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3437592
      i32.load
      call $j871 ;; 
      i32.const 3
      i32.const 0
      call $j33488 ;; SpLevelManager.AddSpLevelProgress
    end
    local.get 1
    i32.const 0
    call $j36725 ;; ObjectUnit.OnCombine
    local.get 0
    local.get 2
    local.get 3
    local.get 4
    call $j19881 ;; LevelManager.CheckLevelState
    local.get 4
    i32.const 48
    i32.add
    global.set 0)

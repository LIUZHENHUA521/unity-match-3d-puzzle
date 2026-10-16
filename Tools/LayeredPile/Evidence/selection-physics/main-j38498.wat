  (func $j38498 (type 0) (param i32 i32)
    i32.const 4509332
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430028
      call $j870 ;; 
      i32.const 3434312
      call $j870 ;; 
      i32.const 3435240
      call $j870 ;; 
      i32.const 3548880
      call $j870 ;; 
      i32.const 4509332
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.const 1
    local.get 0
    call $j9883 ;; LevelManager.GetTouchButton
    if  ;; label = @1
      local.get 0
      i32.const 1
      local.get 0
      call $j13231 ;; LevelManager.CheckClickRay
    end
    block  ;; label = @1
      local.get 0
      i32.const 2
      local.get 0
      call $j9883 ;; LevelManager.GetTouchButton
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871 ;; 
      local.get 0
      call $j1923 ;; LevelManager.HasTwoPlayer
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 2
      local.get 0
      call $j13231 ;; LevelManager.CheckClickRay
    end
    block  ;; label = @1
      local.get 0
      i32.const 1
      local.get 0
      call $j9883 ;; LevelManager.GetTouchButton
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=440
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 0
      i32.store8 offset=440
      local.get 0
      i32.const 1
      local.get 0
      call $j19867 ;; LevelManager.GetTarget
    end
    block  ;; label = @1
      local.get 0
      i32.const 2
      local.get 0
      call $j9883 ;; LevelManager.GetTouchButton
      br_if 0 (;@1;)
      local.get 0
      i32.load8_u offset=441
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3434312
      i32.load
      call $j871 ;; 
      local.get 0
      call $j1923 ;; LevelManager.HasTwoPlayer
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 0
      i32.store8 offset=441
      local.get 0
      i32.const 2
      local.get 0
      call $j19867 ;; LevelManager.GetTarget
    end)

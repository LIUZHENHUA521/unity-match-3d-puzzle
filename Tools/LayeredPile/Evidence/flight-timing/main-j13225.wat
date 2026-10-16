  (func $j13225 (type 1) (param i32 i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 2
    local.get 2
    global.set 0
    i32.const 4509388
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870 ;; 
      i32.const 3432052
      call $j870 ;; 
      i32.const 3434152
      call $j870 ;; 
      i32.const 3434312
      call $j870 ;; 
      i32.const 3434544
      call $j870 ;; 
      i32.const 3566400
      call $j870 ;; 
      i32.const 3566404
      call $j870 ;; 
      i32.const 4509388
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
      i32.load offset=12
      i32.eqz
      if  ;; label = @2
        i32.const 3434312
        i32.load
        call $j871 ;; 
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.const 1
        i32.store8 offset=36
      end
      local.get 0
      i32.const 1
      local.get 2
      call $j13226 ;; LevelManager.LostTarget
      local.get 0
      i32.const 3
      i32.const 4
      local.get 1
      select
      local.get 2
      call $j13232 ;; LevelManager.CanUseTool
      i32.eqz
      if  ;; label = @2
        i32.const 3566400
        i32.const 3566404
        local.get 1
        select
        i32.load
        local.set 0
        i32.const 3434152
        i32.load
        call $j871 ;; 
        i32.const 605
        local.get 0
        i32.const 0
        call $j1117 ;; KUIManager.OpenATip
        local.get 1
        i32.const 1
        i32.xor
        local.set 0
        i32.const 0
        local.get 0
        i32.const 0
        call $j8154 ;; GameManager.GameLose
        local.get 2
        local.get 0
        i32.store offset=12
        i32.const 3434544
        i32.load
        local.set 0
        local.get 2
        i32.const -1
        i32.store offset=4
        local.get 2
        local.get 0
        i32.store
        local.get 2
        local.get 2
        i32.load offset=12
        i32.store offset=8
        i32.const 1
        local.get 2
        i32.const 0
        call $j956 ;; 
        i32.const 0
        call $j1464 ;; GameManager.LogInColor
        br 1 (;@1;)
      end
      i32.const 3428556
      i32.load
      call $j871 ;; 
      i32.const 3428556
      i32.load
      i32.load offset=92
      i32.load offset=12
      local.set 0
      local.get 1
      if  ;; label = @2
        local.get 0
        i32.eqz
        br_if 1 (;@1;)
        local.get 0
        i32.const 3
        i32.const 0
        call $j1552 ;; 
        br 1 (;@1;)
      end
      local.get 0
      i32.eqz
      br_if 0 (;@1;)
      local.get 0
      i32.const 4
      i32.const 0
      call $j1552 ;; 
    end
    local.get 2
    i32.const 16
    i32.add
    global.set 0)

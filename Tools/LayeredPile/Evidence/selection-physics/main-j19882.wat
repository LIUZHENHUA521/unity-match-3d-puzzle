  (func $j19882 (type 0) (param i32 i32)
    (local i32 i32 f32)
    i32.const 4509359
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 3470264
      call $j870 ;; 
      i32.const 3470268
      call $j870 ;; 
      i32.const 4509359
      i32.const 1
      i32.store8
    end
    i32.const 0
    local.set 1
    loop  ;; label = @1
      i32.const 3470264
      i32.load
      drop
      local.get 1
      local.get 0
      i32.load offset=192
      i32.load offset=12
      i32.lt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895 ;; 
        local.set 2
        i32.const 3434312
        i32.load
        call $j871 ;; 
        local.get 0
        local.set 3
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load8_u offset=4
        if  ;; label = @3
          local.get 0
          f32.load offset=44
          local.set 4
        else
          f32.const 0x1p+0 (;=1;)
          local.set 4
        end
        local.get 3
        local.get 2
        i32.const 1
        local.get 4
        local.get 0
        call $j5869 ;; LevelManager.CheckRigid
        local.get 0
        local.get 0
        i32.load offset=192
        local.get 1
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 1
        local.get 0
        call $j9885 ;; LevelManager.CheckVisible
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 1 (;@1;)
      end
    end
    i32.const 3434312
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      local.get 0
      call $j1923 ;; LevelManager.HasTwoPlayer
      i32.eqz
      br_if 0 (;@1;)
      i32.const 0
      local.set 1
      loop  ;; label = @2
        i32.const 3470264
        i32.load
        drop
        local.get 1
        local.get 0
        i32.load offset=208
        i32.load offset=12
        i32.ge_s
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=208
        local.get 1
        i32.const 3470268
        i32.load
        call $j895 ;; 
        local.set 2
        i32.const 3434312
        i32.load
        call $j871 ;; 
        local.get 0
        local.set 3
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load8_u offset=4
        if  ;; label = @3
          local.get 0
          f32.load offset=44
          local.set 4
        else
          f32.const 0x1p+0 (;=1;)
          local.set 4
        end
        local.get 3
        local.get 2
        i32.const 2
        local.get 4
        local.get 0
        call $j5869 ;; LevelManager.CheckRigid
        local.get 0
        local.get 0
        i32.load offset=208
        local.get 1
        i32.const 3470268
        i32.load
        call $j895 ;; 
        i32.const 2
        local.get 0
        call $j9885 ;; LevelManager.CheckVisible
        local.get 1
        i32.const 1
        i32.add
        local.set 1
        br 0 (;@2;)
      end
      unreachable
    end)

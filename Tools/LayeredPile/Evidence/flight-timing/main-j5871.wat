  (func $j5871 (type 4) (param i32) (result i32)
    i32.const 4509369
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870 ;; 
      i32.const 4509369
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871 ;; 
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load8_u offset=81
    if  ;; label = @1
      i32.const 3434312
      i32.load
      call $j871 ;; 
      i32.const 0
      call $j1923 ;; LevelManager.HasTwoPlayer
      i32.const 1
      i32.xor
      local.set 0
    else
      i32.const 0
      local.set 0
    end
    local.get 0
    return)

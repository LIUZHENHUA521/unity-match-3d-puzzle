  (func $j13407 (type 0) (param i32 i32)
    i32.const 4505049
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3429928
      call $j870 ;; 
      i32.const 3543512
      call $j870 ;; 
      i32.const 4505049
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=504
    i32.eqz
    if  ;; label = @1
      i32.const 3429928
      i32.load
      call $j873 ;; 
      local.set 1
      local.get 1
      i32.const 0
      call $j5323 ;; 
      local.get 0
      local.get 1
      i32.store offset=504
      local.get 0
      i32.load offset=504
      local.set 1
      local.get 0
      i32.const 0
      call $j1073 ;; 
      local.set 0
      local.get 1
      i32.const 3543512
      i32.load
      local.get 0
      i32.const 0
      call $j910 ;; 
      i32.const 0
      call $j5322 ;; 
    end)

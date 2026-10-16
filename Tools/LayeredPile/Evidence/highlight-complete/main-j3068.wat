  (func $j3068 (type 7) (param i32 i32 i32 i32)
    i32.const 4505038
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430720
      call $j870 ;; 
      i32.const 3434660
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3521456
      call $j870 ;; 
      i32.const 3543116
      call $j870 ;; 
      i32.const 4505038
      i32.const 1
      i32.store8
    end
    local.get 1
    i32.load
    local.set 3
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 3
    i32.const 0
    i32.const 0
    call $j924 ;; 
    if  ;; label = @1
      local.get 2
      i32.const 0
      call $j2254 ;; 
      local.set 3
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 3
      i32.const 0
      i32.const 0
      call $j924 ;; 
      if  ;; label = @2
        i32.const 3543116
        i32.load
        local.get 2
        i32.const 3521456
        i32.load
        i32.const 0
        call $j960 ;; 
        local.set 1
        i32.const 3430720
        i32.load
        call $j871 ;; 
        local.get 1
        i32.const 0
        call $j1062 ;; 
        local.get 0
        i32.const 0
        i32.const 0
        call $j1401 ;; 
        return
      end
      i32.const 3434660
      i32.load
      call $j873 ;; 
      local.set 0
      local.get 0
      local.get 3
      i32.const 0
      call $j2327 ;; 
      local.get 1
      local.get 0
      i32.store
    end)

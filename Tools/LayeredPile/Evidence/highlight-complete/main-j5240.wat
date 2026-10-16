  (func $j5240 (type 1) (param i32 i32 i32)
    i32.const 4505025
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505025
      i32.const 1
      i32.store8
    end
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 1
    i32.const 0
    i32.const 0
    call $j915 ;; 
    if  ;; label = @1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
      i32.const 0
      call $j1801 ;; 
    end)

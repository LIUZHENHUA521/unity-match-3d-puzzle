  (func $j13405 (type 2) (param i32 i32) (result i32)
    (local i32)
    i32.const 4505018
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      call $j870 ;; 
      i32.const 3495724
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505018
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=448
    local.set 2
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 0
    i32.const 448
    i32.add
    local.set 1
    block  ;; label = @1
      local.get 2
      i32.const 0
      i32.const 0
      call $j924 ;; 
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=24
      local.set 2
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 2
      i32.const 0
      i32.const 0
      call $j915 ;; 
      i32.eqz
      br_if 0 (;@1;)
      i32.const 3432484
      i32.load
      call $j871 ;; 
      i32.const 3432484
      i32.load
      i32.load offset=92
      i32.load offset=24
      local.set 2
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 0
      local.get 2
      i32.const 3495724
      i32.load
      call $j1764 ;; 
      i32.store offset=448
      local.get 0
      i32.load offset=448
      local.get 0
      i32.load8_u offset=761
      i32.const 0
      call $j9812 ;; 
    end
    local.get 1
    i32.load
    return)

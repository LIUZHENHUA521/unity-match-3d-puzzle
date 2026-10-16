  (func $j8123 (type 2) (param i32 i32) (result i32)
    i32.const 4505020
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3432484
      call $j870 ;; 
      i32.const 3495724
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4505020
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=456
    local.set 1
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 0
    i32.const 456
    i32.add
    local.set 0
    block  ;; label = @1
      local.get 1
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
      i32.load offset=40
      local.set 1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
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
      i32.load offset=40
      local.set 1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 0
      local.get 1
      i32.const 3495724
      i32.load
      call $j1764 ;; 
      i32.store
    end
    local.get 0
    i32.load
    return)

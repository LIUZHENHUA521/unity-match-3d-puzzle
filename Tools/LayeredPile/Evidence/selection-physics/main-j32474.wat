  (func $j32474 (type 0) (param i32 i32)
    i32.const 4507871
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3435416
      call $j870 ;; 
      i32.const 4507871
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=16
    local.set 1
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 1
    i32.const 0
    i32.const 0
    call $j915 ;; Object.op_Inequality
    if  ;; label = @1
      local.get 1
      i32.const 0
      i32.const 0
      local.get 0
      call $j6603 ;; ObjectUnit.BeSelect
      local.get 0
      i32.const 16
      i32.add
      i32.const 0
      i32.store
    end)

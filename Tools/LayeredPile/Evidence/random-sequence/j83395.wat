  (func $j83395 (type 0) (param i32 i32)
    i32.const 4509429
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3430028
      call $j870
      i32.const 3548880
      call $j870
      i32.const 4509429
      i32.const 1
      i32.store8
    end
    local.get 0
    local.get 0
    call $j19884
    if  ;; label = @1
      i32.const 3430028
      i32.load
      call $j871
      i32.const 3548880
      i32.load
      i32.const 0
      call $j4798
      drop
      local.get 0
      f32.const 0x0p+0 (;=0;)
      local.get 0
      call $j19858
    end)
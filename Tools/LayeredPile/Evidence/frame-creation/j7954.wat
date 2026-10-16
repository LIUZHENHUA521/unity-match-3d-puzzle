  (func $j7954 (type 1) (param i32 i32 i32)
    i32.const 4507827
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434312
      call $j870
      i32.const 4507827
      i32.const 1
      i32.store8
    end
    block  ;; label = @1
      local.get 1
      if  ;; label = @2
        local.get 0
        i32.load8_u offset=18
        br_if 1 (;@1;)
      end
      i32.const 3434312
      i32.load
      call $j871
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load offset=12
      i32.const 2
      i32.eq
      i32.const 0
      local.get 1
      select
      br_if 0 (;@1;)
      local.get 1
      local.get 1
      call $j19262
      i32.const 0
      local.get 1
      select
      br_if 0 (;@1;)
      local.get 0
      i32.load offset=20
      local.get 1
      i32.const 1
      i32.xor
      i32.const 0
      call $j6539
    end)